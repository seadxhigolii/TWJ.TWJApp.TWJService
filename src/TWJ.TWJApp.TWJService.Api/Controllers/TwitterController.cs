using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.AddTemplate;
using Tweetinvi;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Tweetinvi.Models;
using System.Net.Http;
using Tweetinvi.Credentials.Models;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using Google.Rpc;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelperService;
        private readonly string _twitterClientID;
        private readonly string _twitterClientSecret;
        private readonly string _twitterAPIKey;
        private readonly string _twitterAPISecretKey;
        private readonly string _twitterAccessToken;
        private readonly string _twitterRefreshToken;
        private readonly string _redirectUri = "http://localhost:5001/api/Twitter/Callback";
        private readonly string _twitterOAuthUri = "https://twitter.com/i/oauth2/authorize?response_type=code&client_id={ClientID}&redirect_uri={RedirectURI}&scope=tweet.read%20tweet.write%20users.read%20offline.access&state={State}&code_challenge={CodeChallenge}&code_challenge_method=S256\r\n";
        private readonly string _aesKey;
        private readonly string _aesIV;

        public TwitterController(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ITWJAppDbContext context, IGlobalHelperService globalHelperService)
        {
            _configuration = configuration;
            _twitterClientID = configuration["Twitter:ClientID"];
            _twitterClientSecret = configuration["Twitter:ClientSecret"];
            _twitterAPIKey = configuration["Twitter:Key"];
            _twitterAPISecretKey = configuration["Twitter:SecretKey"];
            _twitterAccessToken = configuration["Twitter:access_token"];
            _twitterRefreshToken = configuration["Twitter:refresh_token"];
            _aesKey = configuration["AES:Key"];
            _aesIV = configuration["AES:IV"];
            _httpClientFactory = httpClientFactory;
            _cache = memoryCache;
            _context = context;
            _globalHelperService = globalHelperService;
        }

        [HttpGet("StartAuthorization")]
        public IActionResult StartAuthorization()
        {
            try
            {
                string state = GenerateState();
                string codeVerifier = GenerateCodeVerifier();
                string codeChallenge = GenerateCodeChallenge(codeVerifier);

                string authorizationUrl = $"https://twitter.com/i/oauth2/authorize?response_type=code&client_id={_twitterClientID}&redirect_uri={_redirectUri}&scope=tweet.read%20tweet.write%20users.read%20offline.access&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256";
                _cache.Set("codeVerifier", codeVerifier, TimeSpan.FromMinutes(10));
                // Redirect with state and code_verifier as query parameters
                var finalRedirectUri = $"{authorizationUrl}&custom_code_verifier={codeVerifier}";
                return Redirect(finalRedirectUri);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> OAuthCallback(string code, string state)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.twitter.com/2/oauth2/token");

            if (!_cache.TryGetValue("codeVerifier", out string codeVerifier))
            {
                return BadRequest("Invalid or expired state parameter");
            }
            var formData = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", _redirectUri},
                {"client_id", _twitterClientID},
                {"code_verifier", codeVerifier}
            };

            request.Content = new FormUrlEncodedContent(formData);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_twitterClientID}:{_twitterClientSecret}"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseString);
            }

            var tokenResponse = JsonConvert.DeserializeObject<TwitterTokenResponse>(responseString);

            var accessToken = tokenResponse.AccessToken;
            var refreshToken = tokenResponse.RefreshToken;
            var accessTokenExpiresIn = tokenResponse.ExpiresIn;

            byte[] key = Convert.FromBase64String(_aesKey);
            byte[] iv = Convert.FromBase64String(_aesIV);
            var encryptedAccessToken = _globalHelperService.EncryptStringToBytes_Aes(accessToken, key, iv);
            var encryptedRefreshToken = _globalHelperService.EncryptStringToBytes_Aes(refreshToken, key, iv);


            var twitterAccessTokenSetting = await _context.UserSettings
                .FirstOrDefaultAsync(us => us.Key == "twitter_access_token");
            var twitterRefreshTokenSetting = await _context.UserSettings
                .FirstOrDefaultAsync(us => us.Key == "twitter_refresh_token");
            var twitterAccessTokenExpiresIn = await _context.UserSettings
                .FirstOrDefaultAsync(us => us.Key == "twitter_access_token_expires_in");

            if (twitterAccessTokenSetting != null)
            {
                twitterAccessTokenSetting.Value = Convert.ToBase64String(encryptedAccessToken);
                _context.UserSettings.Update(twitterAccessTokenSetting);
            }
            else
            {
                twitterAccessTokenSetting = new UserSettings
                {
                    Key = "twitter_access_token",
                    Value = Convert.ToBase64String(encryptedAccessToken)
                };
                await _context.UserSettings.AddAsync(twitterAccessTokenSetting);
            }

            if (twitterRefreshTokenSetting != null)
            {
                twitterRefreshTokenSetting.Value = Convert.ToBase64String(encryptedRefreshToken);
                _context.UserSettings.Update(twitterRefreshTokenSetting);
            }
            else
            {
                twitterRefreshTokenSetting = new UserSettings
                {
                    Key = "twitter_refresh_token",
                    Value = Convert.ToBase64String(encryptedRefreshToken)
                };
                await _context.UserSettings.AddAsync(twitterRefreshTokenSetting);
            }

            if (twitterAccessTokenExpiresIn != null)
            {
                twitterAccessTokenExpiresIn.Value = DateTime.Now.AddSeconds(accessTokenExpiresIn).ToString("yyyy-MM-dd HH:mm:ss.fff zzz") ;
                _context.UserSettings.Update(twitterAccessTokenExpiresIn);
            }
            else
            {
                twitterAccessTokenExpiresIn = new UserSettings
                {
                    Key = "twitter_access_token_expires_in",
                    Value = DateTime.Now.AddSeconds(accessTokenExpiresIn).ToString("yyyy-MM-dd HH:mm:ss.fff zzz")
            };
                await _context.UserSettings.AddAsync(twitterAccessTokenExpiresIn);
            }

            await _context.SaveChangesAsync();

            return Ok(tokenResponse);
        }




        [HttpPost("get-token")]
        public async Task<IActionResult> GetToken([FromBody] string authCode)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.twitter.com/2/oauth2/token");
            string codeVerifier = GenerateCodeVerifier();
            var formData = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", authCode},
                {"redirect_uri", _redirectUri},
                {"client_id", _twitterClientID},
                {"code_verifier", codeVerifier}
            };

            request.Content = new FormUrlEncodedContent(formData);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("YOUR_CLIENT_ID:YOUR_CLIENT_SECRET")));

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseString);
            }

            var tokenResponse = JsonConvert.DeserializeObject<TwitterOAuthResponse>(responseString);
            return Ok(tokenResponse);
        }

        [HttpPost("PostTweet")]
        public async Task<IActionResult> PostTweet([FromBody] TweetRequest tweetRequest)
        {
            var accessToken = _twitterAccessToken;
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.twitter.com/2/tweets")
            {
                Content = new StringContent(JsonConvert.SerializeObject(tweetRequest), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseString);
            }

            return Ok(responseString);
        }
        private static string GenerateCodeVerifier()
        {
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            return Base64UrlEncode(bytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string GenerateState()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[32];
                rng.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray);
            }
        }
    }
}

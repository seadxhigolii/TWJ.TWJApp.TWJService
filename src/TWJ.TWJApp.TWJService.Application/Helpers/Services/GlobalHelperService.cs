using System;
using System.Linq;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Drawing;
using static AngleSharp.Css.Values.Angle;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace TWJ.TWJApp.TWJService.Application.Helpers.Services
{
    public class GlobalHelperService : IGlobalHelperService
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private readonly string _instagramAccessToken;
        private readonly string _facebookPageID;
        private readonly string _twitterApiKey;
        private readonly string _twitterSecretKey;
        private readonly string _twitterAccessToken10;
        private readonly string _twitterAccessTokenSecret10;
        private readonly string _twitterAccessToken20;
        private readonly string _twitterRefreshToken20;
        private readonly string _twitterClientID;
        private readonly string _twitterClientSecret;
        private readonly string _aesKey;
        private readonly string _aesIV;

        public GlobalHelperService(ITWJAppDbContext context, IMemoryCache cache, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _instagramAccessToken = _configuration["Instagram:LongLiveAccessToken"];
            _facebookPageID = _configuration["Instagram:FacebookPageID"];
            _twitterApiKey = _configuration["Twitter:Key"];
            _twitterSecretKey = _configuration["Twitter:SecretKey"];
            _twitterAccessToken20 = _configuration["Twitter:access_token"];
            _twitterAccessToken10 = _configuration["Twitter:AccessToken"];
            _twitterAccessTokenSecret10 = _configuration["Twitter:AccessTokenSecret"];
            _twitterRefreshToken20 = _configuration["Twitter:refresh_token"];
            _twitterClientID = _configuration["Twitter:ClientID"];
            _twitterClientSecret = _configuration["Twitter:ClientSecret"];
            _aesKey = configuration["AES:Key"];
            _aesIV = configuration["AES:IV"];
            _httpClientFactory = httpClientFactory;
        }

        public async Task<User> GetUserFromCache()
        {
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var userId = GetUserIdFromToken(token);

            var user = await _context.User.FindAsync(userId);

            if (user != null)
            {
                return user;
            }

            return null;
        }


        private Guid GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                throw new ArgumentException("Invalid token");
            }

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new ArgumentException("User ID claim not found or invalid in the token");
            }

            return userId;
        }



        public async Task<bool> Log(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            Log log = new Log()
            {
                Class = className,
                Method = methodName,
                Message = ex.Message,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public string TitleToUrlSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            var slug = title.ToLowerInvariant();
            slug = slug.Trim();
            slug = Regex.Replace(slug, @"[^a-z0-9\s]", "");
            slug = Regex.Replace(slug, @"\s+", "-");

            var sameURLBlogPost = _context.BlogPosts.Any(x => x.URL == slug);

            if (sameURLBlogPost)
            {
                int suffixNumber = 1;
                string baseSlug = slug;
                string newSlug = $"{baseSlug}-no-{suffixNumber}";

                var match = Regex.Match(slug, @"-no-(\d+)$");
                if (match.Success)
                {
                    suffixNumber = int.Parse(match.Groups[1].Value) + 1;
                    baseSlug = slug.Substring(0, slug.LastIndexOf("-no-"));
                    newSlug = $"{baseSlug}-no-{suffixNumber}";
                }

                while (_context.BlogPosts.Any(x => x.URL == newSlug))
                {
                    suffixNumber++;
                    newSlug = $"{baseSlug}-no-{suffixNumber}";
                }

                slug = newSlug;
            }

            return slug;
        }


        public string RemoveTextWithinSquareBrackets(string input)
        {
            string pattern = @"\[.*?\]";
            return Regex.Replace(input, pattern, "");
        }
        public int CalculateFontSize(string text, RectangleF rect)
        {
            float area = rect.Width * rect.Height;

            int baseFontSize = (int)Math.Sqrt(area / text.Length);

            int fontSize = (int)(baseFontSize * 0.6); // Adjustable

            return Math.Max(12, Math.Min(fontSize, 72));
        }
        public void DrawTextWithSpacing(Graphics graphics, string text, Font font, Brush brush, RectangleF rect, float letterSpacing, float spaceSpacing, StringFormat format)
        {
            SizeF textSize = graphics.MeasureString(text, font, int.MaxValue, format);
            float totalWidth = textSize.Width + (text.Length - 1) * letterSpacing;

            float startX = rect.X + (rect.Width - totalWidth) / 2;
            float y = rect.Y + (rect.Height - textSize.Height) / 2;

            for (int i = 0; i < text.Length; i++)
            {
                string character = text[i].ToString();
                graphics.DrawString(character, font, brush, startX, y, format);
                SizeF charSize = graphics.MeasureString(character, font, int.MaxValue, format);

                if (character == " ")
                {
                    startX += charSize.Width + spaceSpacing;
                }
                else
                {
                    startX += charSize.Width + letterSpacing;
                }
            }
        }

        public async Task<Unit> PostToInstagramAsync(string imageUrl, string quoteContent, string caption, CancellationToken cancellationToken = default)
        {
            string currentClassName = nameof(GlobalHelperService);

            try
            {
                var client = _httpClientFactory.CreateClient();

                string businessAccountRequestUrl = $"https://graph.facebook.com/v20.0/{_facebookPageID}?fields=instagram_business_account&access_token={_instagramAccessToken}";

                var businessAccountResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.GetAsync(businessAccountRequestUrl, cancellationToken);
                });

                var businessAccountContent = await businessAccountResponse.Content.ReadAsStringAsync();

                if (!businessAccountResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error retrieving Instagram Business Account ID: {businessAccountContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                var businessAccountId = JObject.Parse(businessAccountContent)["instagram_business_account"]["id"].ToString();

                var createMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media";
                var createMediaContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("image_url", imageUrl),
                    new KeyValuePair<string, string>("caption", caption),
                    new KeyValuePair<string, string>("access_token", _instagramAccessToken)
                });

                var createMediaResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(createMediaUrl, createMediaContent, cancellationToken);
                });

                var createMediaResponseContent = await createMediaResponse.Content.ReadAsStringAsync();

                if (!createMediaResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error creating media object: {createMediaResponseContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                var mediaObjectId = JObject.Parse(createMediaResponseContent)["id"].ToString();

                var publishMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media_publish";
                var publishMediaContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("creation_id", mediaObjectId),
                    new KeyValuePair<string, string>("access_token", _instagramAccessToken)
                });

                var publishMediaResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(publishMediaUrl, publishMediaContent, cancellationToken);
                });

                var publishMediaResponseContent = await publishMediaResponse.Content.ReadAsStringAsync();

                if (!publishMediaResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error publishing media object: {publishMediaResponseContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                return Unit.None;
            }
            catch (HttpRequestException ex)
            {
                await Log(ex, currentClassName);
                throw new ApplicationException("Error posting to Instagram: " + ex.Message);
            }
            catch (Exception ex)
            {
                await Log(ex, currentClassName);
                throw new ApplicationException("An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<Unit> PostToTwitterAsync(string imageUrl, string caption, CancellationToken cancellationToken = default)
        {
            string currentClassName = nameof(GlobalHelperService);

            try
            {
                HttpRequestMessage request = new HttpRequestMessage();
                var accessToken10 = _twitterAccessToken10;
                var accessTokenSecret10 = _twitterAccessTokenSecret10;
                var accessToken20 = await _context.UserSettings.Where(key => key.Key == "twitter_access_token").FirstOrDefaultAsync();
                var twitterApiKey = _twitterApiKey;
                var twitterApiSecret = _twitterSecretKey;
                var client = _httpClientFactory.CreateClient();

                if(!string.IsNullOrEmpty(imageUrl))
                {
                    var imageBytes = await DownloadImageAsync(imageUrl, client, cancellationToken);
                    var mediaId = await UploadMediaToTwitterAsync(imageBytes, accessToken10, accessTokenSecret10, twitterApiKey, twitterApiSecret, client, cancellationToken); 
                    request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.twitter.com/2/tweets")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            text = caption,
                            media = new
                            {
                                media_ids = new string[] { mediaId }
                            }
                        }), Encoding.UTF8, "application/json")
                    };

                }
                else
                {
                    request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.twitter.com/2/tweets")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            text = caption
                        }), Encoding.UTF8, "application/json")
                    };
                }

                var accessTokenExpiresIn = await _context.UserSettings.Where(key => key.Key == "twitter_access_token_expires_in").FirstOrDefaultAsync();

                DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(accessTokenExpiresIn.Value, "yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);
                
                if (dateTimeOffset <= DateTime.Now.AddMinutes(10))
                {
                    accessToken20.Value = await RefreshAccessTokenAsync(_twitterClientID, _twitterClientSecret);
                }

                byte[] key = Convert.FromBase64String(_aesKey.Trim());
                byte[] iv = Convert.FromBase64String(_aesIV.Trim());

                var decryptedAccessToken20 = DecryptStringFromBytes_Aes(Convert.FromBase64String(accessToken20.Value.Trim()), key, iv);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", decryptedAccessToken20);

                var response = await client.SendAsync(request, cancellationToken);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"Error posting to Twitter: {responseString}");
                }

                return Unit.None;
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Error posting to Twitter: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        public async Task<Unit> PostReelToInstagramAsync(string videoUrl, string caption, CancellationToken cancellationToken = default)
        {
            string currentClassName = nameof(GlobalHelperService);

            try
            {
                var client = _httpClientFactory.CreateClient();

                string businessAccountRequestUrl = $"https://graph.facebook.com/v20.0/{_facebookPageID}?fields=instagram_business_account&access_token={_instagramAccessToken}";

                var businessAccountResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.GetAsync(businessAccountRequestUrl, cancellationToken);
                });

                var businessAccountContent = await businessAccountResponse.Content.ReadAsStringAsync();

                if (!businessAccountResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error retrieving Instagram Business Account ID: {businessAccountContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                var businessAccountId = JObject.Parse(businessAccountContent)["instagram_business_account"]["id"].ToString();

                // Create video media object
                var createMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media";
                var createMediaContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("video_url", videoUrl),
                    new KeyValuePair<string, string>("caption", caption),
                    new KeyValuePair<string, string>("media_type", "REELS"),
                    new KeyValuePair<string, string>("share_to_feed", "true"),
                    new KeyValuePair<string, string>("access_token", _instagramAccessToken)
                });

                var createMediaResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(createMediaUrl, createMediaContent, cancellationToken);
                });

                var createMediaResponseContent = await createMediaResponse.Content.ReadAsStringAsync();

                if (!createMediaResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error creating media object: {createMediaResponseContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                var mediaObjectId = JObject.Parse(createMediaResponseContent)["id"].ToString();

                // Wait for the media object to be ready for publishing
                bool isMediaReady = false;
                int retries = 0;
                const int maxRetries = 20;

                while (!isMediaReady && retries < maxRetries)
                {
                    await Task.Delay(5000, cancellationToken); // Wait for 5 seconds

                    var checkStatusUrl = $"https://graph.facebook.com/v14.0/{mediaObjectId}?fields=status_code,status&access_token={_instagramAccessToken}";
                    var checkStatusResponse = await client.GetAsync(checkStatusUrl, cancellationToken);
                    var checkStatusContent = await checkStatusResponse.Content.ReadAsStringAsync();

                    if (!checkStatusResponse.IsSuccessStatusCode)
                    {
                        var errorMessage = $"Error checking media status: {checkStatusContent}";
                        await Log(new Exception(errorMessage), currentClassName);
                        throw new ApplicationException(errorMessage);
                    }

                    var checkStatusResponseJson = JObject.Parse(checkStatusContent);
                    var statusCode = checkStatusResponseJson["status_code"].ToString();
                    var status = checkStatusResponseJson["status"].ToString();

                    if (statusCode == "READY")
                    {
                        isMediaReady = true;
                    }
                    else if(statusCode == "FINISHED")
                    {
                        isMediaReady = true;
                    }
                    else if (statusCode == "ERROR")
                    {
                        var errorMessage = $"Media processing failed: {checkStatusContent}";
                        await Log(new Exception(errorMessage), currentClassName);
                        throw new ApplicationException(errorMessage);
                    }

                    retries++;
                }

                if (!isMediaReady)
                {
                    var errorMessage = "Media is not ready for publishing after multiple retries.";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                // Publish video media object
                var publishMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media_publish";
                var publishMediaContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("creation_id", mediaObjectId),
                    new KeyValuePair<string, string>("access_token", _instagramAccessToken)
                });

                var publishMediaResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(publishMediaUrl, publishMediaContent, cancellationToken);
                });

                var publishMediaResponseContent = await publishMediaResponse.Content.ReadAsStringAsync();

                if (!publishMediaResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error publishing media object: {publishMediaResponseContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                return Unit.None;
            }
            catch (Exception ex)
            {
                await Log(ex, currentClassName);
                throw new ApplicationException("An unexpected error occurred: " + ex.Message);
            }
        }





        private async Task<byte[]> DownloadImageAsync(string imageUrl, HttpClient client, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(imageUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        private async Task<string> UploadMediaToTwitterAsync(byte[] imageBytes, string accessToken10, string accessToken10Secret, string apiKey, string apiSecretKey, HttpClient client, CancellationToken cancellationToken)
        {
            var url = "https://upload.twitter.com/1.1/media/upload.json";
            var oauthHeader = CreateOAuthHeader(accessToken10, accessToken10Secret, apiKey, apiSecretKey, "POST", url, new Dictionary<string, string>());

            using var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(imageBytes), "media" }
            };

            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Add("Authorization", oauthHeader);

            var response = await client.SendAsync(request, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Error uploading media to Twitter: {responseString}");
            }

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
            return jsonResponse.media_id_string;
        }


        private string CreateOAuthHeader(string accessToken, string accessTokenSecret, string apiKey, string apiSecretKey, string httpMethod, string url, Dictionary<string, string> additionalParameters)
        {
            var oauthParameters = new Dictionary<string, string>
            {
                { "oauth_consumer_key", apiKey },
                { "oauth_nonce", GenerateNonce() },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", GenerateTimestamp() },
                { "oauth_token", accessToken },
                { "oauth_version", "1.0" }
            };

            // Combine OAuth parameters and additional parameters for the signature
            var allParameters = oauthParameters.Concat(additionalParameters).ToDictionary(p => p.Key, p => p.Value);

            var baseString = CreateSignatureBaseString(httpMethod, url, allParameters);
            var signature = SignBaseString(baseString, apiSecretKey, accessTokenSecret);

            oauthParameters.Add("oauth_signature", signature);

            var headerString = string.Join(", ", oauthParameters.Select(p => $"{Uri.EscapeDataString(p.Key)}=\"{Uri.EscapeDataString(p.Value)}\""));

            return $"OAuth {headerString}";
        }

        private string GenerateNonce()
        {
            return Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        }
        private string GenerateTimestamp()
        {
            var secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            return secondsSinceEpoch.ToString();
        }

        private string CreateSignatureBaseString(string httpMethod, string url, Dictionary<string, string> parameters)
        {
            var sortedParameters = parameters.OrderBy(p => p.Key)
                                             .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}")
                                             .ToArray();

            var parameterString = string.Join("&", sortedParameters);

            return $"{httpMethod.ToUpper()}&{Uri.EscapeDataString(url)}&{Uri.EscapeDataString(parameterString)}";
        }
        private string SignBaseString(string baseString, string consumerSecret, string tokenSecret)
        {
            var key = $"{Uri.EscapeDataString(consumerSecret)}&{Uri.EscapeDataString(tokenSecret)}";
            using (var hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(key)))
            {
                return Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }
        }

        public async Task<string> RefreshAccessTokenAsync(string clientId, string clientSecret)
        {
            try
            {
                var accessToken = await _context.UserSettings.Where(key=> key.Key == "twitter_access_token").FirstOrDefaultAsync();
                var refreshToken = await _context.UserSettings.Where(key => key.Key == "twitter_refresh_token").FirstOrDefaultAsync();
                var expiresIn = await _context.UserSettings.Where(key => key.Key == "twitter_access_token_expires_in").FirstOrDefaultAsync();
                var client = new HttpClient();

                byte[] key = Convert.FromBase64String(_aesKey);
                byte[] iv = Convert.FromBase64String(_aesIV);

                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

                var decryptedRefreshToken = DecryptStringFromBytes_Aes(Convert.FromBase64String(refreshToken.Value), key, iv);

                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", decryptedRefreshToken)
                });

                var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://api.twitter.com/2/oauth2/token")
                {
                    Content = requestContent
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"Error refreshing access token: {responseString}");
                }

                dynamic responseObject = JsonConvert.DeserializeObject(responseString);

                string newAccessToken = responseObject.access_token;
                string newRefreshToken = responseObject.refresh_token ?? refreshToken;
                string newAccessTokenExpiresIn = responseObject.expires_in;

                var encryptedAccessToken = EncryptStringToBytes_Aes(newAccessToken, key, iv);
                var encryptedRefreshToken = EncryptStringToBytes_Aes(newRefreshToken, key, iv);

                accessToken.Value = Convert.ToBase64String(encryptedAccessToken);
                refreshToken.Value = Convert.ToBase64String(encryptedRefreshToken);
                refreshToken.Value = Convert.ToBase64String(encryptedRefreshToken);
                expiresIn.Value = DateTime.Now.AddSeconds(Convert.ToInt32(newAccessTokenExpiresIn)).ToString("yyyy-MM-dd HH:mm:ss.fff zzz");

                _context.UserSettings.UpdateRange(accessToken, refreshToken);

                await _context.SaveChangesAsync();

                return newAccessToken;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Unit> PostToFacebookAsync(string imageUrl, string quoteContent, string caption, CancellationToken cancellationToken)
        {
            string currentClassName = nameof(GlobalHelperService);

            try
            {
                var client = _httpClientFactory.CreateClient();

                var createPostUrl = $"https://graph.facebook.com/v14.0/{_facebookPageID}/photos";
                var createPostContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("url", imageUrl),
                    new KeyValuePair<string, string>("caption", caption),
                    new KeyValuePair<string, string>("access_token", _instagramAccessToken)
                });

                var createPostResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(createPostUrl, createPostContent, cancellationToken);
                });

                var createPostResponseContent = await createPostResponse.Content.ReadAsStringAsync();

                if (!createPostResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error creating Facebook post: {createPostResponseContent}";
                    await Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                return Unit.None;
            }
            catch (HttpRequestException ex)
            {
                await Log(ex, currentClassName);
                throw new ApplicationException("Error posting to Facebook: " + ex.Message);
            }
            catch (Exception ex)
            {
                await Log(ex, currentClassName);
                throw new ApplicationException("An unexpected error occurred: " + ex.Message);
            }
        }
        private async Task<HttpResponseMessage> SendHttpRequestWithRetry(Func<Task<HttpResponseMessage>> httpRequestFunc, int maxRetries = 3)
        {
            int retries = 0;
            HttpResponseMessage response = null;

            while (retries < maxRetries)
            {
                try
                {
                    response = await httpRequestFunc();
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                }
                catch (HttpRequestException)
                {
                    if (retries == maxRetries - 1)
                    {
                        throw;
                    }
                }

                retries++;
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            return response;
        }
        public (byte[] Key, byte[] IV) GenerateAesKeys()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                return (aes.Key, aes.IV);
            }
        }
        public byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
        public string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            string plaintext;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}

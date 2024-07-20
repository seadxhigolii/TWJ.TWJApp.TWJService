using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Dto.Models
{
    public class TwitterOAuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    // Models/TweetRequest.cs
    public class TweetRequest
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class TwitterTokenRequest
    {
        public string GrantType { get; set; } = "authorization_code";
        public string Code { get; set; }
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string CodeVerifier { get; set; }
    }

    public class TwitterTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}

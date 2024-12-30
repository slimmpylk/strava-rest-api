using Newtonsoft.Json;

public class TokenResponse
{
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonProperty("expires_at")]
    public long ExpiresAt { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}
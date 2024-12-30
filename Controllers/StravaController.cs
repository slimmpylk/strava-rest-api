using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

[ApiController]
[Route("api/[controller]")]
public class StravaController : ControllerBase
{
    private readonly IConfiguration _configuration;

    // Constructor: Injects configuration to access app settings (e.g., ClientId, ClientSecret, RefreshToken).
    public StravaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("latest-workout")]
    public async Task<IActionResult> GetLatestWorkout()
    {
        // Fetch sensitive data (Client ID, Secret, and Refresh Token) from configuration settings.
        var clientId = _configuration["Strava:ClientId"];
        var clientSecret = _configuration["Strava:ClientSecret"];
        var refreshToken = _configuration["Strava:RefreshToken"];

        // Step 1: Obtain an Access Token from Strava's API.
        var client = new RestClient("https://www.strava.com/oauth/token");
        var request = new RestRequest
        {
            Method = Method.Post // HTTP POST request to exchange the refresh token for an access token.
        };

        // Add required parameters for Strava's OAuth token exchange process.
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("refresh_token", refreshToken);
        request.AddParameter("grant_type", "refresh_token");

        // Execute the request and attempt to deserialize the token response.
        var tokenResponse = await client.ExecuteAsync(request);
        var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse.Content);
        var accessToken = tokenData?.AccessToken;

        // Validate the token: If it's missing or empty, log an error and return a server error response.
        if (string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("Access token is null or empty.");
            return StatusCode(500, "Access token is null or empty.");
        }

        Console.WriteLine($"Access Token: {accessToken}");

        // Step 2: Fetch the user's latest workout data using the access token.
        client = new RestClient("https://www.strava.com/api/v3/athlete/activities");
        request = new RestRequest();
        request.Method = Method.Get;
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        
        request = new RestRequest
        {
            Method = Method.Get // HTTP GET request to retrieve workout data.
        };

        // Add the authorization header with the access token.
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        // Execute the request and attempt to deserialize the response into a list of workouts.
        var workoutResponse = await client.ExecuteAsync<List<Workout>>(request);

        // If the request fails, log the error and return a server error response.
        if (!workoutResponse.IsSuccessful)
        {
            Console.WriteLine("Error fetching workout data:");
            Console.WriteLine($"Status Code: {workoutResponse.StatusCode}");
            Console.WriteLine($"Response Content: {workoutResponse.Content}");
            return StatusCode(500, "Error fetching workout data.");
        }

        // Retrieve the first (most recent) workout from the list.
        var latestWorkout = workoutResponse.Data?.FirstOrDefault();

        // If no workouts are found, log the issue and return a 404 (Not Found) response.
        if (latestWorkout == null)
        {
            Console.WriteLine("No workouts found.");
            return NotFound("No workouts found.");
        }

        // Return the latest workout data as a successful response (HTTP 200).
        return Ok(latestWorkout);
    }

    // Class representing the structure of a workout as returned by Strava's API.
    public class Workout
    {
        [JsonProperty("name")] // Name of the workout (e.g., "Morning Run").
        public string? Name { get; set; }

        [JsonProperty("distance")] // Total distance of the workout in meters.
        public float? Distance { get; set; }

        [JsonProperty("moving_time")] // Total moving time of the workout in seconds.
        public float? MovingTime { get; set; }

        [JsonProperty("total_elevation_gain")] // Total elevation gain during the workout in meters.
        public float? TotalElevationGain { get; set; }

        [JsonProperty("type")] // Type of activity (e.g., "Run", "Ride").
        public string? Type { get; set; }
    }

    // Class representing the structure of the token response from Strava's OAuth token exchange.
    public class TokenResponse
    {
        [JsonProperty("access_token")] // The access token used to authenticate API requests.
        public string? AccessToken { get; set; }

        [JsonProperty("refresh_token")] // The refresh token used to obtain a new access token.
        public string? RefreshToken { get; set; }

        [JsonProperty("expires_at")] // Expiration time of the access token (UNIX timestamp).
        public long ExpiresAt { get; set; }

        [JsonProperty("expires_in")] // Time in seconds until the access token expires.
        public int ExpiresIn { get; set; }
    }
}

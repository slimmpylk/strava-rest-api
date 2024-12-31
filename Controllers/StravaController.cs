using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace StravaRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StravaController : ControllerBase
    {
        // We use IConfiguration to safely access application settings
        // (like ClientId, ClientSecret, RefreshToken) from appsettings.json or environment variables.
        private readonly IConfiguration _configuration;

        // Constructor: Injects the IConfiguration service so we can read config values.
        public StravaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint: Fetches the most recent workout from Strava.
        /// </summary>
        /// <returns>The most recent workout data in a friendly JSON format, or an error status if something fails.</returns>
        [HttpGet("latest-workout")]
        public async Task<IActionResult> GetLatestWorkout()
        {
            // 1. Read sensitive data (Client ID, Secret, Refresh Token) from configuration settings.
            var clientId = _configuration["Strava:ClientId"];
            var clientSecret = _configuration["Strava:ClientSecret"];
            var refreshToken = _configuration["Strava:RefreshToken"];

            // 2. Obtain a fresh Access Token from Strava using the OAuth refresh flow.
            //    - We create a new RestClient pointing to the token endpoint.
            var client = new RestClient("https://www.strava.com/oauth/token");
            //    - Use an empty string as the resource path, since the base URL already has the correct endpoint.
            var request = new RestRequest("", Method.Post);

            //    - Add parameters required by Strava for exchanging a refresh token for a new access token.
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("grant_type", "refresh_token");

            // 3. Execute the request and parse the token response.
            var tokenResponse = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(tokenResponse.Content))
            {
                // If the response is empty, we can't proceed—return an HTTP 500 (Internal Server Error).
                return StatusCode(500, "Failed to fetch access token.");
            }

            // 4. Deserialize the response JSON into our TokenResponse object.
            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse.Content);
            var accessToken = tokenData?.AccessToken;

            // 5. Validate that we actually got a non-empty access token.
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Access token is null or empty.");
                return StatusCode(500, "Access token is null or empty.");
            }

            // 6. Use the newly acquired access token to fetch the user’s most recent workout.
            //    - Create a new RestClient pointing to Strava’s athlete activities endpoint.
            client = new RestClient("https://www.strava.com/api/v3/athlete/activities");
            //    - Create a GET request (empty resource path again because the base URL already points where we need).
            request = new RestRequest("", Method.Get);

            //    - Add the Bearer token to the Authorization header.
            request.AddHeader("Authorization", $"Bearer {accessToken}");

            // 7. Execute the request to fetch the activities and parse the JSON response into a list of workouts.
            var workoutResponse = await client.ExecuteAsync(request);

            // 8. If we get an empty response, inform the caller that something failed.
            if (string.IsNullOrEmpty(workoutResponse.Content))
            {
                return StatusCode(500, "Error fetching workout data.");
            }

            // 9. Deserialize the JSON into a list of Workout objects.
            var workouts = JsonConvert.DeserializeObject<List<Workout>>(workoutResponse.Content);

            // 10. Extract the first workout from the list (which should be the latest one returned).
            var latestWorkout = workouts?.FirstOrDefault();

            // 11. If there are no workouts, return an HTTP 404 (Not Found).
            if (latestWorkout == null)
            {
                Console.WriteLine("No workouts found.");
                return NotFound("No workouts found.");
            }

            // 12. Format the workout data in a user-friendly way (e.g., converting meters to km, seconds to hh:mm).
            var formattedWorkout = new
            {
                Name = latestWorkout.Name,
                Distance = $"{(latestWorkout.Distance / 1000):F2} km", // Convert meters to kilometers.
                MovingTime = latestWorkout.MovingTime.HasValue
                    ? $"{TimeSpan.FromSeconds(latestWorkout.MovingTime.Value):hh\\:mm}" // Convert seconds to hh:mm format.
                    : null,
                TotalElevationGain = $"{latestWorkout.TotalElevationGain ?? 0} m", // Elevation in meters.
                Type = latestWorkout.Type,
                Date = latestWorkout.StartDate != null
                    ? DateTime.Parse(latestWorkout.StartDate).ToString("dd.MM.yy") // Format the date
                    : "Unknown"
            };

            // 13. Return the formatted workout as a 200 OK response.
            return Ok(formattedWorkout);
        }

        /// <summary>
        /// Endpoint: Fetches a summary of workouts for the last calendar week (Mon-Sun).
        /// </summary>
        /// <returns>A JSON object containing the total distance, total time, and total elevation for last week.</returns>
        [HttpGet("Last-Week-Summary")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            // 1. Read sensitive data from configuration (Client ID, Secret, Refresh Token).
            var clientId = _configuration["Strava:ClientId"];
            var clientSecret = _configuration["Strava:ClientSecret"];
            var refreshToken = _configuration["Strava:RefreshToken"];

            // 2. Obtain a fresh Access Token (same approach as in the GetLatestWorkout method).
            var client = new RestClient("https://www.strava.com/oauth/token");
            var request = new RestRequest("", Method.Post);

            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("grant_type", "refresh_token");

            var tokenResponse = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(tokenResponse.Content))
            {
                return StatusCode(500, "Failed to fetch access token.");
            }

            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse.Content);
            var accessToken = tokenData?.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Access token is null or empty.");
                return StatusCode(500, "Access token is null or empty.");
            }

            // 3. Calculate the start and end timestamps for the last calendar week (Mon-Sun in UTC).
            //    - Start of last week: We go back to the previous Monday in UTC (from the current day).
            //      If current day is Wednesday, subtract (DayOfWeek.Wednesday) plus 1 more day to get to Monday, etc.
            var startOfLastWeek = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek - 6).Date;
            //    - End of last week: Add 7 days to the start of last week, and subtract 1 second to get the last moment of Sunday.
            var endOfLastWeek = startOfLastWeek.AddDays(7).AddSeconds(-1);

            // 4. Fetch workouts within this date range using the 'after' and 'before' parameters (Unix time).
            client = new RestClient("https://www.strava.com/api/v3/athlete/activities");
            request = new RestRequest("", Method.Get);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            //    - The 'after' parameter means: only activities that started after this Unix time.
            //    - The 'before' parameter means: only activities that started before this Unix time.
            request.AddParameter("after", ((DateTimeOffset)startOfLastWeek).ToUnixTimeSeconds());
            request.AddParameter("before", ((DateTimeOffset)endOfLastWeek).ToUnixTimeSeconds());

            var workoutResponse = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(workoutResponse.Content))
            {
                return StatusCode(500, "Error fetching last week's workout data.");
            }

            // 5. Deserialize the JSON response into a list of Workout objects.
            var workouts = JsonConvert.DeserializeObject<List<Workout>>(workoutResponse.Content);
            if (workouts is null || !workouts.Any())
            {
                // If there are no workouts, we could return some kind of "empty week" summary or a 404.
                // Here we just default to 0.0 if it's empty. Adjust as needed.
                return Ok(new
                {
                    TotalDistance = "0.00 km",
                    TotalTime = "00:00",
                    TotalElevationGain = "0 m"
                });
            }

            // 6. Aggregate total distance, time, and elevation gain.
            //    - Convert total distance from meters to kilometers by dividing by 1000.
            var totalDistance = workouts.Sum(w => w.Distance ?? 0) / 1000;
            //    - Sum of all moving times (in seconds).
            var totalTime = workouts.Sum(w => w.MovingTime ?? 0);
            //    - Sum of all elevation gains (in meters).
            var totalElevation = workouts.Sum(w => w.TotalElevationGain ?? 0);

            // 7. Format the aggregate data for a user-friendly response.
            var formattedSummary = new
            {
                TotalDistance = $"{totalDistance:F2} km",
                // Convert total seconds to a TimeSpan and display hh:mm format.
                TotalTime = $"{TimeSpan.FromSeconds(totalTime):hh\\:mm}",
                TotalElevationGain = $"{totalElevation} m"
            };

            // 8. Return the summary as JSON with a 200 OK status.
            return Ok(formattedSummary);
        }

        // Represents a workout as returned by Strava’s API JSON.
        // We mark properties with JsonProperty attributes so that
        // Newtonsoft.Json can map the JSON fields to these class properties.
        public class Workout
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("distance")]
            public float? Distance { get; set; }

            [JsonProperty("moving_time")]
            public float? MovingTime { get; set; }

            [JsonProperty("total_elevation_gain")]
            public float? TotalElevationGain { get; set; }

            [JsonProperty("type")]
            public string? Type { get; set; }
            
            [JsonProperty("start_date")]
            public string? StartDate { get; set; } // Date and time when the workout started

        }

        // Represents the token response returned by Strava’s OAuth endpoint
        // when exchanging a refresh token for an access token.
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
    }
}

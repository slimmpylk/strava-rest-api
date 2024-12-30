var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // This registers the necessary services for controllers

var app = builder.Build();

// Map the root endpoint (for welcome message)
app.MapGet("/", () => "Welcome to the Strava REST API! Use /api/strava/latest-workout to fetch data.");

// Map the Strava Controller routes
app.MapControllers(); // Ensure this is called after registering the services

// Run the application
app.Run();
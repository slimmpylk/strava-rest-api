(not yet online)

Strava REST API Integration

This project provides an API to fetch the latest workout data from Strava, leveraging Strava's official API. It enables seamless integration with personal websites or applications to display recent workout information, such as distance, duration, and type of activity.
Features

    Fetches the latest workout details from Strava (distance, duration, type, etc.)
    Uses OAuth 2.0 authentication for secure access to the Strava API
    Returns the latest workout data in a clean, structured format

Setup Guide

Setting up the Strava REST API on your project requires a few steps. Follow these instructions to get started:

1. Create a Strava API Application

Before you can access Strava's workout data, you'll need to create an API application on Strava. Here's how you do that:

    Go to the Strava API Developer Page.
    Click Create & Manage your App.
    Fill out the application details. Make sure to include your domain or localhost URL for testing.
        Authorization Callback Domain: Use your deployed API URL or http://localhost:5000 for local testing.
    Once created, you will get your Client ID, Client Secret, and Refresh Token (you’ll need to authenticate once to get this token).

2. Configure Your Project

After setting up your Strava API credentials, you need to configure your project to use them:
a) Set Up Azure (If deploying to Azure)

    Create an Azure App Service to host your REST API.
        Set the runtime stack to .NET 8 and the operating system to Linux.
        Use the Free tier for testing (or choose a higher tier based on your needs).
    Configure Application Settings:
        In Azure, navigate to your web app, go to Configuration, and add the following application settings:
            Strava:ClientId: Your Strava Client ID.
            Strava:ClientSecret: Your Strava Client Secret.
            Strava:RefreshToken: Your Strava Refresh Token (this is a one-time process you can retrieve from Strava after authentication).

b) Configure Locally

If you're running the API locally, create a appsettings.json file in the root of your project:

{
"Strava": {
"ClientId": "your_strava_client_id",
"ClientSecret": "your_strava_client_secret",
"RefreshToken": "your_strava_refresh_token"
}
}

3. Running the Application Locally

To run the API locally:

    Ensure you have .NET SDK installed.
        You can check if it's installed by running dotnet --version.
        If not, install it from here.

    Open the terminal in the project folder and run:

    dotnet run

    Your Strava API should now be accessible on http://localhost:5000. You can test it by visiting:
        http://localhost:5000/api/strava/latest-workout

4. Deploying to Azure

Once your API is working locally, you can deploy it to Azure using the Azure Portal or via Git. Follow these steps:

    Create a Git repository for your project (if not already done).
    In the Azure Portal, go to Deployment Center in your App Service settings.
    Connect your repository (GitHub, Azure Repos, etc.).
    Push your project to your repository, and Azure will automatically deploy your API.

5. Using the API

Once everything is set up, you can access the API endpoint to retrieve the latest workout:

    Endpoint: GET /api/strava/latest-workout

This will return the most recent workout data, including:

    Name of the activity
    Distance (in meters)
    Moving Time (in seconds)
    Type (e.g., Run, Bike)

Example Request & Response
Request

GET https://your-api-url/api/strava/latest-workout

Response

{
"name": "Morning Run",
"distance": 10000.0,
"moving_time": 3600.0,
"total_elevation_gain": 150.0,
"type": "Run"
}

Next Steps

    Add user profile info: You can extend the API to include user profile data, such as weight, height, and bio.
    Fetch more activity details: The API can be modified to pull more details, like heart rate zones or segment performance.
    Create a front-end to display the data: Use React, Angular, or plain HTML to display the latest workout data on your website.

    Regarding the connection between the two repositories (SyntaxSiblings-DevHub and strava-rest-api), you can add a simple note in your README.md of SyntaxSiblings-DevHub to show the relationship. For example:

In the SyntaxSiblings-DevHub repository README:
Linked Projects

    Strava REST API: [SyntaxSiblings repo] (https://github.com/slimmpylk/SyntaxSiblings-DevHub)
        This repository contains the backend for integrating Strava workout data with web applications. It's connected to this project and used to display real-time workout statistics on the portfolio site.

License

Feel free to use this code for personal projects or contributions. Please make sure to keep the API keys secure.

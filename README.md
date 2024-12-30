(not yet online)

Strava REST API Integration

This project provides an API to fetch the latest workout data from Strava, leveraging Strava's official API. It's designed to allow seamless integration with personal websites or applications, making it easier to display recent workout information.
Features

    Fetches the latest workout details from Strava (distance, duration, type, etc.)
    Uses OAuth 2.0 authentication for secure access to the Strava API
    Returns the latest workout data in a clean, structured format

Setup

    Clone this repository.
    Set up your Strava API credentials (Client ID, Client Secret, Refresh Token).
    Add your credentials to the application settings in Azure or locally in appsettings.json.
    Deploy or run the application locally using dotnet run.

Routes

    GET /api/strava/latest-workout - Fetch the most recent workout data from Strava.

Next Steps

    Add additional functionality, such as fetching historical data or user profile details.
    Connect this API to a front-end to display workout data visually.

Regarding the connection between the two repositories (SyntaxSiblings-DevHub and strava-rest-api), you can add a simple note in your README.md of SyntaxSiblings-DevHub to show the relationship. For example:
In the SyntaxSiblings-DevHub repository README:
Linked Projects

    Strava REST API: [Link to the API repository](https://github.com/slimmpylk/SyntaxSiblings-DevHub)
        This repository contains the backend for integrating Strava workout data with web applications. It's connected to this project and used to display real-time workout statistics on the portfolio site.

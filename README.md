(READY FOR LAUNCH)

Demo from the Website
![image](https://github.com/user-attachments/assets/062f7b65-7ed5-4fe0-b1e2-34b1a21f6aab)


UML chart
![Flowchart](https://github.com/user-attachments/assets/fc4d2704-4a8b-4471-9f67-13ca374b07b3)



Strava REST API Integration (Overview)

This project lets you grab the latest workout data from Strava and make it easily available on your personal website or application—perfect for showcasing recent runs, rides, or other activities. You’ll see details like:

    Distance
    Duration
    Type of activity

Key Features

    Easy Access to Latest Workout
    Automatically pulls your most recent Strava workout details (like distance, time, type).

    Secure OAuth 2.0
    Uses Strava’s official OAuth flow, so your data stays protected.

    Clean JSON Response
    Retrieves and returns your workout data in a well-structured format for you to display however you want.

Development Environment

    Operating System: Arch Linux
    IDE for .NET (API): JetBrains Rider
    IDE for Front-End (React, etc.): WebStorm
    Version Control: GitKraken + Git Bash
    Hosting: Azure (Linux-based App Service with .NET 8)

    These tools aren’t mandatory; you can choose any setup you like. But if you want the exact environment used in this project, the above combination works seamlessly on Arch Linux.

Setup Guide

Below is the recommended setup path:
1. Create a Strava API Application

    Go to the Strava API Developer Page.
    Click Create & Manage your App.
    Enter your application details. You’ll need to specify a domain or localhost URL (for local testing).
        Authorization Callback Domain: Either your live hosted domain or http://localhost:5000 for local tests.
    After creating the app, Strava provides you with a Client ID, Client Secret, and Refresh Token.
        You’ll only see your Refresh Token after an initial authentication flow, so be sure to note it down.

2. Configure Your Project

You’ll need to tell your .NET API how to use your Strava credentials. You have two main options:
(a) Deploying to Azure

    Create an Azure App Service for your REST API.
        Select the .NET 8 runtime and Linux as the OS.
        The Free tier is fine for testing.
    Under Configuration → Application Settings in Azure, add the following keys:
        Strava:ClientId
        Strava:ClientSecret
        Strava:RefreshToken

(b) Running Locally

    In the root of your .NET project, create a file called appsettings.json with the following format:

    {
      "Strava": {
        "ClientId": "your_strava_client_id",
        "ClientSecret": "your_strava_client_secret",
        "RefreshToken": "your_strava_refresh_token"
      }
    }

    Make sure this file is not committed to a public repo, since it contains sensitive information.

3. Running the Application Locally

    Install .NET SDK (if you haven’t already). Check by running:

dotnet --version

From your project’s folder, run:

    dotnet run

    Once it’s up and running, visit http://localhost:5000/api/strava/latest-workout to see your latest workout data in JSON format.

4. Deploying to Azure

    Push your project to a Git repository (GitHub, Azure Repos, or your own private repo).
    In Azure Portal → Your App Service → Deployment Center, link it to your repo.
    When you push changes to your chosen branch, Azure will build and deploy automatically.

    Tip: If you prefer to deploy manually, you can do so via GitKraken or Git Bash, or even use the Azure CLI. The important part is making sure your environment variables are set in Azure so the API can connect to Strava.

5. Using the API

Endpoint: GET /api/strava/latest-workout

Example JSON Response:

{
  "name": "Morning Run",
  "distance": 10000.0,
  "moving_time": 3600.0,
  "total_elevation_gain": 150.0,
  "type": "Run"
}

This data can then be displayed on any webpage or frontend app (e.g., React or Angular).
Next Steps

    Show More Profile Info: Add Strava user data, such as profile stats or bio.
    Fetch More Activity Details: Pull heart rate, power data, or segment performances.
    Front-End Integration: Use React or Angular to create charts, graphs, or other UI elements for your workout metrics.

Linked Repositories

If you’re also working with the SyntaxSiblings-DevHub repository, you can cross-reference the two projects by adding a short note in the README.md, stating:

    Strava Integration
    This repository integrates with the Strava REST API Project to display real-time workout statistics on the portfolio site.

License & Credits

Feel free to use or adapt this code for personal projects. Just be sure to keep your Strava keys and tokens secure. Always remember:

    “With great power comes great responsibility”—especially when managing API credentials.

That’s it! You now have a fully working Strava REST API integration that can fetch and display the latest workout data. Enjoy hacking on Arch Linux with JetBrains Rider, WebStorm, and GitKraken, and let us know about the cool stuff you build!

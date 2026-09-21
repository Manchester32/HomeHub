# HomeHub API

This is the ASP.NET Core Web API used by the HomeHub Android app.

## What it does

- Registers users and hashes passwords.
- Logs users in.
- Returns the list of HomeHub services.
- Saves service requests in SQLite.
- Returns a user's requests.

- Saves Cash or Card payment choices after a job is completed.
- Loads and updates profile information.

## Run the API

1. Open `HomeHub.sln` in Visual Studio.
2. Run the **http** profile.
3. Swagger should open.
4. The API listens on port `5092`.
5. The SQLite database `homehub.db` is created automatically the first time the API runs.

For Android/BlueStacks testing, keep this API running while the mobile app is open.

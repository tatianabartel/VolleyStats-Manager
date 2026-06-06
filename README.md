# VolleyStats Manager

VolleyStats Manager is a desktop .NET MAUI application for managing and analysing volleyball player statistics. It is designed for coaches who need to track players, matches and in-game performance data.

The application supports user accounts, team management, match recording with set-by-set scores, player statistics per match, a leaderboard, and JSON import/export.

## Features

- user registration and login/logout,
- password stored as a hash (ASP.NET Core Identity `PasswordHasher`),
- multiple teams per user,
- add, edit, and delete players,
- add, edit, and delete matches (with volleyball set-score validation),
- recording player statistics per match (points, blocks, aces, serves, errors),
- player detail view with per-match statistics,
- team leaderboard with filtering by player, month, and year,
- match detail view with per-player statistics,
- import and export of players, matches and statistics using JSON files,
- input validation throughout (jersey numbers, set scores, volleyball rules),
- local persistent storage.

## Technologies

- C#
- .NET MAUI
- MVVM pattern
- CommunityToolkit.Mvvm
- Entity Framework Core
- SQLite
- ASP.NET Core Identity (password hashing)
- asynchronous programming with `async` / `await`

## Supported Platforms

The project is configured for:

- Windows (WinUI 3)

Target framework:

```text
net10.0-windows10.0.19041.0
```

## Storage

The application stores all data in a local SQLite database file (`volleystats.db`) managed via Entity Framework Core.

The database contains the following entities:

- `User` — application accounts
- `Team` — teams belonging to a user
- `Player` — players belonging to a team
- `Match` — matches played by a team, including individual set scores
- `MatchSet` — individual set results for a match
- `PlayerStats` — per-player statistics for a specific match

## Running the Application

### Requirements

- .NET SDK 10
- .NET MAUI workloads
- Windows 10 version 1903 or higher

### Restore packages

```bash
dotnet restore
```

### Build

```bash
dotnet build -f net10.0-windows10.0.19041.0
```

### Run

```bash
dotnet run -f net10.0-windows10.0.19041.0
```

The project can also be opened and run from Visual Studio or Rider.

## Usage

1. **Register** a new account with a username and password.
2. **Create a team** — you will be redirected automatically after registration.
3. **Add players** to your team via the Players section.
4. **Record matches** with set-by-set scores. Volleyball rules are enforced (sets to 25, tiebreak to 15, win by 2, one team must win exactly 3 sets).
5. **Add player statistics** for each match — points, blocks, aces, serves and errors.
6. **View the leaderboard** in the Team Statistics section, with optional filtering by player, month and year.
7. **Export** your data to a JSON file for backup or sharing.
8. **Import** previously exported data — duplicate records are detected and skipped automatically.

## Project Structure

```
VolleyStats Manager/
├── Models/
│   ├── DTO/              — data transfer objects used in the UI layer
│   │   └── Export/       — FullExportDto for JSON import/export
│   └── Services/         — business logic and database access
├── ViewModels/           — MVVM ViewModels for each page
├── Views/                — MAUI XAML pages and code-behind
└── MauiProgram.cs        — dependency injection setup

VolleyStatsManager.DAL/
├── Entities/             — EF Core entity classes
├── Enums/                — PlayerPosition enum
├── Migrations/           — EF Core migrations
└── VolleyDbContext.cs    — database context
```

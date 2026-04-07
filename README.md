# Virtual Counselor

A web app that helps students plan their academic path — browse courses, check degree requirements, and manage their schedule.

## What's in this repo

```
VirtualCounselor/        – ASP.NET Core Blazor app (main project)
VirtualCounselorTests/   – Unit tests
VirtualCounselorUI.sln   – Solution file
```

## Main features

- **Course browser** – Search and explore all available courses
- **Degree requirements** – View major/minor requirements and eligible course substitutions
- **Transcript** – Enter completed courses to track credits and GPA
- **Schedule builder** – Plan and manage your course schedule
- **Cart / Checkout** – Collect courses you intend to take

## Quick start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run locally

```bash
cd VirtualCounselor
dotnet run
```

The app will be available at `https://localhost:5001` (or `http://localhost:5000`).

### Run tests

```bash
cd VirtualCounselorTests
dotnet test
```

### Run with Docker

```bash
cd VirtualCounselor
docker compose up --build
```

The app is served via Nginx on ports `80` and `443`.

## Notes

- Course and degree data is scraped on startup; the first load may take a moment.
- Docker deployment includes Nginx as a reverse proxy and optional Cloudflare Tunnel support.

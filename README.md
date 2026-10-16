# GenZCoders Backend

ASP.NET Core backend for the GenZCoders learning platform. The repository contains the .NET solution and API implementation for course-round resources, learning materials, online meetings, and student assignments.

## Technology

- C# / ASP.NET Core
- .NET SDK (use the version pinned in `global.json`)
- JWT-based authentication
- Entity Framework Core for data access and migrations
- REST API endpoints

## Features documented in the project

- Course materials: list, create, update, and delete materials associated with a course round
- Online meetings: manage course-round meeting details
- Course-round student assignments
- Authenticated API access and role/instructor authorization checks
- Swagger/OpenAPI support for local API exploration

## Getting started

### Prerequisites

- The .NET SDK version specified by `global.json`
- The database service/provider configured by the application
- EF Core CLI tooling if you need to create or apply migrations

### Restore and build

```bash
git clone https://github.com/zeyadmohammeds/genzcoders-backend.git
cd genzcoders-backend
dotnet restore
dotnet build
```

Open `genzcoders.slnx` in a compatible IDE or inspect the project directory for the API startup project. Configure local settings before running it.

## Configuration and database

1. Review the application's configuration files and identify the required connection string and authentication settings.
2. Keep database passwords, JWT signing keys, API tokens, and production settings out of committed files.
3. Supply local values through user secrets or an ignored local environment/configuration file, and use your hosting provider's secret manager in production.
4. If the current model changes require a database migration, create and apply it from the directory containing the relevant EF Core project and configured DbContext.

Do not run migrations against a production database without reviewing the migration and confirming a backup/recovery plan.

## API documentation

When the API is running, use the Swagger/OpenAPI endpoint configured by the application to inspect available routes and schemas. The exact local URL and port depend on the launch settings.

## Security notes

- Treat JWTs and meeting credentials as secrets.
- Enforce authorization on every protected resource, not only in the frontend.
- Validate incoming identifiers and ownership before modifying course data.
- Avoid publishing real student records, access tokens, or production connection strings in issues, logs, screenshots, or examples.

## Project status

The repository includes implementation work for materials, meeting management, and course-round student assignment. Review the source and current database migrations before assuming all deployment steps are complete.

## Maintainer

**Zeyad Mohammed** — [GitHub](https://github.com/zeyadmohammeds) · [Portfolio](https://zeyadmohammeds.vercel.app/)

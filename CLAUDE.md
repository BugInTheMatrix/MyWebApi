# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

"NZ Walks" — a .NET 8 solution (`MyWebApi.sln`) with two projects:

- **MyWebApiProject** — ASP.NET Core Web API (Regions, Walks, Images, Auth). Swagger UI at `/swagger` in Development.
- **NZWalks.UI** — ASP.NET Core MVC frontend that consumes the API over HTTP via `IHttpClientFactory`. It has no project reference to the API and keeps its own copies of DTOs (`NZWalks.UI/Models/DTO`).

There are no test projects.

## Commands

```powershell
dotnet build MyWebApi.sln
dotnet run --project MyWebApiProject --launch-profile http   # API on http://localhost:5144
dotnet run --project NZWalks.UI                              # MVC UI
```

The UI hardcodes `http://localhost:5144/api/...` in `NZWalks.UI/Controllers/RegionsController.cs`, so the API must run on the `http` profile (or update those URLs).

### EF Core migrations

There are **two DbContexts**, so every `dotnet ef` command needs `--context`:

```powershell
dotnet ef migrations add "<Name>" --project MyWebApiProject --context NZWalksDbContext
dotnet ef database update --project MyWebApiProject --context NZWalksDbContext

dotnet ef migrations add "<Name>" --project MyWebApiProject --context NzWalksAuthDbContext --output-dir Migrations/NzWalksAuthDb
dotnet ef database update --project MyWebApiProject --context NzWalksAuthDbContext
```

## Architecture (MyWebApiProject)

- **Two SQL Server databases** (local, Trusted_Connection; see `appsettings.json`):
  - `NZWalksDbContext` → `NZWalksDb`: domain data (Regions, Walks, Difficulties, Images). Difficulties and Regions are seeded via `HasData` in `OnModelCreating`.
  - `NzWalksAuthDbContext` → `NZWalksAuthDb`: ASP.NET Identity (`IdentityDbContext`), with seeded roles `Reader` and `Writer`.
- **Request flow**: Controller → repository interface (`Repositories/I*Repository`, registered scoped in `Program.cs`) → EF Core. Controllers never touch DbContext directly. Domain models (`Models/Domain`) ↔ DTOs (`Models/DTO`) are mapped with AutoMapper in `Mappings/AutoMapperProfiles.cs`; add a mapping there when adding a DTO.
- **Walks querying**: `SQLWalkRepository.GetAllAsync` implements filtering (`filterOn=Name`), sorting (`Name`, `Length`/`LengthInKm`) and paging (`pageNumber`, `pageSize`) from query-string params. Walk queries eagerly `Include("Difficulty")` and `Include("Region")`.
- **Auth**: Identity Core + JWT bearer. `AuthController` (`/api/Auth/Register`, `/api/Auth/Login`) uses `UserManager`; `TokenRepository` issues tokens signed with `Jwt:Key/Issuer/Audience` from config. Login looks users up **by email** (Register sets Email = UserName) and only returns a token if the user has at least one role. Password rules are relaxed (min length 6). `RegionsController` actions require roles via `[Authorize(Roles = ...)]` (GETs: `Reader,Writer`; create/update/delete: `Writer`).
- **Validation**: `[ApiController]` plus a custom `[ValidateModel]` action filter (`CustomValidationFilter/ValidateModelAttribute.cs`).
- **Errors**: `Middlewares/ExecptionHandlerMiddleware.cs` (note spelling) catches unhandled exceptions, logs them with a GUID, and returns a generic 500 JSON body.
- **Logging**: Serilog to console and `Logs/NzWalks_log.txt` (configured in `Program.cs`, replaces default providers).
- **Image uploads**: `LocalImageRepository` writes files to `<ContentRoot>/Images`, saves an `Image` row with a public URL, and `Program.cs` serves that folder as static files at `/Images`.

ask me before executing any git command
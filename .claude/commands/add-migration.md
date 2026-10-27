---
description: Add an EF Core migration and update the database
argument-hint: <MigrationName> [domain|auth]
---

Add an EF Core migration named from `$ARGUMENTS` in MyWebApiProject.

- If the second argument is `auth`, use `--context NzWalksAuthDbContext --output-dir Migrations/NzWalksAuthDb`.
- Otherwise use `--context NZWalksDbContext`.

Run `dotnet ef migrations add`, show me the generated migration, then ask before running `dotnet ef database update`.

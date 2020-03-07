# **Sketch.Market**
----------

The Sketch Market contains an API and Database for the Sketch ecosystem.

*ASP.NET Core 2.1, C#, Sqlite/ PostgreSQL/ SQL Server*

----------

#### Run
----------

Command Prompt

1. Execute `dotset aspnetcore_environment=Development`
2. Execute `dotnet run`

IDE

1. Select `Sketch.Market.Api`
2. Click `Run`

#### Entity Framework Migrations
----------
1. Run `Windows Powershell ISE`
2. Open `~/src/Sketch.Market.Api/add-migration.ps1`
3. Navigate to `~/src/Sketch.Market.Api`
4. Execute Script
5. Enter your migration name and press `Enter`

#### Dependencies
----------
* Stack.Data
* Stack.Http
* Stack.Http.Identity
* Stack.Patterns.Service
* Stack.Patterns.Repository
* EntityFrameworkCore
* AutoMapper

#### Integrations
----------
* RocketChat
* Swagger
* RabbitMQ

# Creating database locally with ef
Create file `dbsettings.json` with database config. File strucute:
```{
  "ConnectionStrings": {
    "Relational": "<connection string>"
  }
}
```
## Relational database
To create relational part of data storage use 2 commands:
1. dotnet ef migrations add <Migration Name> --context RelationalDbContext

If successful, result should look like: 
> Entity Framework Core 2.1.0-rc1-32029 initialized 'RelationalDbContext' using provider 'Npgsql.EntityFrameworkCore.PostgreSQL' with options: MigrationsAssembly=Lightest.Api
2. dotnet ef migrations add Grant --context PersistedGrantDbContext
3. dotnet ef migrations add Config --context ConfigurationDbContext

Output should contain only info with created sql and `Done.` in the end.
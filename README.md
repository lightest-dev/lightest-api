[![Build Status](https://travis-ci.com/lightest-dev/lightest-api.svg?branch=master)](https://travis-ci.com/lightest-dev/lightest-api)

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

# Logging in to Lightest.Api
Api project is split into 2 parts: API itself and IdentityServer, which provides log in and register functionality.
Logging in requires 3 steps, all URLs are listed for development mode and should be changed in production.
Some of the provided values are currently hard-coded, but will be replaced with config files.

1. POST `https://localhost:5000/api/Account/Login` with following body in json format
```json
{
  "login": "string",
  "password": "string",
  "rememberMe": bool
}
```
2. POST `https://localhost:5000/connect/authorize` with body in URL encoded form format with following fields:
```
client_id=client
response_type=code id_token
redirect_uri=url
scope=openid profile api
nonce=int
```
If user is logged in, redirect will occur to redirect_uri, path will include code, which will be used in next step.

3. POST `https://localhost:5000/connect/token` with body in URL encoded form format with following fields:
```
client_id=client
client_secret=secret
grant_type=authorization_code
scope=openid profile api
redirect_uri=url
code={code from step 2.}
```
Access token returned can be used to work with API and should be included. Refresh token are not currently supported.

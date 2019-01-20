[![Build Status](https://travis-ci.com/lightest-dev/lightest-api.svg?branch=master)](https://travis-ci.com/lightest-dev/lightest-api)

# Logging in to Lightest.Api
Api project is split into 2 parts: API itself and IdentityServer, which provides log in and register functionality.
Logging in requires 3 steps, all URLs are listed for development mode and should be changed in production.
Some of the provided values are currently hard-coded, but will be replaced with config files.

1. POST `https://localhost:5200/Account/Login` with following body in json format
```json
{
  "login": "string",
  "password": "string",
  "rememberMe": bool
}
```
2. POST `https://localhost:5200/connect/authorize` with body in URL encoded form format with following fields:
```
client_id=client
grant_type = token
redirect_uri=http://localhost:4200
scope=api
nonce=123
state=qwe
response_type=token
```
If user is logged in, redirect will occur to redirect_uri, path will include access code.

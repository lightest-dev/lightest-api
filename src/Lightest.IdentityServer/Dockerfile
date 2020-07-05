FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

ARG vcs_ref
ARG build_date
ARG version

LABEL org.label-schema.maintainer="Andrii Vasylyk" \
    org.label-schema.url="https://github.com/lightest-dev/lightest-api" \
    org.label-schema.name="Lightest Identity server" \
    org.label-schema.license="Apache-2.0" \
    org.label-schema.version="$version" \
    org.label-schema.vcs-url="https://github.com/lightest-dev/lightest-api" \
    org.label-schema.vcs-ref="$vcs_ref" \
    org.label-schema.build-date="$build_date" \
    org.label-schema.schema-version="1.0"

WORKDIR /app
COPY artifacts/identity/* ./
EXPOSE 5200
ENTRYPOINT ["dotnet", "Lightest.IdentityServer.dll"]
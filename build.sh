set -e
set -v

dotnet build

# code coverage generation settings are very long,
# they are moved to custrom .rsp file
dotnet test @coverage.rsp

# Upload coverage data
codecov -f cover.xml

set +v

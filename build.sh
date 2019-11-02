set -e
set -v

dotnet build

# code coverage generation settings are very long,
# they are moved to custrom .rsp file
dotnet test @coverage.rsp

# Upload coverage data
curl -s https://codecov.io/bash > codecov
chmod +x codecov
./codecov -f "cover.xml" -t $CODECOV_TOKEN

set +v

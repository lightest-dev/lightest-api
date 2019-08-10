dotnet build
dotnet test /p:CollectCoverage=true /p:CoverletOutput=../../../cover.xml /p:CoverletOutputFormat=opencover
dotnet tool install coveralls.net --version 1.0.0 --tool-path tools
./tools/csmacnz.Coveralls --opencover -i cover.xml --useRelativePaths

dotnet build
dotnet test
dotnet tool install coveralls.net --version 1.0.0 --tool-path tools
.\tools\csmacnz.Coveralls 

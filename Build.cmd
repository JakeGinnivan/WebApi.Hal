@echo off

copy README.md WebApi.Hal\Readme.txt
.nuget\nuget.exe pack WebApi.Hal\WebApi.Hal.csproj -build -version 1.0.1.0
pause
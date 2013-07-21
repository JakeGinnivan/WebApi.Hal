@echo off

copy README.md WebApi.Hal\Readme.txt
.nuget\nuget.exe pack WebApi.Hal\WebApi.Hal.csproj -build -version 2.0.0.0
pause
@echo off

REM Check if Visual Studio is running
tasklist /FI "IMAGENAME eq devenv.exe" | find /I "devenv.exe" >nul
if %ERRORLEVEL%==0 (
    echo Visual Studio is currently running.
    echo Please close Visual Studio and then run this script again.
    pause
    exit /b
) else (
    echo Visual Studio is not running. Proceeding with the script...
)

REM Ask for the new version
set /p version="Enter the new version (e.g. 1.0.4): "

REM Navigate to the EasyFlow.Desktop project directory
cd EasyFlow.Desktop\

echo ===================================================
echo New version: %version%
echo ===================================================

REM Update the version in the EasyFlow.csproj file, ensuring UTF-8 encoding
powershell -Command "(Get-Content EasyFlow.Desktop.csproj -Encoding UTF8) -replace '<Version>.*<\/Version>', '<Version>%version%</Version>' | Set-Content EasyFlow.Desktop.csproj -Encoding UTF8"

echo Updated to %version%

REM Publish the project with the new version
dotnet publish -c Release --self-contained -r win-x64 -o ..\..\publish -f net8.0-windows10.0.17763.0

REM Go back to the initial directory
cd ..\..\

REM Pack with Velopack
vpk pack -u EasyFlow.Desktop -v %version% -p .\publish -e EasyFlow.Desktop.exe

REM Create a zip file of the Releases
set zipName=EasyFlow-%version%-win-x64.zip
powershell Compress-Archive -Path .\Releases\* -DestinationPath %zipName%

echo ===================================================
echo  NEW VERSION: %version%
echo ===================================================
echo .
REM Delete the 'publish' and 'Releases' folders
rmdir /s /q publish
rmdir /s /q Releases

echo Cleanup completed.
pause
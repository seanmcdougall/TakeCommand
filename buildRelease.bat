

rem @echo off

copy /Y "TakeCommand\bin\Release\TakeCommand.dll" "GameData\TakeCommand\Plugins"
copy /Y TakeCommand.version GameData\TakeCommand
copy /Y ..\MiniAVC.dll GameData\TakeCommand

set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

rem set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
) 
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)


type TakeCommand.version
set /p VERSION= "Enter version: "


copy /Y README.md GameData\TakeCommand
copy /Y LICENSE.md GameData\TakeCommand
 

set FILE="%RELEASEDIR%\TakeCommand-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData


@echo off
rem =======================================================================
rem Copyright (C) Microsoft Corporation.  All rights reserved.
rem  
rem THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
rem KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
rem IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
rem PARTICULAR PURPOSE.
rem ======================================================================

echo ------------------------------------------------------------------------
echo Generating Cassini strong name key ...
sn -q -k Cassini.snk
if errorlevel 1 goto problems

echo ------------------------------------------------------------------------
echo Compiling Cassini.dll ...
csc /nologo /t:library /keyfile:Cassini.snk /r:System.dll /r:System.Web.dll /out:Cassini.dll AssemblyInfo.cs ByteParser.cs ByteString.cs Connection.cs Host.cs Messages.cs Request.cs Server.cs
if errorlevel 1 goto problems

echo ------------------------------------------------------------------------
echo Removing Cassini strong name key ...
del Cassini.snk

echo ------------------------------------------------------------------------
echo Installing Cassini.dll into Global Assembly Cache ...
gacutil /i Cassini.dll /f /nologo
if errorlevel 1 goto problems

echo ------------------------------------------------------------------------
echo Compiling CassiniWebServer.exe application ...
csc /nologo /t:winexe /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:Cassini.dll /win32icon:CassiniWebServer.ico /res:CassiniWebServer.ico,CassiniWebServerIcon /out:CassiniWebServer.exe CassiniWebServerMain.cs
if errorlevel 1 goto problems

echo ------------------------------------------------------------------------
echo Build completed. 
echo     To run Cassini please specify physical path, port, and virtual path
echo         For example:
echo             CassiniWebServer c:\temp 8080 /temp


goto done

:problems
echo ---------------
echo Errors in build
echo ---------------

:done
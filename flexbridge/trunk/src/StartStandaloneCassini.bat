@echo off
REM This script starts the Cassini web server as a standalone application
REM to host the FlexBridge test web resources.
REM Normally you don't need to do this because the tests automatically
REM launch a embedded copy of Cassini.
setlocal
set SRC=%~dp0
"%SRC%..\libs\Cassini\bin\CassiniWebServer.exe" "%SRC%Castle.FlexBridge.Tests" 3700 /

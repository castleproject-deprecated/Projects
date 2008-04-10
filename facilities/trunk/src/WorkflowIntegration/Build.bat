@echo off
REM Builds everything and drops it in the bin\debug and build\net-2.0 folder.
%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe "%~dp0\WorkflowIntegration-vs2005.sln" /v:minimal /p:OutDir=..\bin\Debug\ %*
%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe "%~dp0\Castle.Facilities.WorkflowIntegration\Castle.Facilities.WorkflowIntegration-vs2005.csproj" /v:minimal /p:OutDir=..\build\net-2.0\ %*

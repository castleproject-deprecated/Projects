@echo off
REM Builds everything and drops it in the bin\debug and build\net-2.0 folder.
%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe "Castle.Facilities.OptionalPropertyInjection-vs2008.sln" /v:minimal /p:OutDir=..\bin\Debug\ %*
%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe "Castle.Facilities.OptionalPropertyInjection\Castle.Facilities.OptionalPropertyInjection.csproj" /v:minimal /p:OutDir=..\build\net-3.5\debug\ %*

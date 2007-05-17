Cassini Web Server Sample v2.0 README.TXT
------------------------------------------

--------------------------------------------------------------------------
Updated 3/6/2006:
Fixed memory leak of Connection objects
--------------------------------------------------------------------------

This Cassini version requires .NET Framework v2.0.

This sample illustrates using the v2.0 ASP.NET hosting APIs (System.Web.Hosting)
to create a simple managed Web Server with System.Net APIs.

New in Cassini v2.0:
* It uses new v2.0 ASP.NET hosting APIs (System.Web.Hosting.ApplicationManager)
* It performs socket listening from the default app domain (instead of
  worker app domain as in v1.0)
* It only binds to Loopback IP address, thus making localhost only restriction
  more robust
* It is very close (but not identical) to ASP.NET development web server that
  ships with Visual Studio 2005.


Package contents
----------------

Sources files for the Cassini.dll assembly that implements the web server functionality:
    AssemblyInfo.cs
    ByteParser.cs
    ByteString.cs
    Connection.cs
    Host.cs
    Messages.cs
    Request.cs
    Server.cs

Source files for the Cassini Web Server application:
    CassiniWebServerMain.cs
    CassiniWebServer.ico

Batch file to build the Cassini assembly and the server application:
    build.bat
    
Compiled binaries:
    Cassini.dll
    CassiniWebServer.exe


Instructions to Build Cassini
-----------------------------

TO BUILD THIS SAMPLE THE ENVIRONMENT MUST HAVE PATH SET TO BE ABLE TO RUN:
* SN.EXE
* GACUTIL.EXE
* CSC.EXE

Go to the directory containing Cassini source files, run 'build.bat' and
it will:
* compile Cassini.dll assembly
* add Cassini.dll assembly into Global Assembly Cache
* compile CassiniWebServer.exe web server application


Instructions to Run Cassini
---------------------------

1) Make sure Cassini.dll is installed into GAC
(build.bat does it automatically)

2) Run the EXE: 'CassiniWebServer <physical-path> <port> <virtual-path>'.
For example:
    CassiniWebServer c:\ 80 /

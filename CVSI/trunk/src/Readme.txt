+=================+
| How to use CVSI |
+=================+
Step 1 - Install VS SDK
-----------------------
- As I have not yet registered the Visual Studio integration package with Microsoft. To use it you will need to
  preinstall the Visual Studio 2005 SDK Version 4.0:
  
  http://www.microsoft.com/downloads/details.aspx?FamilyID=51A5C65B-C020-4E08-8AC0-3EB9C06996F4&displaylang=en

Step 2 - Build Castle.NVelocity
-------------------------------
- Open \Castle.NVelocity\trunk\src\Castle.NVelocity.sln
  in Visual Studio and build it.

Step 3 - Build Castle.VisualStudio.NVelocityLanguageService
-----------------------------------------------------------
- Open \CVSI\trunk\src\Castle.VisualStudio.NVelocityLanguageService\Castle.VisualStudio.NVelocityLanguageService.sln
  in Visual Studio.
- Go into the project properties under Debug set these values (without the quotes):
  Set "Start external program" to "C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\devenv.exe"
  Set "Command line arguments" to "/rootsuffix Exp"
- Run the solution without debugging.

You will now be able to use the experimental hive of Visual Studio to open *.vm and *.njs files.

+=================+
|      Notes      |
+=================+
- You can start the Visual Studio experimental hive copy at anytime via the
  "Start Visual Studio 2005 under Experimental hive" menu item in the start menu.

+=================+
|  Known Issues   |
+=================+
- You cannot have multiple files open at once because they each overwrite each other scanner state information.

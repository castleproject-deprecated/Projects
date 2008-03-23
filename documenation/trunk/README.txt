This package contains the DocBook source, tools, and build scripts required to generate the documentation for Castle frameworks.

The documentation for each framework can be built by executing its build script (which can be found in the same folder as this file) with NAnt.  The build target will generate the documentation in a subfolder of the "build" folder, which will also be created in the same folder as this file.

To build all documentation for all Castle frameworks and tools simple execute default.build with NAnt.  This script will execute all other build scripts in the same folder to generate the documentation.

A Java runtime must be installed to execute the transformations, or alternately IKVM.NET can be used by changing the NAnt tasks running Java to instead run ikvm.exe.

To build the CHM documentation you will also need to install the hhc.exe HTML Help compiler.

Other tools required to build the documentation are available in the tools directory under this folder.

NOTE: At the moment the output will not appear in the build folder in the event that there are any spaces in the path.  Hopefully this will be addressed shortly.
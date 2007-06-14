Castle.FlexBridge
by Jeff Brown.

>> http://using.castleproject.org/display/Contrib/Castle.FlexBridge


Overview
========

The Castle FlexBridge project offers remoting and data services interoperation
with Adobe Flex (tm).  It should not be confused with Adobe's Flex (tm)-Ajax Bridge
project or other official Adobe products.

The project aims to provide a lightweight, fast, robust and well-tested Open Source implementation
of Adobe Flex (tm) remoting, data and media services for .Net.  It also includes powerful client-side
components such as an Inversion of Control container with Constructor Dependency Injection similar
to Castle MicroKernel.  The client and server components can be used together or independently as you wish.

At this time, there are several other third party solutions available with some or
all of these features.  This project aims to differentiate itself by achieving a higher
standard of quality.

The implementation should make efficient use of resources, scale out to handle heavy
concurrent load, be robust in the face of errors, and yield consistent and correct
results.  Furthermore, the API and internal architecture should leverage .Net
idioms and features to ease integration, and provide many opportunities for extension
and customization.

It is not the purpose of this project to provide a standalone and fully-featured
"media server" component in the same manner as Adobe Flex Data Services (tm),
Adobe Media Server (tm), WebOrb (tm) and similar products.  Rather it is intended to
provide components that can be seamlessly integrated into existing applications
to solve specific concerns.  While a "media server" could be constructed using these
components, it is beyond the scope of the project to provide one.


Documentation
=============

The API contains copious XML documentation regarding the usage of each component.

For more in-depth documentation, installation instructions, and tutorial matter,
please refer to the Wiki page below:

http://using.castleproject.org/display/Contrib/Castle.FlexBridge


Thanks
======

Many thanks to the contributors of the OS Flash project for reverse-engineering and
publishing the AMF protocol.  Also thanks to the implementors of the many other
Open Source Flex Remoting libraries which provided some good reference code and ideas.

The protocol data at http://osflash.org/documentation/amf3 and http://osflash.org/documentation/amf
were valuable resources.

The RemoteObjectAMF0 adapter for AMF0 based RemoteObject access used for integration
testing was written by Renaun Erickson and is available at
http://renaun.com/blog/flex-components/remoteobjectamf0/

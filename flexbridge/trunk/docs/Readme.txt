Castle.FlexBridge
by Jeff Brown.


Overview
========

The Castle.FlexBridge project offers remoting and data services interoperation
with Adobe Flex (tm).  It should not be confused with Adobe's Flex (tm)-Ajax Bridge
project or other official Adobe products.

The project aims to provide a lightweight, fast, robust and well-tested Open Source
implementation of Adobe Flex (tm) remoting, data and media services for .Net.

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



Motivation
==========

I wrote this library after realizing how painfully slow and CPU-intensive SOAP
remoting can be when a lot of data needs to be pushed around.  My application
was spending a huge amount of time deserializing SOAP responses during which it
was unresponsive to the user.  Moreover, I wanted to be able to push data to the
client as it became available rather than constantly polling for it.

I looked at existing solutions to these problems but none satisfied.  I didn't
trust the implementations and at the time none of them provided strong support
for .Net 2.0 features.  I also wanted drop-in integration with Castle core
services such as logging because my application was already using Windsor everywhere.

It didn't seem that hard and I had a little free time...


Thanks
======

Many thanks to the contributors of the OS Flash project for reverse-engineering and
publishing the AMF protocol.  Also thanks to the implementors of the many other
Open Source Flex Remoting libraries which provided some good reference code and ideas.

The protocol data at http://osflash.org/documentation/amf3 and http://osflash.org/documentation/amf
was an invaluable resource.

The RemoteObjectAMF0 adapter for AMF0 based RemoteObject access used for integration
testing was written by Renaun Erickson and is available at
http://renaun.com/blog/flex-components/remoteobjectamf0/


Design Philosophy
=================

0. Test everything thoroughly.  Write unit tests _and_ integration tests!

   This one speaks for itself.

1. Don't allocate unnecessary objects.

   Some AMF implementations create lots of Arrays, Lists or Dictionaries that they just
   don't need or that usually remain empty.  Use flyweights like EmptyArray and
   EmptyDictionary to capture common cases.  For other circumstances, allow the use of
   interface types so that the client is not locked into a particular IList
   or IDictionary representation, particularly if the client has a perfectly good object
   already at hand so there's no sense forcing its contents to be copied.
   
   For flyweights, see EmptyArray, EmptyDictionary and ASClass.UntypedDynamicClass.
   For interface types, see ASObject, ASArray and others.
   
2. Don't mandate object representation.  

   The user might have very definite ideas about how an object should be represented.
   As long as an object can satisfy a minimal interface for serialization purposes,
   allow it to be used.  Don't unnecessarily restrict the layout of objects.  In particular,
   this makes it easy to define very efficient adapters for various types.
   
   See IActionScriptObject, IActionScriptByteArray, and others.

3. Keep the core serialization code simple.

   The core serialization code is already complicated by binary data representation
   and I/O concerns, don't push any more behavior into it than strictly necessary.
   In particular, don't force the serialization code to know about all sorts of special
   object mapping concerns like how a List might be coerced into an array.  It is
   possible for the serialization code to be completely isolated from such concerns if
   we don't mandate object representation.  (see #2)

   See how AMF0ObjectWriter and AMF3ObjectWriter delegate data mapping concerns to
   IASValue and the IActionScriptSerializer.  The visitor pattern is put to good use
   here to eliminate run-time type checks and to abstract the internal representation
   of the IASValue instances from their consumers.

4. Immutable types make things easier.

   ASClass and other built-in ActionScript wrapper types are deliberately
   designed to restrict the kinds of changes that can be made to the object after it
   has been constructed.  This makes it possible for us to perform stricter checks
   on object invariants to catch any mistakes sooner.  Moreover, it enables us
   to make stronger assumptions about object state.  For instance, we can use
   ASClass.UntypedDynamicClass as a flyweight only because ASClass is immutable.
   
   See ASClass, ASObject, and others.

5. Don't skimp on runtime assertions.

   The serialization code performs extra checks to ensure that the layout of the
   objects being serialized is sane.  These tests are quite cheap to perform and
   pay off big time in bug-catching potential.

6. If your feature is really cool, can you think of a way it might be implemented
   as an add-on extension?
   
   Built-in magic is usually great.  Most clients appreciate being able to do lots of cool
   things out of the box.  However, it's even better if the clients can also get in
   on the show to do a little prestidigitation of their own or to tweak how the
   built-in magic works a little bit.
   
   Usually the best policy is to ensure that you provide consistent interfaces
   to support all of the magic your framework provides.  Don't do things behind the
   scenes where the client cannot control them.  Providing these interfaces makes
   the architecture cleaner and opens up many avenues for extension.  It's also a
   good idea to provide a means to opt-out of the magic someplace.

   Notice how providing built-in infrastructure for mapping between ActionScript and
   native types pays off by making it easy to extend the framework with new custom
   mappings of all sorts.  It simplifies the framework and even enables a performance
   optimization because mappers can be looked up in a hashtable based on the properties
   of the objects to be mapped rather than by evaluating a long sequence of runtime type
   tests tucked away in some dark corner.

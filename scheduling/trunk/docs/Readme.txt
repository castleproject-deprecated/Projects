Castle.Components.Scheduler
by Jeff Brown.


Overview
========

The Castle.Components.Scheduler project offers a lightweight and reusable
general-purpose scheduling service that integrates well with most .Net applications.
It is similar in purpose to the Java Quartz job scheduling framework but
its implementation aims to leverage .Net idioms whenever possible.

The Scheduler service is intentionally kept relatively simple so that multiple
implementations may be offered and so that the operation of the service can be
easily mocked for testing.  The default implementation should suffice
for most purposes.


Motivation
==========

I wrote this library to solve a recurring problem (pardon the pun) that had
not been satisfactorily addressed in several applications I have worked on.
Robust job scheduling is such a common need and many tools are available.

Unfortunately at the time I wrote this, I had a specific business need for 
lightweight in-process scheduling with clustering for .Net in a fully open source
and redistributable form.  The problem was that I couldn't find one.
Regrettably, Quartz.Net did not yet support the features I needed.
So I wrote this simple one instead...


Thanks
======

Many thanks to the developers of the Quartz and Quartz.Net projects.
I have borrowed certain design ideas from these projects and found them to be
most excellent.  With any luck I haven't thoroughly corrupted their intent!

I looking forward to plugging Quartz.Net into this scheduling service someday!

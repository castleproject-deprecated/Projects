Usage:

SolutionTransform <Script File Name> <Solution Full Path>

e.g.
SolutionTransform CastleSilverlight.boo "C:\OSS3\Castle Core\src\Core-vs2008.sln"

This creates a silverlight version of the original trunk project (at least on my machine)

e.g.

SolutionTransform CastleStandardize.boo C:\Projects\scratch\Reference35\SolutionTransform\SolutionTransform.sln

This was used to ensure compliance with Castle coding standards when the code was originally checked in.

BUGS
====

# 21 Dec 2009

The only known problem at the moment is the rewriting of DLLs e.g. NUnit to NUnitLite.  The underlying problem is that 
the Castle Projects don't have valid lib directories at the moment.  This will be fixed when it's possible to do so.
Castle.Tools.StaticMapGenerator
===============================

* What?
This tool generates a strongly typed data structure that represents static resources in a website (javascripts, styles and images)

* How?
Just run Castle.Tools.StaticMapGenerator.exe in the site's root or bin folder

* command line options:
/site:       point to the site's root on the local disk. 
             default - current directory, or it's parent should the 
             current directory being a site's bin folder

/ignore:     Directories to ignore while searching, separated by '|'
             default: ".svn"|"bin"|"obj"

/namespace:  The namespace for the generated classes.
             default: StaticSiteMap

* Sample usage:
Castle.Tools.StaticMapGenerator.exe /site:C:\mysite /namespace:MySite.StaticSiteMap




patches would be welcome

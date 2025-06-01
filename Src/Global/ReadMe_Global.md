# Important

This project must not have any references to any other project in this solution!  It is accessed by most of the other projects and thus cannot have a circular dependency.

Also great care should be taken to not reference packages unnecessarily as ALL packages must be compatible with Blazor Client apps that run in the browser.  The package will be downloaded to client and thus adds to initial load times.
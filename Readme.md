IrcD.Net
========

Fully Functional Object Oriented Implementation of IrcD Server.

[![Build Status](https://travis-ci.org/FreeApophis/ircddotnet.svg?branch=master)](https://travis-ci.org/FreeApophis/ircddotnet)
[![NuGet package](https://buildstats.info/nuget/IrcD.Net)](https://www.nuget.org/packages/IrcD.Net)

Functionality
-------------

* Object Oriented Design, very flexible
* Flexible User and Channel Modes
* Fully RFC Compliant (this breaks some Clients)
* Client Compatibility Modes
* Can translate channels on the fly to the users language via google-translate (Channel Mode +T)

Limitations
-------------

* Single IRC Server only
* No Services
* No flood protection
* No channel protection
* Very basic access control


Development
===========

Compile on Windows
------------------

* Open VS2010 solution
* If you dont have nunit installed, Unload the Test Project

Compile On Linux
---------------

* Use a Mono 2.10+
* Run xbuild on the solution file
* IF you don't have nunit installed, remove project from the Solution


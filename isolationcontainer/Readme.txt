Daemoniq: Windows service hosting for mere mortals 

What is it?
------------
 Daemoniq is a windows service hosting tool for .Net applications. It provides a
 higher level of abstraction around the classes provided in the System.ServiceProcess 
 namespace. 

Why Daemoniq?
--------------
 Because Developing and debugging windows services or daemon processes in .Net can be a
 really painful and tedious process. The higher level of abstraction provided by Daemoniq 
 allows developers to concentrate on writing windows services in .Net by providing functions
 like configuration, installation and debuggability. 

The Latest Version
-------------------
  Details of the latest version can be found on the Daemoniq project web site 
  http://code.google.com/p/daemoniq/

Compilation and Installation
-----------------------------

   a. Build Requirements
   ---------------------
   To build Daemoniq, you will need the following components:
   on Windows

       * A version of the Microsoft .NET Framework
         Available from http://msdn.microsoft.com/netframework/
         
         You will need the .NET Framework SDK as well as the runtime components 
		 if you intend to compile programs.

         Note that Daemoniq currently supports versions 3.5 of the Microsoft .NET Framework. 
	 
   b. Building the Software
   ------------------------
   Build Daemoniq using Microsoft .Net
		
		* The build scripts are located in the build/scripts/ directory. There you will find
		  several batch files and a nant.build file. Execute full-build.bat to fully compile
		  Daemoniq.

  These instructions only apply to the source distribution of Daemoniq, as the binary distribution 
  contains pre-built assemblies.

Documentation
-------------
 Documentation is available in HTML format, in the doc/ directory.

License
-------
 Copyright 2009 Kriztian Jake Sta. Teresa
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 
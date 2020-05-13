# IncVersion
![version](https://img.shields.io/badge/version-1.2-blue)

Increments the Build part of the `<FileVersion>` property in the Visual Studio Project File automatically after every build. 
It can be used in Visual Studio 2019 projects on Sdk-style project files.

[Download IncVersion](https://github.com/FolkerKinzel/IncVersion/tree/master/binaries/IncVersion.zip)

IMPORTANT: On some systems, the content of the ZIP file is blocked. Before extracting it, right click on the ZIP file, select Properties, and click on the Unblock button - if it is present - in the lower right corner of the General tab in the Properties dialog.

## How it works

After every build of your project, IncVersion increments the build part of the current `<FileVersion>` property in your .csproj-file. If it didn't have
a `<FileVersion>`-Property before the build, IncVersion will add `<FileVersion>1.0.0.0</FileVersion>`.

You might wonder, why IncVersion does not change the csproj-File *before* Visual Studio compiles the output? Unfortunately, it is impossible, 
because Visual Studio reads the `<FileVersion>` from the project file before it executes any Build Events. In order to clarify this behavior, 
I decided for myself to prefer the Post Build Event rather than the Pre Build Event.

Why doesn't IncVersion increment the Assembly Version or the Nuget Package Version? That's why only yourself knows, if the current build is an 
important or breaking change to your application or not. Different version numbers describe different aspects and they don't need to be identical.



## Set up IncVersion

#### 1. Make sure to have the .NetCore 3.1 runtime installed on your computer

IncVersion is a very small platform-independent .NetCore-3.1-dll with no runtime components included. That's a design choice, because - following my
approach - IncVersion is copied to every project directory. Therefore it's necessary to have the runtime installed global.



#### 2. Download the zip-File with IncVersion and unzip it to the solution directory of your project

Download IncVersion [here](https://github.com/FolkerKinzel/IncVersion/tree/master/binaries/IncVersion.zip) and unblock it, if your computer blocks it. (See description above.)

Unzip it to the directory, where the solution file (*.sln) of your project is. ("Right-click and select "Unzip all".) The solution directory now should look like this:

<ul type="none">
<li>&lt; YourSolutionDirectory</>
<li>
   <ul type="none">
       <li>&#9633; YourSolution.sln</li>
       <li>&lt; IncVersion</li>
       <li>
          <ul type="none">
             <li>&#9633; IncVersion.dll</li>
             <li>&#9633; IncVersion.runtimeconfig.json</li>
             <li>&#9633; README.txt</li>
         </ul>
      </li>
      <li>&lt; YourProjectDirectory</li>
      <li>
          <ul type="none">
             <li>&#9633; YourProject.csproj</li>
         </ul>
      </li>
  </ul
</li>
</ul>

**Notice:** You have to unzip IncVersion to every solution directory, where your project is referenced - 
otherwise the Post Build Event will fail (resulting in a build error), if you try to build your project 
from there!

#### 3. Give your project a Post Build Event to invoke IncVersion
To invoke IncVersion, you have to add a Post Build Event to every project that you want IncVersion to increment the `FileVersion` property. Choose only
projects, that are physically located in the same project directory, where you unzipped IncVersion in the previous step.

To add a Post Build Event, right-click the project in Visual Studio Solution Explorer and select "Properties". In the project properties window select "Build Events".


Paste the following code to the textbox "Post Build Event":
> if $(ConfigurationName) == Release if '$(TargetFramework)' == 'netcoreapp3.1' dotnet $(SolutionDir)IncVersion\IncVersion.dll $(ProjectDir)$(ProjectFileName)

This Post Build Event **MUST NOT** contain any line-breaks!

If you have an earlier Post Build Event, make a line-break at the end of the earlier Post Build Event and paste the code to the new line.

**Replace `netcoreapp3.1` with one of your build targets**, if NetCore 3.1 is not among them. To find the correct naming of your build target, 
open your project file in the text editor: Your build target is the content of the property `TargetFramework`. If it's a multitargeting project,
the build targets belong to `TargetFrameworks`: Choose one!

*The post build event explained:*
* _`if $(ConfigurationName) == Release` causes IncVersion to be invoked only if you build the Release-Configuration. You may remove this, if you like
               - in addition - also to count the debug builds._
* _`if '$(TargetFramework)' == 'netcoreapp3.1'` causes IncVersion to be called only ones per build - even if you have more than one build target.
  If you have only one build target (and plan never to have a second), you can remove this._


## FAQ

#### Can I assign the file version independently when using IncVersion?

Of course, you can: Because of that IncVersion changes the project file after the build, the current build takes the file version number, that you
have entered in the project file immediately before the build.

It becomes a little bit tricky, if you use the "Publish" function of Visual Studio. For a workaround, read the next question.

#### How to get a FileVersion-Build-Part with value 0 when publishing a .NetCore app?

Visual Studio calls the Build Events twice, if you right-click to your project in Visual Studio Solution Explorer and choose "Publish". Therefore
you have to set a file version number with only two parts before you publish.

For instance: Setting FileVersion to 1.3 before publishing, produces assemblies
with a FileVersionAttribute, that has the value 1.3.0.0 .


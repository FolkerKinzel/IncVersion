# IncVersion

Increments the Build-Part of the current `<FileVersion>`-property in the Visual Studio Project File automatically. 
It can be used in Visual Studio 2019 projects with Sdk-style project files.



## How it works

After every build of your project, IncVersion increments the build part of the current `<FileVersion>`-Property in your .csproj-file. If you didn't had 
a `<FileVersion>`-Property before the build, IncVersion will add `<FileVersion>1.0.0.0</FileVersion>` to the project file.

You might wonder, why IncVersion does not change the csproj-File *before* Visual Studio compiles the output? Unfortunately, it is impossible, 
because Visual Studio reads the `<FileVersion>` from the project file before it executes any Build Events. In order to clarify this behavior, 
I decided for myself to prefer the Postbuild Event rather than the Prebuild Event.

Why doesn't IncVersion increment the Assembly Version or the Nuget Package Version? That's why only yourself knows, if the current build is an 
important or breaking change to your application or not. Different version numbers describe different aspects and they don't need to be identical.



## Setup IncVersion

#### 1. Make sure to have the .NetCore-3.1-Runtime installed on your computer

IncVersion is a very small platform independent .NetCore-3.1-dll with no runtime components included. That's a design choice, because - following my
approach - IncVersion is copied to every project directory. Therefore it's necessary to have the runtime installed global.



#### 2. Download the zip-File with IncVersion and unzip it to the solution directory of your project

The solution directory now should look like this:

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
             <li>&#9633; README.md</li>
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

#### 3. Give your project a Postbuild Event to invoke IncVersion
To invoke IncVersion you have to add a Postbuild-Event to every project that you want IncVersion to increment the `FileVersion` property. Choose only
projects, that are physically located in the same project directory where you unzipped IncVersion in the previous step.

To add a Postbuild-Event, right-click the project in Visual Studio Solution Explorer and choose "Properties". In the Project-Properties select "Build-Events".


Paste the following code to the textbox "Postbuild-Event":
> if $(ConfigurationName) == Release if '$(TargetFramework)' == 'netcoreapp3.1' dotnet $(SolutionDir)IncVersion\IncVersion.dll $(ProjectDir)$(ProjectFileName)

This Postbuild-Event MUST NOT contain any line-breaks!

If you have an earlier postbuild event, make a line-break at the end of the earlier postbuild event and paste the code to the new line.

**Replace `netcoreapp3.1` with one of your build targets**, if NetCore 3.1 is not among them. To find the correct naming of your build target, 
open your project file in the text-editor: Your build target is the content of the property `TargetFramework`. If it's a multitargeting project,
the build targets are under `TargetFrameworks`: Choose one!

*The post build event explained:*
* _`if $(ConfigurationName) == Release` makes, that IncVersion is only invoked, when you build the Release-Configuration. You can remove this, if you
               - in addition - like to count also the Debug-builds._
* _`if '$(TargetFramework)' == 'netcoreapp3.1'` makes, that IncVersion is called only ones per build - even if you have more than one build target.
  If you have only one build target (and plan never to have a second), you can remove this._


## FAQ

#### How to get a FileVersion-Build-Part with value 0 when publishing a .NetCore app?

If you right-click to your project in Visual Studio Solution Explorer and choose "Publish", Visual Studio calls the Build Events twice. Therefore
you have to set a fileversion number with only two parts before you publish.

For instance: Setting FileVersion to 1.3 before publishing produces Assemblies
with a FileVersionAttribute, that has the value 1.3.0.0 .


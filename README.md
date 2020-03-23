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

## How to get this work
#### 1. Make sure to have the .NetCore-3.1-Runtime installed on your computer

#### 2. Download the zip-File with IncVersion and unzip it to the solution directory of your project
The solution directory now should look like this:

<ul type="none" style="color:red;">
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




## FAQ
#### How to get a Build-Part with value 0 when publishing a .NetCore app?
If you right-click to your project in Visual Studio Solution Explorer and choose "Publish", Visual Studio calls the Build Events twice. Therefore
you have to set a fileversion number with only two parts before you publish.

For instance: Setting FileVersion to 1.3 before publishing produces Assemblies
with a FileVersionAttribute, that has the value 1.3.0.0 .


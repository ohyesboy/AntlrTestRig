This project aimed to mimic the official ANTLR4 TestRig which looks like this:

![](readmeDocs/hello-parrt.png)



## Features 

* All features that official TestRig provides: zoom, save as PNG, tree view, error highlight

![UI2](readmeDocs/UI2.png)

![UI1](readmeDocs/UI1.png)

* It also enhanced some parts that official TestRig does not have:

  * Auto expand to 5 levels in treeview
  * Use different color for terminal nodes
  * Watch input file change and reload
  * Watch dll file change and reload
  * Show rule index and token type (with -ruleindex option)


## TODO:

* The way UI shows problem token is kind of different than official one.
* Post script(-ps option) not supported



## Usage

* First make sure you setup your machine the official way, following [the article on ANTLR github](https://github.com/antlr/antlr4/blob/master/doc/getting-started.md) , that means you have java runtime installed, antlr jar file in the class path,  **antlr4** shortcut ready, etc..

* Copy the grammar file under the execution folder of this project, for example, `tsql.g4` from [here](https://github.com/antlr/grammars-v4/blob/master/tsql/tsql.g4)

* `antlr4 -Dlanguage=CSharp tsql.g4` If this goes well, you will see some parser and lexer cs files generated, like `tsqlParser.cs`

* Compile the cs:
  `csc /target:library /out:tsql.dll  /reference:Antlr4.Runtime.Standard.dll tsql*.cs`
  If this step goes well, you should see `tsql.dll` compiled by `csc`

  > If your g4 file is not in same directory as the exe, you can copy `Antlr4.Runtime.Standard.dll` to the  g4 file's directory

* Make input.sql file in the g4 file's folder, type some sql in it.

* Call this program: `%PATH TO AntlrTestRig.exe% tsql tsql_file input.sql -gui -tokens ` from the g4 files folder.

  > If you are not calling from the g4 file folder, use -folder option to specify the g4 dll folder.

  This will scan all dll files under the folder to find tsqlParser type to parse your input file from the start rule you specified, in this case, tsql_file rule.

  ​


> Note you are encouraged to create shortcut to make less typing, I have one like this in **acs.bat** (stands for ANTLR+csharp) in the %PATH%
>
> ```
> @call antlr4 -Dlanguage=CSharp %1.g4
> @if %errorlevel% neq 0 exit /b %errorlevel%
>
> @call csc /target:library /out:%1.dll  /reference:Antlr4.Runtime.Standard.dll %1*.cs
> @if %errorlevel% neq 0 exit /b %errorlevel%
> ```
>
> So I can call `acs tsql ` to generate C# and compile with 1 command

> If you see compile issues from the generated code, it is usually because either of the 2 reasons:
>
> - The antlr4 you setup has different version than the one the runtime dll is (v4.6 now), ANTLR's API may change, keep the version the same. 
> - The g4 file may contains none C# code injection. 





## Workflow with Notepad++

Since the TestRig can watch input and dll changes and reload without restart, it is convenient to work side by side with an editor with some shortcuts. I use Notepad++'s NppExec plugin to setup Ctrl+D to call `acs.bat $(NAME_PART)` (as I the setup above) 

![npp](readmeDocs/npp.png)

The highlight for g4 comes from [https://github.com/B1naryStudio/antlr-npp](https://github.com/B1naryStudio/antlr-npp)
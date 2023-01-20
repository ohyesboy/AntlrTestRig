@echo Restore the packages first (by building project), then run this. C# compiler Csc.exe should be in your path.

csc /target:library /out:demo.dll /reference:.\..\packages\Antlr4.Runtime.Standard.4.11.1\lib\\net45\Antlr4.Runtime.Standard.dll demo*.cs
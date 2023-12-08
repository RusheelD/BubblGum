@echo off

REM type ./commands [arg]. A command is run based off arg.

IF "%1"=="gen" (
    REM compiles and runs scanner + parser
    antlr4 -o ./AntlrCSharp/Parser ./AntlrCSharp/Parser/BubblGum.g4 -Dlanguage=CSharp
) ELSE IF "%1"=="run" (
    REM compiles and runs C# project
    dotnet run --project ./AntlrCSharp/AntlrCSharp.csproj
) ELSE (
    REM invalid argument
    echo Bruh what you doing you little Gumball??
)
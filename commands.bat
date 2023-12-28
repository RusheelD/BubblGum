@echo off

REM type ./commands [arg]. A command is run based off arg.

IF "%1"=="gen" (
    REM compiles and runs scanner + parser
    antlr4 -o ./Lexer_Parser/Parser ./Lexer_Parser/Parser/BubblGum.g4 -Dlanguage=CSharp
) ELSE IF "%1"=="run" (
    REM compiles and runs C# project
    dotnet run --project ./Main/Main.csproj "%2" "%3"
) ELSE (
    REM invalid argument
    echo Bruh what you doing you little Gumball??
)
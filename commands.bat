@echo off

REM type ./commands [arg] to run a command
REM ex. ./commands gen 

IF "%1"=="gen" (
    REM compiles and registers ANTLR grammar changes
    antlr4 -o ./Lexer_Parser/Parser ./Lexer_Parser/Parser/BubblGum.g4 -Dlanguage=CSharp
) ELSE IF "%1"=="run" (
    REM runs compiler
    dotnet run --project ./Main/Main.csproj "run" "%2" "%3"
) ELSE IF "%1"=="test" (
    REM tests compiler
    dotnet run --project ./Main/Main.csproj "test" "%2"
)  ELSE (
    REM invalid command
    echo Invalid command. What are you doing you pink Gumball???
)
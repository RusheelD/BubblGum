@echo off

REM type ./commands [arg] to run a command
REM ex. ./commands gen 

IF "%1"=="gen" (
    REM compiles and registers ANTLR grammar (lexer + parser)
    antlr4 -o ./Lexer_Parser/Parser ./Lexer_Parser/Parser/BubblGum.g4 -Dlanguage=CSharp
) ELSE IF "%1"=="genHeader" (
    REM compiles and registers ANTLR grammar storing imports + namespaces (used to link files efficiently)
    antlr4 -o ./Preprocessing/Parser ./Preprocessing/Parser/BubblGumHeader.g4 -Dlanguage=CSharp
) ELSE IF "%1"=="genAll" (
    REM runs gen and genHeader
    antlr4 -o ./Lexer_Parser/Parser ./Lexer_Parser/Parser/BubblGum.g4 -Dlanguage=CSharp
    antlr4 -o ./Preprocessing/Parser ./Preprocessing/Parser/BubblGumHeader.g4 -Dlanguage=CSharp
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
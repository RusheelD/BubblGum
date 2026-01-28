#!/bin/bash
# Build BubblGum Compiler as standalone executable

echo "============================================"
echo "Building BubblGum Compiler"
echo "============================================"

cd "$(dirname "$0")"

# Clean previous builds
echo "Cleaning previous builds..."
dotnet clean BubblGum.sln > /dev/null 2>&1

# Build as standalone executable (Linux)
echo "Building standalone executable..."
dotnet publish Main/Main.csproj -c Release -r linux-x64 --self-contained false /p:PublishSingleFile=true -o bin/Release/

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Build failed!"
    exit 1
fi

# Make executable
chmod +x bin/Release/bbgm

echo ""
echo "✅ Build successful!"
echo ""
echo "Executable location: bin/Release/bbgm"
echo ""
echo "To install system-wide:"
echo "  sudo cp bin/Release/bbgm /usr/local/bin/"
echo ""
echo "Try it:"
echo "  bin/Release/bbgm --help"
echo "  bin/Release/bbgm Tests/Lexer_Parser/HelloWorld.bbgm"
echo ""

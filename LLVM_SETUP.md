# LLVM Backend Setup Instructions

## Prerequisites

The BubblGum LLVM compiler backend requires the LLVM native libraries to be installed on your system.

## Installation

### Windows

1. **Download LLVM**
   - Go to [LLVM Releases](https://github.com/llvm/llvm-project/releases)
   - Download the latest Windows installer (e.g., `LLVM-20.0.0-win64.exe`)
   - Or use Chocolatey: `choco install llvm`

2. **Install LLVM**
   - Run the installer
   - Make sure to check "Add LLVM to the system PATH for all users" during installation
   - Default install location: `C:\Program Files\LLVM`

3. **Verify Installation**
   ```cmd
   llvm-config --version
   ```

4. **Set Environment Variable** (if not added automatically)
   - Add `C:\Program Files\LLVM\bin` to your PATH
   - Set `LLVM_SYS_200_PREFIX=C:\Program Files\LLVM` (adjust version as needed)

### macOS

```bash
brew install llvm@20
export PATH="/opt/homebrew/opt/llvm/bin:$PATH"
export LLVM_SYS_200_PREFIX="/opt/homebrew/opt/llvm"
```

### Linux (Ubuntu/Debian)

```bash
wget https://apt.llvm.org/llvm.sh
chmod +x llvm.sh
sudo ./llvm.sh 20
```

## Compiling BubblGum Programs

Once LLVM is installed, you can compile BubblGum programs to LLVM IR:

```bash
# Compile to LLVM IR
dotnet run --project Main/Main.csproj run -C yourprogram.bbgm

# This generates yourprogram.ll (LLVM IR)
```

## Compiling LLVM IR to Executable

After generating the `.ll` file, you can compile it to an executable:

```bash
# First, compile the runtime library
cd CodeGeneration/Runtime
clang -c runtime.c -o runtime.o

# Then link with your program
clang yourprogram.ll runtime.o -o yourprogram
./yourprogram
```

## Troubleshooting

### "Unable to load DLL 'libLLVM'"

This means LLVM is not installed or not in your PATH. Follow the installation steps above.

### Version Mismatch

The code uses LLVMSharp.Interop 20.1.2, which requires LLVM 20.x. Make sure your LLVM version matches.

## Alternative: Text-Only IR Generation

If you cannot install LLVM, you can still examine the generated IR by checking the `.ll` file that's created, though the module verification step will fail.

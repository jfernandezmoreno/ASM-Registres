#!/usr/bin/env bash
# Regenerate the ASM Registres installer.
# Usage: ./build_setup.sh [version]
# Example: ./build_setup.sh 1.0.1
set -e

VERSION="${1:-1.0.0}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR/../ASM_Registres_NET10"
PUBLISH_DIR="$PROJECT_DIR/bin/Publish/win-x64"
ISCC="/c/Users/juancarlosf/AppData/Local/Programs/Inno Setup 6/ISCC.exe"
TELERIK_LICENSE="$HOME/AppData/Roaming/Telerik/telerik-license.txt"

# ISCC does not accept bash-style UNC paths (//server/...). Use the Z: mapping instead.
# Z: is mapped to \\192.168.1.230\asm\dptos (verified via `net use`).
ISS_FILE_WIN='Z:\informatica\Aplicaciones\ASM_Registres\ASM_Registres_NET10\Setup\ASM_Registres.iss'

echo ">>> Publishing self-contained win-x64..."
cd "$PROJECT_DIR"
rm -rf "$PUBLISH_DIR"
dotnet publish -c Release -r win-x64 --self-contained true -o "$PUBLISH_DIR" -p:PublishSingleFile=false

echo ">>> Copying Telerik license..."
cp "$TELERIK_LICENSE" "$PUBLISH_DIR/telerik-license.txt"

echo ">>> Compiling installer (version $VERSION)..."
"$ISCC" "//DMyAppVersion=$VERSION" "$ISS_FILE_WIN"

echo ">>> Done. Installer at:"
ls -lh "$SCRIPT_DIR/Output/ASM_Registres_Setup_$VERSION.exe"

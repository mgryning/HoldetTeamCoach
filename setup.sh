#!/usr/bin/env bash
# Installs the latest stable version of dotnet using Microsoft's official script
set -e

# Download install script
curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh

# Install the latest version to /usr/local/dotnet
/tmp/dotnet-install.sh --version latest --install-dir /usr/local/dotnet

# Symlink dotnet into /usr/bin for ease of use
ln -sf /usr/local/dotnet/dotnet /usr/bin/dotnet

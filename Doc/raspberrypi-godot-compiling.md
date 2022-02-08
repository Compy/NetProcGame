# Godot / Mono / Pi

Cannot build directly to ARM32 from other machines to work on raspberry pi?

Godot has to be compiled on the PI and projects have to be built on the PI, same with export templates?

Instructions found:

https://www.reddit.com/r/godot/comments/kfi0oc/how_to_use_custom_export_templates/gl4vlpw/?utm_source=share&utm_medium=web2x&context=3

### Setup PI build environment

- `sudo apt-get install build-essential scons pkg-config libx11-dev libxcursor-dev libxinerama-dev libgl1-mesa-dev libglu-dev libasound2-dev libpulse-dev libudev-dev libxi-dev libxrandr-dev yasm`
- `sudo apt-get install clang`
- `sudo apt-get install lld`

### Setup for Mono builds

https://www.mono-project.com/download/stable/#download-lin-raspbian

Raspbian 10

```
sudo apt install apt-transport-https dirmngr gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/debian stable-raspbianbuster main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt update
```

### Download Godot source and checkout 3.4 branch

```
git clone https://github.com/godotengine/godot.git
git checkout 3.4
```

### Compile Godot

`scons platform=x11 target=release_debug tools=yes use_llvm=yes CCFLAGS="-mtune=cortex-a72 -mcpu=cortex-a72 -mfloat-abi=hard -mlittle-endian -munaligned-access -mfpu=neon-fp-armv8" module_mono_enabled=yes mono_glue=no -j4`

This takes a while so go for a sleep. `core/io` is one of the last directories to be built. Pi4 4GB [Time elapsed: 01:17:38.915.0]

## Compile Binaries

The following commands you need an X-Server setup if using SSH, startX. I just run direct from the raspberry PI terminal using Raspbian.

### Generate Mono Glue

`bin/godot.x11.opt.tools.32.llvm.mono  --generate-mono-glue modules/mono/glue/ --video-driver GLES2`

### Generate Binary

Make sure you have a dotnet SDK installed. .Net5.0 built in my tests.

`scons platform=x11  tools=yes use_llvm=yes CCFLAGS="-mtune=cortex-a72 -mcpu=cortex-a72 -mfloat-abi=hard -mlittle-endian -munaligned-access -mfpu=neon-fp-armv8" module_mono_enabled=yes mono_glue=yes -j4`

Good 30 minutes build time.

### Create export templates

```
scons platform=x11 tools=no use_llvm=yes CCFLAGS="-mtune=cortex-a72 -mcpu=cortex-a72 -mfloat-abi=hard -mlittle-endian -munaligned-access -mfpu=neon-fp-armv8" module_mono_enabled=yes mono_glue=yes target=debug -j4

scons platform=x11 tools=no use_llvm=yes CCFLAGS="-mtune=cortex-a72 -mcpu=cortex-a72 -mfloat-abi=hard -mlittle-endian -munaligned-access -mfpu=neon-fp-armv8" module_mono_enabled=yes mono_glue=yes target=release_debug -j4

scons platform=x11 tools=no use_llvm=yes CCFLAGS="-mtune=cortex-a72 -mcpu=cortex-a72 -mfloat-abi=hard -mlittle-endian -munaligned-access -mfpu=neon-fp-armv8" module_mono_enabled=yes mono_glue=yes target=release -j4

```
# Magic Leap Unity Project Template

## Project

Hello, Cube!

## Versions

### Unity

2019.2.x

### MLSDK

v0.23.0

### LuminOS

0.98.x

## Instructions After Downloading

1) Using Unity Hub, download Unity 2019.2.x and make sure Lumin support is checked during installation
2) `ADD` the project using Unity Hub
3) Open the project using Unity Hub
4) Under File > Build Settings, make sure the build target is Lumin
5) Under Unity preferences, set the MLSDK path
6) Under project settings > player settings > Lumin tab (Magic Leap logo icon) > publishing settings, set your cert path (and make sure the privkey file is in the same directory. If this is confusing, refer to and read our docs. There’s also a `README` in the privkey folder after unzipping)
7) Make sure USB debugging is enabled between your device and computer (which requires MLDB access) and you’re allowing untrusted sources
8) Open the `EmptyScene` Scene from `Assets`>`Scenes`>`EmptyScene`
9) Navigate to https://creator.magicleap.com/learn/guides/gsg-create-your-first-unity-app

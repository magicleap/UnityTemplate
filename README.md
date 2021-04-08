# Magic Leap Unity Project Template

## Overview
This project is meant to provide the Magic Leap Unity SDK and examples and has been configured to help the user quickly jump in start developing for the Lumin platform, whether it is via the Zero Iteration tool or deploying the app directly to the device. We recommend duplicating this project and using it as the foundation for new Magic Leap Apps. 


**Caution**: Do not add or change files in the *Assets/MagicLeap* folder to to prevent issues when importing new SDK versions in the future.

### Compatible with
- Unity Editor 2020.2
- LuminOS 0.98.20+
###  Packages Included
- Magic Leap XR Plugin 6.1.0-preview.2
- MLSDK v0.25.0


## Instructions After Downloading

1) Using Unity Hub, download Unity 2020.2.x and make sure Lumin support is checked during installation
2) `ADD` the project using Unity Hub
3) Open the project using Unity Hub
4) Under File > Build Settings, make sure the build target is Lumin
5) Under Unity preferences, set the MLSDK path
6) Under project settings > publishing settings, set your cert path (and make sure the privkey file is in the same directory. If this is confusing, refer to and read our docs. There’s also a `README` in the privkey folder after unzipping)
7) Make sure USB debugging is enabled between your device and computer (which requires MLDB access) and you’re allowing untrusted sources
8) Open the `HelloCube` Scene from `Assets`>`Scenes`>`HelloCube`
9) Navigate to https://developer.magicleap.com/learn/guides/gsg-create-your-first-unity-app

## Additional Information

[How to generate a ML Certificate](https://developer.magicleap.com/en-us/learn/guides/developer-certificates)  
[Enabling developer mode on your device](https://developer.magicleap.com/en-us/learn/guides/setting-up-your-device-for-development)  
[Downloading the mlsdk](https://developer.magicleap.com/en-us/learn/guides/develop-setup)  
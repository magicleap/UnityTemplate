# Magic Leap Unity Project Template


## Deprecation Notice

This project is depricated, please use the [MagicLeapUnityExamples](https://github.com/magicleap/MagicLeapUnityExamples) project instead.

## Overview
This project is meant to provide the Magic Leap Unity SDK and examples and has been configured to help the user quickly jump in start developing for the Lumin platform, whether it is via the Zero Iteration tool or deploying the app directly to the device. We recommend duplicating this project and using it as the foundation for new Magic Leap Apps. 


**Caution**: Do not add or change files in the `Assets/MagicLeap` folder to to prevent issues when importing new SDK versions in the future.

### Compatible with
- Unity Editor 2020.2
- LuminOS 0.98.20+
###  Packages Included
- Magic Leap XR Plugin 6.1.0-preview.2
- MLSDK v0.25.0


## Instructions After Downloading

1) Using **Unity Hub**, download Unity 2020.2.x and make sure Lumin support is selected during installation.
2) **Add** and **Open** the project using  Unity Hub.
4) Go to **File > Build Settings** and set the build target to **Lumin**.
5) Under **Unity Preferences**, set the **MLSDK** path. For example: `/Users/YourUserName/MagicLeap/mlsdk/0.25.0/`
6) Go to **Project Settings > Publishing Settings** and set the **ML Certificate** . Make sure the privkey file is in the same directory.
7) Make sure USB debugging is enabled between your device and computer (which requires MLDB access) and you’re allowing untrusted sources
8) Open the **HelloCube** scene located under `Assets/Scenes/` or learn how to [Create Your First App](https://developer.magicleap.com/learn/guides/gsg-create-your-first-unity-app).   
9) **Build and Run** the demo scene

## Using Zero Iteration  

1) In Unity, delete the `Assets/Plugins/Lumin/Editor` directory if it exists.  
2) Exit Unity.  
3) Launch **The Lab** and open **Zero Iteration**.  
4) Create or select a **Target** who's SDK matches the SDK that will be used in Unity.  
5) Toggle the state to **Connected** or select the **Connect** button.  
6) After ZI is running, launch Unity.  
4) Go to **Magic Leap > ML Remote > Import Support Libraries**.
5) Use Zero Iteration from Unity as normal.

### Notice  
This git project is configured to ignore the `Assets/Plugins/Lumin/Editor` folder. This is done intentionally, to avoid conflicting ZI files when developing between different operating systems and operating system versions.

## Additional Information

[How to generate a ML Certificate](https://developer.magicleap.com/en-us/learn/guides/developer-certificates)  
[Enabling developer mode on your device](https://developer.magicleap.com/en-us/learn/guides/setting-up-your-device-for-development)  
[Downloading the mlsdk](https://developer.magicleap.com/en-us/learn/guides/develop-setup)  
[Getting started with Zero Iteration](https://developer.magicleap.com/en-us/learn/guides/zero-iteration)  
[Using Zero Iteration inside Unity](https://developer.magicleap.com/en-us/learn/guides/1-3-zero-iteration-unity)  

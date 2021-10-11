# Setup VR Study

A computer with Windows 10 installed is required. The AR experiment is intended to only support the **Microsoft HoloLens Gen1** at the current stage.

## Basic PC Setup

- Clone this Repository
- Install [Visual Studio 2019](https://visualstudio.microsoft.com)
- Install [Unity 2019.2.13f1](https://unity3d.com/get-unity/download/archive)
- Install the workloads: Desktop development with C++ and Universal Windows Platform (UWP)
development
- You need at least the "Windows 10 SDK (10.0.xxxxx) Components! We later build for 10.0.18362.0
- Select "UWP build support" & IL2CPP for Hololens in the Build settings
- Connect Microsoft Xbox one Bluetooth Controller with the HoloLens

## Open Project & install required packages

Open the project with Unity and wait till it's fully loaded. All required packages are already included.

- Open the project with Unity and wait till it's fully loaded.
  - You'll might get several error messages. This will be fixed in the process
- Close "MRTK Project Configurator"-dialog for the moment.
- Open File -> Build Settings
  - Select "UWP"
  - Target Platform: HoloLens
  - Architecture: x86
  - Press Switch Platform
- Open "Mixed Reality Toolkit" -> Utilities -> "Configure Unity Project"
  - Apply
  - Wait for completion
- Open Windows -> "Package Manager", search for "Windows Mixed Reality" and install.
- Open Edit -> Projects Setings -> Audio and make sure "MS HRTF Spatializer" is selected at *Spatializer Plugin*
- Restart Unity
- Open Scene Assets/_project/Scenes/Performance
- If vibration cues should be involved:
  - Select in Hierarchy: Feedback -> Controller -> Transition. Look for "Tactor Controller (Script)" in the Inspector and Change "IP" to the IP of your raspberry, "Port" to 5005 and "Topic" to "/actuators"
Press play to check if scene can run.

## Build and deploy
- Open File -> Build Settings
- Make Sure only Scene "_projects/Scenes/Performance" is selected
- Press "Build"
- Select empty or create a new directory to build to.
- Wait for build.
- Open directory.
- Open multimodal-hololens.sln inside build-directory (Not the one inside your regular project folder)
- Wait for Visual Studio to load project
- At the top change Config to "Release", "x86" and "Device"
- Unfold "multimodal-hololens (Universal Windows)" and open "Package.appxmanifest
- Select tab "Capabilities" and enable:
  - Internet (Client and Server)
  - Internet (Client)
  - Low-Level-Devices
  - Private Area Network (Client and Server)
  - Spatial Perception
- Connect Hololens via usb to your computer.
- Right click on "multimodal-hololens (Universal Windows)" -> deploy

## Start Application on Hololens

- Make sure your XboX-controlleris connected to your Hololens
- Start the application "multimodal-hololens"

## Log files for Study
All log files can be found on the hololens via Device Portal. Open Device Portal and go to System -> File explorer -> LocalAppData -> multimodal-hololens -> LocalState. We create 2 files for every participant: "Study-PerformanceX.csv" and "Study-PrimaryX.csv". Number at the end will be increased sequently. If you delete all files it starts at zero again. If you delete Log File for Participant 2 out of 5. The next log file will be 2 again. Log files will be generated every time you start the application. On a re-deploy, all existing data will be lost!
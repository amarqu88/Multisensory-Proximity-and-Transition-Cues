# Setup VR Study

A computer with Windows 10 installed and a connected HTC Vive is required to run the system properly. Other VR hardware like Oculus Rift will probably work as well, but has not been tested yet. The project can also be tested within the Unity editor without VR equipment using only a keyboard. The VR experiment is intended to only support the simulation of the Microsoft HoloLens Gen1 at the current stage.

## Basic PC Setup

- Clone this Repository
- Install [Visual Studio 2019](https://visualstudio.microsoft.com)
- Install [Unity 2019.2.13f1](https://unity3d.com/get-unity/download/archive)
- Install [SteamVR](https://store.steampowered.com/app/250820/SteamVR/)
- Connect Microsoft Xbox one controller with the PC

## Open Project & install required packages

Open the project with Unity and wait till it's fully loaded.
All required packages are already included. In case something is not working properly, consider reimporting packages:

- TextMeshPro: Unity Editor -> Window -> TextMeshPro -> "Import TMP Essential Resources" -> Import
- [SteamVR](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647)
- [SteamAudio](https://github.com/ValveSoftware/steam-audio/releases/tag/v2.0-beta.18)
- [extOSC](https://assetstore.unity.com/packages/tools/input-management/extosc-open-sound-control-72005) (for communication with Raspberry Pi for vibration cues)

Originally, the paid Asset was used [Odin](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) for a better visualization of the simulated FOVs. All Odin references were removed or commented out due to licensing policies. 

## Troubleshooting

- Restart Unity
- *XR: OpenVR Error! OpenVR failed initialization with error code VRInitError_Init_HmdNotFound: "Hmd Not Found (108)!*: This error is shown when your HMD is not connected or SteamVR is closed. It should not affect the functionality of the system
- Yellow warnings should also not affect the functionality of the system
- Check if steam spatializer is selected under global audio settings: Edit -> Project Settings -> Audio -> Spatializier Plugin
- Open Scene: _project/Scenes/PerformanceStudy
- Connect your XBOX-Controller to your computer. Unity should recognize the controller.
Output in unity Console: *Joystick connected ("Controller (Xbox One For Windows)")*
Press Play.
- For vibration cues:
  - Select in Hierarchy: Feedback -> Controller -> Transition. Look for "Tactor Controller (Script)" in the
  - Inspector and Change "IP" to the IP of your raspberry, "Port" to 5005 and "Topic" to "/actuators"

## Log files for Study

All log files can be found in the project directory under Assets. We create 2 files for every participant: "Study-
PerformanceX.csv" and "Study-PrimaryX.csv". Number at the end will be increased sequently. If you delete all
files it starts at zero again. If you delete Log File for Participant 2 out of 5. The next log file will be 2. Log files will
be generated every time you start the application.

# Remarks
We currently do not build the project as stand alone version. Instead we run it in the editor.



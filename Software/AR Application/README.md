# Follow the below steps to set up the Attina AR application on your smartphone:

1. Install Unity 2020.3.12f1 on your computer if you haven't done so already.
2. Clone this Unity project from GitHub.
3. Open Unity and click on "Open Project" on the initial screen. Navigate to the folder where you downloaded the project and select the project folder to open it in Unity.
4. Set the build settings for the platform you want to build for. To do this, go to File > Build Settings. Select the platform you want to build for (Android/iOS) and click on "Switch Platform" to set it.
5. Open the 'AttinaAR' scene and click on the Comms gameobject. Enter the controller's IP address in the exposed parameter in the Inspector window.
6. If you are building for Android, you will need to have the Android SDK and JDK installed on your computer. Unity will automatically detect these if they are installed in the default locations. If not, you will need to set the path to these in the Unity preferences.
7. Connect your Android/iOS device to your computer using a USB cable.
8. In Unity, go to File > Build and Run (or Build and Export for iOS) to build the project for your device.
9. For Android: Once the build is complete, the app will be installed on your device and you can test it out. For iOS: Follow the additional instructions below.

### Additional Steps for iOS Build

1. Make sure you have Xcode installed on your computer.
2. In Unity, go to Edit > Project Settings > Player and select the iOS platform from the list of platforms.
3. Set the company and product name for your app under "Player Settings" in the "Identification" section.
4. Set the bundle identifier for your app in the "Other Settings" section. The bundle identifier should be unique and in the format of a reverse domain name (e.g. com.yourcompany.yourappname).
5. Select the iOS target device for your app (e.g. iPhone, iPad) in the "Other Settings" section.
6. Set the minimum iOS version required for your app in the "Other Settings" section.
7. Make sure you have a valid iOS developer account and certificate set up in Xcode.
8. Connect your iOS device to your computer using a USB cable and open Xcode.
9. In Xcode, go to Window > Devices and Simulators and select your iOS device.
10. In Unity, go to File > Build and Export and select the iOS platform.
11. Save the Xcode project in a folder on your computer and open it in Xcode.
12. In Xcode, select your iOS device as the target and click on the "Run" button to build and install the app on your device.


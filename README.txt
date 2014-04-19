Team: ZAVA Studios
Project: RougeCraft
Site: http://www.eng.utah.edu/~siu/ZAVA




External Libraries
==========================


This project has all the libraries needed included in the submitted package. The notable exception to this are for setting up publication on the OUYA. All instructions for this will be located in the “Running on OUYA” section of this document.




Running the game
==========================


For these steps to work, you must have Unity installed. For best results, the version of Unity should be  4.3 and above. Both the free and paid versions of Unity will work.


Method 1
* Inside of the Assets directory, open the main scene file, “mainGame.unity”, using the Unity Editor.
* To start the game, either click the Play button at the top of the editor, go to Edit ->Play, or press ctrl+P(in Windows).
        Method 2
* Inside of the Assets directory, open the main scene file, “mainGame.unity”, using the Unity Editor.
* Go to File-Build Setting. Make sure that all the scenes in the build are checked and included in the build.
* Select the platform to build on.
* If you are prompted for a screen resolution, choose 1280x720, or higher if possible, our game was designed around this minimum resolution. If lower resolutions are used, you some of the menus may get cut off.

Warning
==========================
Occasionally the files associating prefabs (meta files) with their scripts or other assets may get corrupted (usually from a bad merge). In these situations, the associations need to be made in the prefabs again.




Running on OUYA
==========================


WARNING: These instructions are hairy, only partially complete, and fraught with peril. These instructions are only known to work - and only conditionally at that - on Windows 8. If you’re really dedicated to getting this built on the actual device, though, feel free to continue reading.


For instructions from the OUYA Plugin’s creator, go here:
https://devs.ouya.tv/developers/docs/unity


In order to get RogueCraft to build, from scratch, and launch on the OUYA console, several steps must be followed carefully:


Part 1) Install the Android SDK and Android NDK. Complete instructions found here:
        https://devs.ouya.tv/developers/docs/setup


   * Downloaded version of the Android SDK is required to be an outdated version, specifically Platform-16. This download is not available from standard Android Sources, so to obtain the required files, follow the instructions in the provided link.
   * Ensure you place the downloaded SDK directory somewhere memorable. We’ll need to sync that up with Unity later.
   * Once downloaded, open the SDK Manager and download:
   * Android SDK Tools (paying careful attention that the revision is 21)
   * Android SDK Platform-tools
   * SDK Platform
   * Android Support Library
   * Google USB Driver (this will be important later)


Part 2) Install the Java JDK. The version must be JDK6 1.6, 32-bit. Doesn’t matter if you’re on a 64-bit machine, only 32-bit will work.


Part 3) Setup Unity to work for Android Development and the OUYA Plugin


It should be the case that our Unity Project has all the necessary tweaks to make it build properly for the Android platform. Further, the necessary OUYA plugins should already function out of the box. However, you will need to make sure of the following things:


   * There should be a tab available on the left hand side with OUYA in the title. If there is not, select the Window menu option, and click “Open Ouya Panel”.
   * Check the Java JDK, Android SDK, and Android NDK buttons in the Ouya Panel. If any of the Paths (located at the top) are greyed out, you will need to select the correct paths on your computer.
   * If this panel has a large error message reading “[error](bundle mismatched)”, or similar, click the “Sync Bundle ID” button.


Part 4) Install necessary drivers on the OUYA console


For one reason or another, the OUYA console does a terrible job at being recognized as a proper Android device on PCs. Whereas your average Android phone or tablet can be plugged in and install drivers automatically, this doesn’t work with the OUYA. You’ll need to do this yourself.


   * Plug the OUYA in (both into the wall, and into your PC’s USB port. The OUYA must be on in order to be recognized) and wait for your PC to recognize it.
   * Go to your PC’s Device Manager (Right click on My Computer -> Manage -> Device Manager), and locate the OUYA on the list.
   * Right click on the OUYA device, and select “Update Driver Software…”
   * Click “Browse my computer for driver software.”
   * Click Browse, and navigate to your Android SDK’s base folder. Then go to extras/google/usb_driver/, and select android_winusb.inf


Part 5) Launch the game


   * Return to Unity, and go to File -> Build Settings…
   * Select Android as the platform, then click Build and Run.
   * You will be prompted to save a .apk file. Choose whatever name you would like, save that file, and wait for the game to build.
   * Upon completion, the game should automatically launch on the OUYA (assuming, of course, your OUYA is plugged into an HDMI monitor so you can see it launch).


Again, I must reiterate that I cannot provide any guarantee these steps will work. In many cases, in particular, installing the drivers with those prescribed steps have not worked for me. The precise steps are not recommended: the require editing security settings on your PC to allow installation of insecure drivers (which the provided android_winusb.inf is), and tinkering with BIOS settings to allow launching your computer in Safe Mode.


In all reality, I am only sharing these steps as a requirement for the project. In practice, this game would be built, submit to the OUYA app store, and never built by an external party who was not already intimately familiar with working on the OUYA itself. Do not damage your computer for the sake of testing this build on the OUYA platform - it is not worth it for you, I promise.
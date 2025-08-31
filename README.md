# JellyBuddy
A Jellyfin companion app for iOS and Android built in .NET Maui (because I don't wanna learn the webdev languages for app dev and want to make a cross platform app :D no hate)

## Building from Source
1. Clone the repo
2. Install .NET 8
3. Install Maui and the required platform workloads
    ```
    dotnet workload install maui maui-android maui-ios
    ```
4. Open the solution in your preferred C# IDE
5. Get a Syncfusion community license key and add it to a `Syncfusion.Licensing.txt` file
   1. You can find instructions on how to get a Syncfusion key here: https://help.syncfusion.com/common/essential-studio/licensing/how-to-generate
   2. The Jellybuddy csproj is configured to reference this file, so if you don't have it, it will be easy to create in the correct place.
   3. **You must not commit this file to the repo**
6. Create and update JellyfinServerUrl.txt in the Jellybuddy.Core project and populate with your Jellyfin server URL
   1. This file is ignored by the gitignore, so you can add your private server URL to it if you want to test with a private server.
   2. This URL is used to generate the Jellyfin API Models which are used for DTO purposes.
7. Build and run!



## TODO LIST

1. Ability to manage multiple servers
2. See Active sessions, with playback state, the device playing, user and if it is direct or transcode, also there ability to send a message to that session
3. Then comes the library tab, where you could look at the number of items in your library, and going into each one would just load a poster, and basic item details on more details
4. Then Users tab, where you can create, edit/delete/modify a user, basically everything that is in the user page in the jellyfin web (allowing transcoding, library permission etc) also shows if the user is active or not.
5. Them comes statistics, something like streamyfin/jellystats, number of movies in the libbrary, number of watched movies and also if possible per user stats. (nice looking graphs)
6. More details - basically everything else in the jellyfin dashboard, plugins, scheduled tasks, devices, activity log, Alerts, etc

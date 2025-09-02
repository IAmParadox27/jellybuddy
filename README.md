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



## To-Do LIST

## Server Management
- [ ] Add ability to manage multiple Jellyfin servers
- [ ] Switch between servers easily

## Active Sessions
- [ ] Show all active sessions
  - [ ] Display playback state (playing, paused, stopped)
  - [ ] Show device playing
  - [ ] Show user name
  - [ ] Indicate if playback is direct or transcoded
- [ ] Add ability to send a message to any session

## Library Management
- [ ] Show number of items in each library
- [ ] Display library content with posters (Streamyfin style?)
- [ ] Show basic item details
- [ ] Provide a “more details” view for each item

## Users Management
- [ ] List all users
- [ ] Show active/inactive status
- [ ] Add ability to Create new users
- [ ] Add ability to Edit existing users
- [ ] Add ability to Delete users
- [ ] Add ability to Manage user permissions (library access, transcoding, etc.)

## Statistics
- [ ] Display library statistics (similar to Streamyfin/JellyStats)
  - [ ] Total number of movies/TV shows
  - [ ] Number of watched items
  - [ ] Per-user statistics
- [ ] Visualize data with graphs (Streamystats style?)

## More Details / Advanced
- [ ] Plugins management
- [ ] Scheduled tasks overview and ab
- [ ] Device list and details
- [ ] Activity log
- [ ] Alerts & notifications

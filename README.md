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

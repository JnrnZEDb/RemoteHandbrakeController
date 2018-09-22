# RemoteHandbrakeController
Simple app to control HandbrakeCLI from a separate PC.

## Description
The purpose of this app is to be able to encode video files using HandBrakeCLI from a Windows PC but using a separate PC to actually do the encoding.  This app simply reads from a file location to get a list of .mkv files.  Then with a list of desired files to encode, it sends a command to a Linux PC with HandBrakeCLI installed to encode the files.  

Since HandBrake likes using a lot of threads, this allows you to encode your videos from your Windows PC and still be able to play games (or whatever else) without your CPU being hogged down.  Let another PC do it for you.

The design of this program was based around for personal use so it won't really fit other people's uses.

Future versions will ideally allow for more customization and configuration.
## Potentially Useful Links
### Instructions for building HandBrakeCLI for Linux
https://handbrake.fr/docs/en/latest/developer/build-linux.html
### Mount Windows Shares in Linux
https://wiki.ubuntu.com/MountWindowsSharesPermanently

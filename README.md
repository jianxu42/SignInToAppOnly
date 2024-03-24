# SITAO: SignInToAppOnly Automation Utility
Welcome to the SITAO (SignInToAppOnly) utility! This tool is designed to help users automate the sign-in process to applications by monitoring for specific windows and hyperlinks.

## Features
- Automated Window Monitoring: Continuously scans for a window with the specified title.
- Hyperlink Interaction: Automatically detects and interacts with a hyperlink within the identified window.
- Ease of Use: Just pass the window title and hyperlink text as arguments, and let SITAO handle the rest.

## How to use it 
To use SITAO, provide it with two arguments: the exact name of the window you want to target, and the text of the hyperlink you wish to invoke. It's a simple command-line invocation that saves you time by automating repetitive sign-in steps.
```
SignInToAppOnly.exe "YourWindowName" "YourHyperlinkText"
```
SITAO is perfect for users who frequently interact with applications that require manual sign-ins through a graphical interface. Now you can automate this with ease, thanks to SITAO's seamless integration with the Windows Automation API.

Start simplifying your sign-in processes today with SITAO!


## Example of Power Apps
```
SignInToAppOnly.exe "Power Apps" "No, sign in to this app only"
```
![Alt text](PowerApps.png)

## Note
This is solely for testing purposes and should not be considered reliable.

Ensure that it does not cause excessive interruptions or interference with the target application, especially if the application has anti-automation measures. Additionally, frequent polling and automated actions may be considered malicious behavior by some antivirus software. Always ensure that your automation strategy complies with the application's terms of use and privacy policy.

# SLDC: Soulslike Death Counter & Discord Enhancer
<img src="https://github.com/KarolWasTaken/sldc/blob/master/Resources/Bonfire-black.svg" alt="SLDC logo" height="200">
**SLDC** is your *ultimate companion* for Soulslike games, designed to track your in-game deaths and enhance your Discord Rich Presence.

## Dark and Light Theme
![MainWindow Design](https://github.com/KarolWasTaken/sldc/blob/master/GITHUB-RESOURCES/dark.gif)

**Theme Support:** Choose between dark and light modes to suit your preferences.

**Death Tracker:** Keep an accurate count of your in-game deaths in real-time.

## DRP
![DRP](https://github.com/KarolWasTaken/sldc/blob/master/GITHUB-RESOURCES/DRP.png)

**Enhanced Discord Rich Presence:** Display dynamic details like your death count and current covenant directly in your Discord profile.

Whether you're battling through challenging bosses or running your next no-death attempt, SLDC makes sure your progress (or lack thereof) is front and center.

## Streamer Window
![Streamer Window](https://github.com/KarolWasTaken/sldc/blob/master/GITHUB-RESOURCES/streamerWindow.png)

**Streamer-Friendly Window:** A green-screen-compatible "Streamer Window" that displays your death count for seamless integration into your streams.

**Minimize to Toolbar:** Say goodbye to taskbar clutter! With our cutting-edge Minimize-to-Toolbar feature, you can keep your workspace clean by tucking away SLDC into the toolbar until you need it.

## Install
- Head over to the [latest release](https://github.com/KarolWasTaken/sldc/releases/latest) and download the zip file
- Extract the zip file into a folder
- Run ``sldc.exe``
- Press "Connect" when in-game
- Enjoy!
### Credit
Thank you to [Nordgaren](https://github.com/Nordgaren) for making the library that made all this possible: [PropertyHook!](https://github.com/Nordgaren/PropertyHook)

## Run Code Locally Notes

Please create a file `.env` in the root of the project that stores the discord rich presence tokens in the following format
```
DS1_TOKEN=<token>
DS2_TOKEN=<token>
DS3_TOKEN=<token>
BL_TOKEN=<token>
```
The files for each asset for the discord rich presence are found inside [resources](https://github.com/KarolWasTaken/sldc/tree/master/Resources).



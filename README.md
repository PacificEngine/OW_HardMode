![GitHub release (latest by date)](https://img.shields.io/github/v/release/PacificEngine/OW_HardMode?style=flat-square)
![GitHub Release Date](https://img.shields.io/github/release-date/PacificEngine/OW_HardMode?label=last%20release&style=flat-square)
![GitHub all releases](https://img.shields.io/github/downloads/PacificEngine/OW_HardMode/total?style=flat-square)
![GitHub release (latest by date)](https://img.shields.io/github/downloads/PacificEngine/OW_HardMode/latest/total?style=flat-square)

# Randomizer Mod by Pacific Engine

## Installing the Mod
1) Download and Install https://outerwildsmods.com/
1) From the Application, Install `PacificEngine's Common Resources` by `PacificEngine`
1) From the Application, Install `Difficulty Mod` by `PacificEngine`

## Using the Mod
All mod features can be configured in settings.

Includes:
* Shortening Loop Duration
* Adding a Damage Multiplier
* Changing Maximum Fuel
* Changing Maximum Oxygen
* Disabling the Ship
* Adjusting Anglerfish Detection Distance
* Adjusting Anglerfish Speed and Acceleration
* Preventing Anglerfish from getting Stunned
* Giving Anglerfish the ability to smell you or your ship
* Giving Anglerfish the ability see you or your ship (Good luck!)

## Creating Code
Create a new file called `PacificEngine.OW_HardMode.csproj.user`
```text/xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <OuterWildsRootDirectory>$(OuterWildsDir)\Outer Wilds</OuterWildsRootDirectory>
    <OuterWildsModsDirectory>%AppData%\OuterWildsModManager\OWML\Mods</OuterWildsModsDirectory>
  </PropertyGroup>
</Project>
```

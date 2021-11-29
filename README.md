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

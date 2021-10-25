# Using the Mod
All mod features can be configured in settings.

Includes:
* Shortening Loop Duration
* Adding a Damage Multiplier
* Changing Maximum Fuel
* Changing Maximum Oxygen
* Disabling the Ship
* Adjusting Anglerfish Detection Distance
* Adjusting Anglerfish Speed and Acceleration

# Creating Code
Create a new file called `Jose.CheatsMod.csproj`
```text/xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <OuterWildsRootDirectory>$(OuterWildsDir)\Outer Wilds</OuterWildsRootDirectory>
    <OuterWildsModsDirectory>%AppData%\OuterWildsModManager\OWML\Mods</OuterWildsModsDirectory>
  </PropertyGroup>
</Project>
```
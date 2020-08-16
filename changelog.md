# v1.0.0 - Initial Release
- Adds a "Ranged Damage Per Second" stat to ranged weapons
- The base stat is the expected DPS if all shots hit.  Clicking it gives a detailed breakdown of expected DPS at various ranges based on the the weapon's accuracy stats


# v1.0.1 - Best Accuracy
**Minor tweaks**
- Updated the DPS stat to use the best-achieveable accuracy of the gun instead of assuming an accuracy of 100%.  Should make it easier to compare weapons with low base accuracy, like miniguns


# v1.1.0 - Turret Support
**Features**
- Added support for turrets as well
- Added back the original DPS ignoring accuracy as a separate stat, "Max Ranged Damage Per Second"

**Bugfixes**
- Added support for minimum ranges to the turrets


# v1.1.1 - Hotfix
**Bugfixes**
- Replaced the bad assembly that was uploaded in v1.1.0 by accident.  Should fix the null pointer exceptions


# v1.1.2 - Hotfix
**Bugfixes**
- Fixed an issue that caused a null pointer exception to be thrown when looking at an abstract ThingDef (e.g. in the Prepare Carefully screen)
- Fixed the turret support to work with mortars and similar ammunition-using turrets.
- Added several sanity checks with verbose error messages to hopefully catch the most common null pointer exceptions

**Known Issues**
- Turrets with swappable ammunition (e.g. mortars) will only show their DPS when loaded with ammunition.  This is a technical limitation due to there being no easy way to get a sane default ammunition.


# v1.2.0 - Rimworld 1.2 support
**Features**
- Updated to Rimworld 1.2
- Added a new Ranged DPS stat to pawns and turrets that accounts for their shooting ability in addition to everything else.
- The Ranged DPS and Average DPS stats now also shows the range at which their respective DPS is achieved.  As before, this is the range that maximizes DPS.

**Changes**
- Turrets' old "Ranged damage per second" stat has been renamed to "Weapon ranged DPS", to make it clear it's only based on the turret's weapon and does not include the shooting accuracy of the mount.
- "Ranged damage per second" for guns has been renamed "Average ranged DPS" to differentiate it from the "Ranged DPS" stat on pawns.
- "Max ranged damage per second" has been renamed "Max ranged DPS" to be consistent with the other stat names

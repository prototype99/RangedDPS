# v1.0.0 - Initial Release
- Adds a "Ranged Damage Per Second" stat to ranged weapons
- The base stat is the expected DPS if all shots hit.  Clicking it gives a detailed breakdown of expected DPS  at various ranges based on the the weapon's accuracy stats


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

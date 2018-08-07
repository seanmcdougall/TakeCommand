1.4.12.2
	Updated .version file for all of 1.4
	Added OpenCockpit rear gunner seat

1.4.12.1
	Added fix so returning to editor won't prevent spawning again
	Added check for female kerbal 

1.4.12
	Updated for 1.4.1
	Updated build scripts
	Updated version file

1.4.11.1
	Added line for the Stockalike Mk1 Open Cockpit in the patch to add a vesselType line

1.4.11
- Added check for external seat being inside fairing
- Added Jenkins build

1.4.10.1
 - Updated MM dll

1.4.10
 - Updated for 1.3.1

1.4.9.1
- Added MM patch to add add AutoAction function to External Command Seat-like parts. 

1.4.9
- Updated for 1.3

1.4.8
- Now finds the best kerbal in the seats

1.4.7
- Added Akita rover seat from USI Konstruction/MKS, thanks Aelfhe1m
- Added code to override UpdateControlSourceState, to report correct state for external seats

1.4.6
- Updated all patches to add EVA Parachute if that mod is installed
- removed BetterCrewAssignment (can't do internal transfer to external seat)
- Moved cfg for stock external command seat to ModuleManager directory
- Fixed code so that KSP won't try to transfer a kerbal into/outof an external seat
- Added AssemblyVersion code

1.4.5
- Added patch for Omicron
- Added patch form Kerbonov-KN2

1.4.4
- Added patch for WildBlueIndustries
- Added patch for USI Exploration
- Added patch for NESDparts

1.4.3
- Patch added for BetterCrewAssignment
- Fixed issue when a part has multiple external seats
- Patch added for SXTContinued, for the Lark

1.4.2.1 (2016/12/17)
- Fixed build scripts (paths were wrong)
- Now contains correct .version file

1.4.2 (2016/12/13)
- Recompiled for 1.2.2
- added build scripts integrated with VS

1.4.1 (2015/05/06)
- recompiled for KSP 1.1.2
- updated to ModuleManager 2.6.24

1.4.0 (2015/04/24)
- updated and recompiled for KSP 1.1
- updated to ModuleManager 2.6.23

1.3.0 (2015/12/06)
- now allows spawning Kerbals into command seats after launch (i.e. on a crew transfer)

1.2.1 (2015/11/14)
- recompiled for KSP 1.0.5
- updated to ModuleManager 2.6.13

1.2 (2015/09/04)
- fixes compatibility issue with Action Groups Extended (and likely other mods as well)
- updated to use ModuleManager 2.6.8

1.1.4 (2015/06/23)
- recompiled for KSP 1.0.3/1.0.4
- updated to latest version of ModuleManager

1.1.3 (2015/05/21)
- fixes compatibility issue with Kerbal Inventory System (KIS)
- fixes compatibility issue with Kerbal Joint Reinforcement (KJR)

1.1.2 (2015/05/15)
- tweaked MM config again to add ":for" tag

1.1.1 (2015/05/12)
- tweaked MM config to remove ":final" tag
- module is now just disabled after processing is complete instead of being removed from the part

1.1 (2015/05/12)
- fixed "No Control" warning
- ModuleManager is now required for more reliable PartModule manipulation (latest version is included in release package)
- general code cleanup and commenting

1.0 (2015/05/10)
- initial release

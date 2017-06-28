# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) 
and this project adheres to [Semantic Versioning](http://semver.org/).

## [1.4.4] - 2017-06-28
## Fixes
 - #33 Fix issue where Overlays were not drawn during init, thx @renfred

## [1.4.3] - 2017-06-06
## Fixes
 - #32 (IntPtr, JniHandleOwnership) constructor to avoid leaky abstraction, thx @shmoogems

## [1.4.2] - 2017-05-15
### Added
 - #30 Enhance map with ability to choose whether to show callout on tap or not, thx @renfred

## [1.4.1] - 2017-04-25
### Fixes
 - #29 Fix an issue where native controls are still shown when disabled. thx @renfred

## [1.4.0] - 2017-04-19
### Fixes
 - #26 iOS Remove Overlay does not function. thx @Steve-Himself

### Added
 - Add conditional flags to enable/disable touch map to dismiss and show/hide native zoom and location buttons. Thx @renfred

## [1.3.9] - 2017-04-18
### Fixes
 - Fix item deselection issue and inconsistency between two versions #25, thx @renfred

## [1.3.8] - 2017-03-22
### Fixes
- #23 Android maps not showing user location, thx @shmoogems

## [1.3.7] - 2017-03-06
### Fixes
- Fixes NullReferenceException when the selected item is null

## [1.3.6] - 2017-02-27
### Fixes
- #21 Android MapRegion incorrectly constructed in OnCameraChange

## [1.3.5] - 2017-02-22
### Fixes
- #18 Android does not update VisibleRegion when map position changed
- #20 Update CI to automatically build nuget package with correct version

## [1.3.3] - 2017-02-05
### Fixes
- #16 - Fix for the android ShowInfoWindow glitch when marker clicked
- #14 - iOS MoveToRegion bug fix calculating the counterpoint of the MapRegion
- fix possible NullReference Exception if SelectedItem is null and PropertyChanged Event is handled.

## [1.3.2] - 2017-01-17
### Fixes
- Fix for Android OnInfoWindowClick Error

## [1.3.1] - 2016-11-22
### Added
- Linker support: UnifiedMaps is now linker safe
- Selection test to sample app

## [1.3.0] - 2016-11-06
### Added
- support for pin images and selection
- circle map overlay
- bindable visible-region property

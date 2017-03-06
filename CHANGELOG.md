# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) 
and this project adheres to [Semantic Versioning](http://semver.org/).

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

# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [2.0.0] - 2020-12-28 by Robert Classen
### Changed
- Re-implemented entire code-base
- Changed name to NoteFinder
- Changed Notes to be drawn in a collapsible tree hierarchy (matching the folder structure in the Project window, excluding "empty" folders in which no Notes are found)
- Moved Tag list into new menu to improve layout
- Replaced FileSystemWatcher with custom implementation to be able to detect changes to script files more reliably

### Added
- Added option to toggle Tags on or off
- Added option to set custom colors for Tags


## [1.0.0] - 2016-03-06 by Denis Sylkin
### Added
- Implemented base version of ToDoManager
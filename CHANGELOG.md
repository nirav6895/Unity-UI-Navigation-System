# Changelog

All notable changes to this project will be documented in this file.

---

## [1.2.1] - 2026-11-21

### Fixed
- Popups stayed invisible while the game is paused (`Time.timeScale = 0`), e.g. a pause menu: their open/close animation used scaled time. It now uses unscaled time
- Notifications never closed automatically while the game is paused. The auto close now waits `Configurations.Notification.ShowTime` in unscaled time
- Removed the stray `Samples.meta`, which made Unity warn that the meta file exists but its folder can't be found in every project that installs the package
- README install URL pointed to an old version

## [1.2.0] - 2026-11-10

### Added
- MIT license (`LICENSE.md`), and the license in `package.json`

## [1.1.0] - 2026-11-03

### Added
- `UINavigationManager.ReplaceUI<T>()` to replace the current UI without growing the stack
- Virtual `OnShow()` / `OnHide()` hooks in `UI`
- iOS edge swipe gesture to go back (same as the device back key), configurable from `Configurations.SwipeBack` (left / right edge)

### Changed
- Default UI is never hidden by `HideCurUI()` (close button / device back key) and never removed by `ReplaceUI<T>()`
- Going back to a Screen shows it again with its previous sorting order and parameters. `SetParameters()` is not called again
- Showing a Screen hides the current Screen along with all Popups above it. Going back shows them again
- Showing the UI which is already on top only sets its parameters, instead of adding it to the stack again
- Sample moved to `Samples~`. Import it from Package Manager

### Fixed
- Popups and Notifications placed in the scene under `uiRoot` were not found
- `HideAllUITill<T>()` was hiding UIs while T was on top, instead of till T is on top
- Popup transition was stopping all coroutines of the popup
- Log messages were built even when logs are disabled
- `SetDefaultUI<T>()` was throwing NullReferenceException when UI is not found
- Static state (navigation stacks and `OnChangeCurShowingUI` listeners) was kept between Play sessions when Domain Reload is disabled. It's now reset when Play Mode starts
- `UIPrefabListController.Instance` is cleared when the controller is destroyed

## [1.0.0] - 2026-05-05

### Added
- UI navigation including Screens, Popups & Notifications (stack-based)
- Animated transitions for all kind of UI
- Notifications with Auto-Close funcationality
- Back/Close funcationality inlcuding Device Back Key
- Sample demo scene

### Notes
- Initial stable release

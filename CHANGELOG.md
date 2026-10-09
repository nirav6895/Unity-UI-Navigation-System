# Changelog

All notable changes to this project will be documented in this file.

---

## [Unreleased]

### Added
- `UINavigationManager.ReplaceUI<T>()` to replace the current UI without growing the stack
- Virtual `OnShow()` / `OnHide()` hooks in `UI`

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

## [1.0.0] - 2026-05-05

### Added
- UI navigation including Screens, Popups & Notifications (stack-based)
- Animated transitions for all kind of UI
- Notifications with Auto-Close funcationality
- Back/Close funcationality inlcuding Device Back Key
- Sample demo scene

### Notes
- Initial stable release

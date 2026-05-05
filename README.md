# Unity UI Navigation System

A reusable and modular UI navigation framework for Unity, designed to manage screens, popups, notifications, transitions between them and back navigation in a scalable and maintainable way.

---

## 🚀 Overview

In modern Unity games, UI navigation often becomes complex as features grow. This system provides a clean and extensible architecture to handle:

- UI navigation including Screens, Popups & Notifications (stack-based)
- Animated transitions for all kind of UI
- Notifications with Auto-Close funcationality
- Back/Close funcationality inlcuding Device Back Key

This package is built with production use-cases in mind and can be integrated into both small and large-scale projects.

---

## ✨ Features

### 🧭 UI Navigation
- Stack-based UI management (Push & Pop UI)
- Strongly-typed API support
- Parameter passing between UI

### 🎬 Transitions
- Built-in animation support
- Interface-based system (`ITransition`)
- Easily customizable

### 🔔 Notifications
- Lightweight toast system
- Configurable duration & animation

### 🔙 Back Navigation
- Android back button support
- Escape key support (PC)
- Unified navigation handling

### 🧱 Architecture
- Modular and decoupled design
- Built using SOLID principles
- Reusable across multiple projects

---

## 📦 Installation

### Option 1: Git URL (Recommended)

Open Unity → Package Manager → Add package from Git URL:

```
https://github.com/nirav6895/Unity-UI-Navigation-System.git
```

---

### Option 2: Local Package

1. Clone/download this repository
2. Copy `Unity-UI-Navigation-System` into your project
3. Add via Package Manager → Add from disk

---

## 🛠️ Basic Usage

```csharp
UINavigationManager<SNP>.Instance.ShowUI<HomeScreen>();
UINavigationManager<Notification>.Instance.ShowUI<NoInternetNotification>();
UINavigationManager<SNP>.Instance.HideCurUI();
UINavigationManager<SNP>.Instance.HideCurUIOnlyIfMatch(this);
```

---

## 📂 Structure

```
Packages/com.nirav.screen-navigation/
├── Documentation/
├── Runtime/
├── Samples/
├── package.json
```

---

## 🎮 Sample

A demo scene is included under:

```
Samples/BasicNavigationDemo
```

This demonstrates:
- UI Navigation
- Popup over Screens
- Notifications
- Back navigation
- Transition Animations

---

## 🧠 Design Principles

This system follows:

- Single Responsibility Principle
- Open/Closed Principle
- Decoupled architecture via interfaces
- Reusable modular design

---

## 📈 Use Cases

- 2D Games UI
- 3D Games UI
- AR/VR UI

---

## 📚 Documentation

- [Documentation](./Packages/com.nirav.screen-navigation/Documentation~/Manual.md)

---

## 📜 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

Developed by Nirav Patel  
Senior Unity Game Developer (10 years)

---

## ⭐ Support

If you find this useful, consider giving it a ⭐ on GitHub!

# Unity Screen Navigation System - Documentation

---

## 🧭 Overview

This package provides a structured way to manage UI navigation in Unity using a modular and extensible architecture.

---

## 🧩 Core Components

### UI
Base class for all kind of UI like Screen, Popup and Notification.

Base class:
```
UI
```

### Screen, Popup and Notification
All three class are derived from UI class.
Screen represents full UI Page. (e.g., Main Menu, Store)
Popup represents overlay UI on top of another Popup or any screen. (e.g., Exit Game Popup, Offer Popup)
Notification is overlay UI, slides as a notification and get closed automatically after some time if not closed manually. (e.g., No Internet Notification)

Base class:
```
Screen
Popup
Notification : UI
```

### UINavigationManager
Main manager who manages the navigation of UIs in the game. It uses a stack to manage the showing and hiding of UIs.

Base class:
```
UINavigationManager
```

### UIPrefabListController
Manages the list of UI prefabs and their instantiation.

Base class:
```
UINavigationManager
```

### ITransition
Manages UI transition.

interface:
```
ITransition
```

---

## 📦 Setup

1. Install Package
2. Create a gameobject in scene and add component "UIPrefabListController". Set "UiRoot" in "UIPrefabListController". Set "UiRoot" to the "Your Preferred GameObject" where you would like to instantiate all runtime UIs and where all existing UIs are present as a child of that GameObject in scene.
3. Create "UI Prefab" by adding component which gets inherited from one of this component (Screen/Popup/Notification) to your UI object. (e.g., Create class HomeScreen : Screen and add HomeScreen component to root of your UI Prefab)
4. Add "UI Prefabs"(means Prefab with UI Component) in list under "UIPrefabListController".
Option 1: Simply add your "UI Prefab" in your scene under UiRoot.
Option 2: If you want your "UI Prefab" to instantiate on demand then create Scriptable Object of "UIPrefabListSO" and add "UI Prefab" reference to that Scriptable Object and attach that Scriptable Object to "UIPrefabListController" component in scene.
5. Implement a code to Show UI(Screen/Popup/Notification)

```csharp
UINavigationManager<SNP>.Instance.ShowUI<HomeScreen>();
UINavigationManager<Notification>.Instance.ShowUI<NoInternetNotification>();
```

To Navigate Screen/Popup, use UINavigationManager<SNP>.Instance
To Navigate Notification, use UINavigationManager<Notification>.Instance


---

## ⚙️ Best Practices

- Keep UI logic separate from business logic
- Use events instead of tight coupling
- Avoid direct references between screens
- Use ScriptableObjects for configurations (optional)
- Keep transitions reusable

---

## 🧠 Architecture Notes

- Designed for scalability
- Suitable for LiveOps-heavy games
- Works for both 2D & 3D projects
- Optimized for mobile-first navigation patterns

---

## 📌 Summary

This system helps you:
- Reduce UI complexity
- Improve maintainability
- Standardize navigation across projects
- Build scalable game UI systems

---

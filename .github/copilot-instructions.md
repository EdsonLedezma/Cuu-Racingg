# Project Guidelines

## Code Style
- Use Unity C# conventions used in this repo (`ProjectSettings/ProjectVersion.txt` is Unity `6000.3.6f1`).
- Keep gameplay scripts under `CuuRacing.<Feature>` namespaces (`Garage`, `Mobile`, `Settings`, `UI`, `Gameplay`).
- Keep serialized inspector fields documented with `[Header]` and `[Tooltip]` when adding or changing public inspector wiring.
- Keep XML summaries for public types and public methods in project scripts.
- Use `_camelCase` for private fields.
- Use the Unity New Input System (`UnityEngine.InputSystem`) APIs for input logic. Project setting `activeInputHandler: 1` means New Input System only.

## Architecture
- Main runtime flow is scene-driven: `MainMenu` -> race scenes (`DefaultScene - Mobile` or `PlainTestTrack - Mobile`) with `Garage` and `Ajustes` as supporting scenes.
- Feature boundaries in `Assets/Scripts`:
  - `Garage/`: car data, selection, and track selection (`CarData`, `GarageManager`, `TrackSelector`, `AutoSpawner`).
  - `Mobile/`: gyro, HUD layout loading, and mobile controls.
  - `Settings/`: settings persistence and mobile button layout configuration.
  - `UI/`: menu navigation and async scene loading.
- Cross-scene state relies on `PlayerPrefs`:
  - `GarageManager` writes selected car/track.
  - `AutoSpawner` reads selected car.
  - `AjustesController` writes gyro/layout settings.
  - `GyroscopeController` and `MobileHUDLayoutLoader` read those settings.
- Preserve `[DefaultExecutionOrder(-100)]` in `Assets/Scripts/Garage/AutoSpawner.cs`; other systems depend on it running early.

## Build and Test
- No repo-level build/test scripts are defined (`npm`, `dotnet`, or CI scripts are not present).
- Standard validation flow is Unity Editor based:
  - Open project in Unity `6000.3.6f1`.
  - Verify scene order in `ProjectSettings/EditorBuildSettings.asset`.
  - Validate behavior in `MainMenu`, `Garage`, `Ajustes`, and race scenes.
- Only run Unity batchmode build/test commands when explicitly requested; they are environment-dependent and slow.

## Conventions
- Keep scene name strings exact and synchronized with Build Settings paths/names.
- When writing settings that must survive scene transitions, call `PlayerPrefs.Save()` before loading a scene.
- Keep these intentionally disabled placeholders disabled unless BxB package integration is being intentionally reworked:
  - `Assets/Scripts/Mobile/CameraButtonMobile.cs`
  - `Assets/Scripts/Settings/SettingsManager.cs`
- Treat `Assets/BxB Studio`, `Assets/SlimUI`, and `Assets/TextMesh Pro` as external dependencies in this repo context (see `.gitignore`).

## References
- `SETUP_MANUAL_UNITY.md` (full scene setup walkthrough).
- `PLANIFICACION_DEFAULTSCENE_MOBILE.md` (mobile HUD/layout planning).
- `Packages/dev.bxbstudio.inputs-manager/README.md` (input package notes).
- `Packages/dev.bxbstudio.inputs-manager/CONTRIBUTING.md` (package coding style for that package scope).
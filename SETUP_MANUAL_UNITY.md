# SETUP MANUAL DETALLADO - Integración Mobile Completa en Cuu Racing

## 🎯 RESUMEN EJECUTIVO

### Arquitectura de Escenas y Flujo:
```
MainMenu.unity
├─ Btn_Garage → Garage.unity
├─ Btn_Jugar → DefaultScene-Mobile.unity (con TrackSelector)
└─ Btn_Ajustes → Ajustes.unity ⭐ [NUEVA]

Ajustes.unity ⭐ [NUEVA ESCENA]
├─ Panel Giroscopio (toggle + slider)
├─ Panel Configurar Botones (abre sub-panel)
│  └─ Sub-panel Layout (4 botones arrastrables)
└─ Volver → Garage.unity

Garage.unity (modificado)
├─ TrackSelector (dropdown con Pista Clásica/Prueba) ⭐
└─ OnPlayClick → carga escena seleccionada + auto

DefaultScene-Mobile.unity & PlainTestTrack-Mobile.unity (modificados)
├─ Canvas HUD Mobile
│  ├─ Joystick (ya existe)
│  ├─ Btn_Accelerate ⭐
│  ├─ Btn_Brake ⭐
│  ├─ Btn_Camera 🎥 ⭐
│  └─ Btn_Lights 💡 ⭐
├─ GyroscopeController ⭐
└─ LightController ⭐
```

### Todos los Botones Mobile (COMPLETO):
| Botón | Escena | Función | Script |
|-------|--------|---------|--------|
| **Joystick** | Race | Dirección izq/dcha | Dynamic Joystick.prefab (BxB) |
| **Acelerador (Throttle)** | Race | Acelera auto | MobileHUDController |
| **Freno** | Race | Frena auto | MobileHUDController |
| **Cámara 🎥** | Race | Cambia perspectiva | CameraButtonMobile |
| **Luces 💡** | Race | Enciende/apaga luces | LightButtonMobile |
| **Freno Mano** | Race | Freno emergencia | MobileCanvas.prefab (BxB) |

---

## ✅ PASO 0: Verificar MainMenu.unity (YA HECHO)

**CuuRacingMenu.cs está actualizado:**
```csharp
public string settingsSceneName = "Ajustes";

public void OnAjustesClick()
{
    PlayClick();
    StartCoroutine(LoadSceneAsync(settingsSceneName));
}
```

**En MainMenu.unity:**
- El botón `Btn_Ajustes` → OnClick → CuuRacingMenu.OnAjustesClick()
- ✅ Esto carga automáticamente Ajustes.unity

---

## 🎨 PASO 1: CREAR ESCENA Ajustes.unity (VERSIÓN SIMPLIFICADA)

### ⚡ ESTRUCTURA FINAL - 3 PARTES INDEPENDIENTES

```
La escena tiene 3 COSAS PRINCIPALES que funcionan INDEPENDIENTES:

1️⃣  PANEL GIROSCOPIO (siempre visible)
   - Toggle: "Incluir Giroscopio" 
   - Slider: "Sensibilidad 1-10"
   - Botón: "Configurar Botones"
   - Botón: "Volver"

2️⃣  SUB-PANEL BOTONES (oculto, aparece al hacer click)
   - 4 botones pequeños que puedes ARRASTRAR
   - Botón: "Guardar" (guarda posiciones)
   - Botón: "Cerrar" (oculta sub-panel)

3️⃣  SCRIPT AjustesController.cs (lo hace todo)
   - Detecta clicks
   - Guarda datos en PlayerPrefs
   - Muestra/oculta sub-panel
```

---

### 1.1 CREAR CANVAS - NIVEL 0

**Paso 1: Crear Canvas base**
```
Right-click en la escena → UI (TextMeshPro) → Panel
→ Renombrar a: "Ajustes"
```

**Paso 2: Configurar Canvas**
- En Canvas component:
  - Render Mode: Overlay
  - Scale Mode: Scale with Screen Size
  - Reference Resolution: 1920 x 1080

**Resultado:** Tienes un Canvas vacío gris. ✅

---

### 1.2 CREAR PANEL GIROSCOPIO - NIVEL 1

```
Ajustes (Canvas)
└─ SettingsPanel (Panel - caja blanca central)
   └─ [Aquí van todos los elementos]
```

**Paso 1: Crear Panel blanco**
```
Right-click Ajustes → UI → Panel
→ Renombrar a: "SettingsPanel"
```

**Paso 2: Posicionar Panel**
- RectTransform:
  - Anchor: Center (9 cuadrados del medio)
  - Pos X: 0, Pos Y: 0
  - Width: 600
  - Height: 600
- Image component:
  - Color: White

**Resultado:** Ves un panel blanco en el centro de la pantalla. ✅

**Paso 3: Agregar Layout Group (para organizar elementos)**
- Add Component → Vertical Layout Group
- Layout Element:
  - Check "Preferred Width": 600
  - Check "Preferred Height": 600

---

### 1.3 ELEMENTOS DENTRO DEL PANEL - NIVEL 2

Todos estos elementos van **dentro de SettingsPanel**:

#### A) TITULO "AJUSTES"
```
Right-click SettingsPanel → UI → Text (TextMeshPro)
→ Renombrar a: "LabelTitle"
→ Text: "AJUSTES"
→ Font Size: 40, Bold
→ Alignment: Center
```

#### B) ETIQUETA + TOGGLE "Giroscopio"
```
Right-click SettingsPanel → UI → Toggle
→ Renombrar a: "GyroToggle"
→ Label text: "Incluir Giroscopio"
→ Toggle.IsOn: OFF (por defecto desactivado)
```

#### C) ETIQUETA + SLIDER "Sensibilidad"
```
Right-click SettingsPanel → UI → Slider
→ Renombrar a: "GyroSensSlider"
→ Min Value: 0.1
→ Max Value: 10
→ Value: 5
→ Whole Numbers: OFF
```

**Agregar etiqueta de valor:**
```
Right-click GyroSensSlider → UI → Text (TextMeshPro)
→ Renombrar a: "SensValueText"
→ Text: "5.0"
→ Size: 60 x 40
```

#### D) BOTONES EN FILA (ABAJO DEL PANEL)
```
Crear 2 botones lado a lado:

1. Right-click SettingsPanel → UI → Button
   → Renombrar a: "ConfigBtn"
   → Text: "⚙️ Configurar"
   → Color: Cyan
   → Size: 200 x 60

2. Right-click SettingsPanel → UI → Button
   → Renombrar a: "BackBtn"
   → Text: "← Volver"
   → Color: Red
   → Size: 150 x 60
```

**Resultado en el panel:**
```
┌─────────────────────────────┐
│        AJUSTES              │
│                             │
│  ☑ Incluir Giroscopio       │
│  Sensibilidad: [====•===] 5.0
│                             │
│  [⚙️ Configurar] [← Volver] │
└─────────────────────────────┘
```

✅ **El panel giroscopio está LISTO**

---

### 1.4 CREAR SUB-PANEL (PARA ARRASTRAR BOTONES) - NIVEL 2

Este panel aparece **encima del anterior** cuando haces click en "⚙️ Configurar"

```
Ajustes (Canvas)
├─ SettingsPanel (Panel blanco) [del paso anterior]
│
└─ ButtonLayoutPanel (Panel - SUB-PANEL OCULTO)
   ├─ Bg_Overlay (Image - fondo semi-transparente)
   ├─ Btn_Acelerar (Button pequeño para arrastrar)
   ├─ Btn_Frenar (Button pequeño)
   ├─ Btn_Camera (Button pequeño)
   ├─ Btn_Lights (Button pequeño)
   ├─ BtnSave (Button "Guardar")
   └─ BtnClose (Button "Cerrar")
```

#### Paso 1: Crear Canvas para Sub-Panel
```
Right-click Ajustes → UI → Panel
→ Renombrar a: "ButtonLayoutPanel"
→ Anchor: Stretch-Stretch
→ Left: 0, Right: 0, Top: 0, Bottom: 0
→ RectTransform: Offsaets to 0
```

**Image component:**
- Color: Black, Alpha: 0.7 (transparente)

**Resultado:** Cubre toda la pantalla con un overlay negro. ✅

#### Paso 2: Crear fondo del sub-panel (caja interior)
```
Right-click ButtonLayoutPanel → UI → Panel
→ Renombrar a: "PreviewBox"
→ Width: 500, Height: 400
→ Anchor: Center
→ Color: Gray
```

**Resultado:** Ves un cuadro gris en el centro sobre el overlay negro.

#### Paso 3: Crear 4 botones pequeños (DENTRO de PreviewBox)

```
Para cada botón:
Right-click PreviewBox → UI → Button
→ Size: 80 x 80
→ Text tamaño: 40, emoticón (▶, ⏹, 📷, 💡)
```

**Nombres y posiciones:**
```
Btn_Acelerar: (-150, -50)  - Color: GREEN   - Text: ▶
Btn_Frenar:   (150, -50)   - Color: RED     - Text: ⏹
Btn_Camera:   (-150, 150)  - Color: BLUE    - Text: 📷
Btn_Lights:   (150, 150)   - Color: YELLOW  - Text: 💡
```

#### Paso 4: Botones Guardar/Cerrar (ABAJO del sub-panel)
```
Right-click PreviewBox → UI → Button
→ Renombrar a: "BtnSave"
→ Text: "✓ Guardar"
→ Color: GREEN
→ Pos: (-100, -180)

Right-click PreviewBox → UI → Button
→ Renombrar a: "BtnClose"
→ Text: "✕ Cerrar"
→ Color: RED
→ Pos: (100, -180)
```

**Resultado en pantalla:**
```
┌──────────────────────────────────────┐
│  Fondo negro 70% transparente        │
│  ┌──────────────────────────────────┐│
│  │    PREVIEW DE BOTONES            ││
│  │  ▶        🎥                     ││
│  │          📷                      ││
│  │  ⏹        💡                     ││
│  │    [✓ Guardar] [✕ Cerrar]       ││
│  └──────────────────────────────────┘│
└──────────────────────────────────────┘
```

✅ **El sub-panel está LISTO**

---

### 1.5 OCULTAR SUB-PANEL AL INICIO

**IMPORTANTE:** El ButtonLayoutPanel debe estar **OCULTO por defecto**

- Click en ButtonLayoutPanel en Hierarchy
- En Inspector: Desmarca el checkbox "Active" (o Script lo hace)
- El toggle aparecerá cuando hagas click en "⚙️ Configurar"

---

### 1.6 AGREGAR EL SCRIPT AjustesController.cs

### 1.6 AGREGAR EL SCRIPT AjustesController.cs

#### Paso 1: Crear GameObject para el script
```
Right-click Ajustes → Create Empty
→ Renombrar a: "AjustesManager"
```

#### Paso 2: Agregar script
- Click en AjustesManager
- Inspector → Add Component → AjustesController.cs

#### Paso 3: Asignar referencias en Inspector (TABLA SIMPLIFICADA)

En AjustesController inspector, arrastra estos GameObjects del árbol:

| Campo en Script | Arrastrar GameObject | Notas |
|---|---|---|
| **Gyro Toggle** | SettingsPanel → GyroToggle | El toggle del giroscopio |
| **Gyro Sens Slider** | SettingsPanel → GyroSensSlider | El slider 1-10 |
| **Sens Value Text** | SettingsPanel → SensValueText | TMP_Text que muestra "5.0" |
| **Button Layout Panel** | ButtonLayoutPanel | El sub-panel oculto |
| **Buttons to Manage[]** Size | 4 | Cantidad de botones |
| **Buttons to Manage[0]** | ButtonLayoutPanel → Btn_Acelerar | ▶ |
| **Buttons to Manage[1]** | ButtonLayoutPanel → Btn_Frenar | ⏹ |
| **Buttons to Manage[2]** | ButtonLayoutPanel → Btn_Camera | 📷 |
| **Buttons to Manage[3]** | ButtonLayoutPanel → Btn_Lights | 💡 |
| **Config Button** | SettingsPanel → ConfigBtn | "⚙️ Configurar" |
| **Save Layout Button** | ButtonLayoutPanel → BtnSave | "✓ Guardar" |
| **Close Layout Button** | ButtonLayoutPanel → BtnClose | "✕ Cerrar" |
| **Back Button** | SettingsPanel → BackBtn | "← Volver" |
| **Garage Scene Name** | (dejar) | "Garage" |

**Resultado:** El script tiene acceso a todos los elementos. ✅

---

### 1.7 CONECTAR BOTONES A EVENTOS (OnClick)

#### Botón "⚙️ Configurar"
- Click en ConfigBtn en Hierarchy
- En Inspector → Button → On Click()
- Click "+" para agregar evento
- Arrastra "AjustesManager" al espacio en blanco
- Dropdown: AjustesController → **OnConfigButtonClick()**

#### Botón "✓ Guardar" 
- Click en BtnSave
- Button → On Click()
- Click "+"
- Arrastra AjustesManager
- Dropdown: AjustesController → **OnSaveLayoutClick()**

#### Botón "✕ Cerrar"
- Click en BtnClose
- Button → On Click()
- Click "+"
- Arrastra AjustesManager
- Dropdown: AjustesController → **OnCloseLayoutClick()**

#### Botón "← Volver"
- Click en BackBtn
- Button → On Click()
- Click "+"
- Arrastra AjustesManager
- Dropdown: AjustesController → **OnBackClick()**

#### Toggle "Incluir Giroscopio"
- Click en GyroToggle
- Toggle → On Value Changed (bool)
- Click "+"
- Arrastra AjustesManager
- Dropdown: AjustesController → **OnGyroToggleChanged(bool)**

#### Slider "Sensibilidad"
- Click en GyroSensSlider
- Slider → On Value Changed (Single)
- Click "+"
- Arrastra AjustesManager
- Dropdown: AjustesController → **OnSensitivitySliderChanged(float)**

**Resultado:** Todos los botones conectados a funciones. ✅

---

### 1.8 CÓMO FUNCIONA (FLUJO SIMPLE)

```
Usuario hace CLICK EN "⚙️ Configurar"
           ↓
OnConfigButtonClick() se ejecuta
           ↓
ButtonLayoutPanel.SetActive(true) [el sub-panel aparece]
           ↓
Usuario ARRASTRA botones a nuevas posiciones
           ↓
Usuario hace CLICK EN "✓ Guardar"
           ↓
OnSaveLayoutClick() se ejecuta
           ↓
Script GUARDA posiciones en PlayerPrefs
           ↓
Sub-panel se oculta
           ↓
LISTO: La próxima vez que cargues carrera, botones estarán en nuevas posiciones
```

---

## 📊 TABLA RÁPIDA - LO QUE NECESITAS HACER

| Escena | Estado | Acción | Objetos | Scripts | Tiempo |
|--------|--------|--------|---------|---------|--------|
| MainMenu | ✅ LISTO | Nada | 0 | 0 | 0 min |
| **Ajustes** | 🔲 HACER | Crear nueva | 13 | 1 | 30-40 min |
| Garage | 🔲 MODIFICAR | Agregar dropdown | 1 | 1 | 10 min |
| DefaultScene-Mobile | 🔲 MODIFICAR | Agregar HUD | 9 | 3 | 40 min |
| PlainTestTrack-Mobile | 🔲 MODIFICAR | Copiar DefaultScene | 9 | 3 | 40 min |
| Build Settings | 🔲 VERIFICAR | Orden escenas | 0 | 0 | 5 min |
| | | **TOTAL** | **32 objetos** | **8 scripts** | **~2h 5min** |

---

## 🚗 PASO 2: Actualizar Garage.unity

### Estructura Rápida

```
Garage (Canvas ya existe)
├─ [Elementos existentes]
└─ TrackSelectorPanel (Panel - NEW)  ⭐
   ├─ Label: "PISTA"
   └─ TrackDropdown (TMP_Dropdown)
```

### Crear Dropdown

```
Right-click Canvas de Garage → UI (TextMeshPro) → Dropdown
→ Renombrar a: "TrackDropdown"
→ Tamaño: 300 x 60
→ Positioned: Arriba a la derecha
```

### Asignar Script

- Add Component → TrackSelector.cs
- Inspector:
  - Track Dropdown: Arrastra "TrackDropdown"
  - (el resto es automático)

### Resultado

El dropdown se llena automáticamente con:
- Pista Clásica (DefaultScene-Mobile.unity)
- Pista Prueba (PlainTestTrack-Mobile.unity)

✅ **PASO 2 LISTO**

---

## 📱 PASO 3: DefaultScene-Mobile.unity - HUD Mobile

### Estructura Rápida

```
DefaultScene-Mobile (Escena existente)
│
├─ [Vehículos, track, luces naturales - ya existe]
│
├─ Canvas-MobileHUD (NEW Canvas - UI flotante) ⭐
│  ├─ MobileHUDController (Script)
│  ├─ Joystick (prefab BxB Studio)
│  └─ ControlButtons (Panel con 4 botones)
│
└─ GyroscopeController (GameObject NEW)
   └─ GyroscopeController (Script)
```

### 3.1 Crear Canvas Nuevo

```
Right-click en escena → UI → Panel
→ Renombrar a: "Canvas-MobileHUD"
→ Canvas component:
   - Render Mode: Screen Space - Overlay
   - Order in Layer: 100 (para que esté adelante)
```

### 3.2 Agregar Joystick (prefab BxB)

```
En la carpeta Assets/BxB Studio/MVC Getting Started - Mobile/Prefabs/JoystickPack/
→ Arrastra "Dynamic Joystick.prefab" como HIJO de Canvas-MobileHUD
→ Posición: Lado izquierdo abajo (ej: X: -300, Y: -200)
```

**Resultado:** Ves el joystick flotante en la pantalla. ✅

### 3.3 Crear Panel de Botones

```
Right-click Canvas-MobileHUD → UI → Panel
→ Renombrar a: "ControlButtonsPanel"
→ Anchor: Bottom-Right
→ Width: 200, Height: 300
```

### 3.4 Crear 4 Botones (DENTRO de ControlButtonsPanel)

Cada botón: 80 x 80 px, con emoticón

```
Botón 1: Btn_Accelerate (▶ VERDE)
   Position: (-150, -50)
   Color: GREEN

Botón 2: Btn_Brake (⏹ ROJO)
   Position: (150, -50)
   Color: RED

Botón 3: Btn_Camera (📷 AZUL)
   Position: (-150, 150)
   Color: BLUE

Botón 4: Btn_Lights (💡 AMARILLO)
   Position: (150, 150)
   Color: YELLOW
```

**Para cada botón:**
```
Right-click ControlButtonsPanel → UI → Button
→ Renombrar
→ Tamaño: 80 x 80
→ Posición: (ver arriba)
→ Color: (ver arriba)
→ TMP_Text: (emoticón)
```

### 3.5 Agregar Scripts a Botones

**En Btn_Camera:**
- Add Component → CameraButtonMobile.cs
- (el resto es automático)

**En Btn_Lights:**
- Add Component → LightButtonMobile.cs
- (el resto es automático)

### 3.6 Agregar MobileHUDController

**En Canvas-MobileHUD:**
- Add Component → MobileHUDController.cs
- Inspector assignments:

| Campo | Arrastrar |
|-------|-----------|
| Mobile Canvas | Canvas-MobileHUD |
| Control Buttons[] Size | 4 |
| Control Buttons[0] | Btn_Accelerate |
| Control Buttons[1] | Btn_Brake |
| Control Buttons[2] | Btn_Camera |
| Control Buttons[3] | Btn_Lights |

### 3.7 Agregar GyroscopeController

```
Right-click en escena → Create Empty
→ Renombrar a: "GyroscopeController"
→ Add Component → GyroscopeController.cs
→ Inspector: Use Gyroscope ON
```

### 3.8 Agregar LightController al Vehículo

En el vehículo (dentro de Vehicles container):
```
Add Component → LightController.cs
Inspector:
   - Head Lights[]: [] (auto-detecta)
   - Brake Lights[]: [] (auto-detecta)
```

✅ **PASO 3 LISTO**

---

## 📱 PASO 4: PlainTestTrack-Mobile.unity

**REPETIR EXACTAMENTE TODO EL PASO 3**

(Misma estructura, mismos scripts, mismo layout)

Solo cambia: la escena del vehículo, pero los controles son idénticos

✅ **PASO 4 LISTO**

---

## � PASO 5: Verificar Build Settings

```
File → Build Settings
→ Agrega las escenas en este orden:

Scene 0: MainMenu.unity
Scene 1: Ajustes.unity ⭐ (NEW)
Scene 2: DefaultScene-Mobile.unity
Scene 3: PlainTestTrack-Mobile.unity
Scene 4: Garage.unity
```

✅ **PASO 5 LISTO - Setup Completo**

---

## 📋 CHECKLIST FINAL

Antes de jugar, verifica:

- [ ] Ajustes.unity creada con Panel + Sub-panel
- [ ] TrackDropdown en Garage.unity
- [ ] Canvas-MobileHUD en DefaultScene-Mobile.unity
- [ ] 4 botones (Acelerar, Frenar, Cámara, Luces) en DefaultScene
- [ ] Mismo setup en PlainTestTrack-Mobile.unity
- [ ] GyroscopeController en ambas escenas de carrera
- [ ] LightController en vehículos
- [ ] Build Settings: 5 escenas en orden correcto
- [ ] Todos los scripts compilando sin errores

---

## � RESUMEN VISUAL - CHEAT SHEET

### En 30 segundos: Qué va en CADA escena

#### 📋 MainMenu.unity (YA ESTÁ LISTO)
```
✅ Btn_Ajustes → OnClick() → CuuRacingMenu.OnAjustesClick()
   (Carga Ajustes.unity automáticamente)
```

#### 🎨 Ajustes.unity (CREAS NUEVA)
```
Canvas-Ajustes
├─ SettingsPanel (caja blanca)
│  ├─ LabelTitle "AJUSTES"
│  ├─ GyroToggle
│  ├─ GyroSensSlider + SensValueText
│  ├─ ConfigBtn ("⚙️ Configurar")
│  └─ BackBtn ("← Volver")
│
└─ ButtonLayoutPanel (sub-panel OCULTO)
   ├─ Btn_Acelerar ▶ (Draggable)
   ├─ Btn_Frenar ⏹ (Draggable)
   ├─ Btn_Camera 📷 (Draggable)
   ├─ Btn_Lights 💡 (Draggable)
   ├─ BtnSave "✓ Guardar"
   └─ BtnClose "✕ Cerrar"

AjustesManager (Empty)
└─ Add Script: AjustesController.cs (asigna refs en inspector)
```

#### 🚗 Garage.unity (MODIFICAS)
```
Canvas (ya existe)
├─ [Elementos existentes]
└─ TrackDropdown (NEW)
   └─ Add Script: TrackSelector.cs
```

#### 📱 DefaultScene-Mobile.unity (MODIFICAS)
```
Escena (ya existe con vehículo)
│
├─ Canvas-MobileHUD (NEW Canvas Overlay)
│  ├─ Joystick (prefab Dynamic Joystick.prefab)
│  ├─ ControlButtonsPanel (Panel)
│  │  ├─ Btn_Accelerate ▶ (GREEN)
│  │  ├─ Btn_Brake ⏹ (RED)
│  │  ├─ Btn_Camera 📷 (BLUE) + Script: CameraButtonMobile.cs
│  │  └─ Btn_Lights 💡 (YELLOW) + Script: LightButtonMobile.cs
│  │
│  └─ Add Script: MobileHUDController.cs (asigna refs)
│
├─ GyroscopeController (Empty NEW)
│  └─ Add Script: GyroscopeController.cs
│
└─ Vehículo (existente)
   └─ Add Script: LightController.cs (auto-detecta lights)
```

#### 📱 PlainTestTrack-Mobile.unity (MODIFICAS)
```
REPETIR TODO DE DefaultScene-Mobile.unity
(Misma estructura, mismos scripts, unterschiedlicher vehículo)
```

---

## �🎮 VERIFICAR QUE FUNCIONA

**Test 1: Navegación**
```
Play → MainMenu → Click "Ajustes" → Abre Ajustes.unity ✅
```

**Test 2: Giroscopio**
```
En Ajustes → Toggle ON → Slider a 5 → Cierra escena → Vuelve a abrir
→ Toggle sigue ON ✅ (guardado en PlayerPrefs)
```

**Test 3: Botones**
```
En Ajustes → "Configurar" → Aparece sub-panel ✅
→ Arrastra botones → "Guardar" → Sub-panel desaparece ✅
```

**Test 4: Carrera**
```
Garage → Select "Pista Clásica" → Play → DefaultScene carga ✅
→ Puedes acelerar, frenar, cambiar cámara, luces ✅
```

---

## ⚙️ DETALLES TÉCNICOS (Si algo no funciona)

### ¿Los botones no se arrastran?
- [ ] ButtonLayoutPanel tiene Canvas component?
- [ ] EventTrigger agregado a botones?
- [ ] Canvas.worldCamera = null?

Solución: Ver método `AddEventTriggersToButtons()` en AjustesController.cs

### ¿PlayerPrefs no guarda?
- [ ] ¿Clicaste "Guardar"?
- [ ] ¿ButtonLayoutPanel está activo cuando guardas?

Solución: En editor, Console verifica `PlayerPrefs.GetString("MobileButtonLayout")`

### ¿Escena no carga desde Garage?
- [ ] ¿TrackDropdown tiene opciones?
- [ ] ¿Build Settings contiene todas las escenas?
- [ ] ¿GarageManager.OnPlayClick() se ejecuta?

Solución: Verifica que TrackSelector.cs está asignado al Canvas

---

## 🗺️ ROADMAP VISUAL - RESUMEN EJECUCIÓN

### ORDEN RECOMENDADO PARA HACER TODO:

```
COMIENZA AQUÍ ↓

┌─────────────────────────────────────────────────────────────┐
│ 1️⃣  PASO 1: CREAR Ajustes.unity (La más larga - 30-40 min) │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ Create Canvas → Rename "Ajustes"                        │
│  ✓ Create SettingsPanel (white box in center)              │
│  ✓ Add Title + Gyro Toggle + Sensitivity Slider            │
│  ✓ Add 2 buttons: "Config" + "Back"                        │
│  ✓ Create ButtonLayoutPanel (hidden overlay)               │
│  ✓ Add 4 draggable buttons + Save/Close buttons            │
│  ✓ Create AjustesManager + Add AjustesController.cs        │
│  ✓ Assign 14 references in Inspector                       │
│  ✓ Connect 6 button OnClick events                         │
│                                                             │
│  ⏱️ Tiempo: 30-40 MINUTOS                                  │
│  ⚠️ CRÍTICO: Buttons DEBEN ser children de ButtonLayoutPanel
│  ✅ Cuando termines: Presiona "Config" y prueba arrastrar   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ 2️⃣  PASO 2: MODIFICAR Garage.unity (Rápido - 10 min)      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ Right-click Canvas → Create Dropdown                    │
│  ✓ Rename to "TrackDropdown"                               │
│  ✓ Assign TrackSelector.cs script                          │
│  ✓ Drag 2 references (Dropdown, Sound)                     │
│                                                             │
│  ⏱️ Tiempo: 10 MINUTOS                                     │
│  ✅ Cuando termines: Deberías ver "Pista Clásica" / "Prueba"
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ 3️⃣  PASO 3: MODIFICAR DefaultScene-Mobile.unity (40 min)  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ Create Canvas → Rename "Canvas-MobileHUD"              │
│  ✓ Drag Dynamic Joystick.prefab as child (BxB Studio)     │
│  ✓ Create ControlButtonsPanel with 4 buttons              │
│  ✓ Name them: Btn_Accelerate, Btn_Brake, Btn_Camera, etc  │
│  ✓ Assign 3 scripts to Canvas/Buttons                     │
│  ✓ Create GyroscopeController (Empty)                     │
│  ✓ Add LightController to Vehicle                         │
│  ✓ Assign ~8 references in Inspectors                     │
│                                                             │
│  ⏱️ Tiempo: 40 MINUTOS                                     │
│  ✅ Cuando termines: Ves Joystick + 4 buttons en carrera  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ 4️⃣  PASO 4: COPIAR TODO A PlainTestTrack-Mobile (40 min)  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ Repeats EXACTLY Step 3 but in PlainTestTrack scene    │
│  ✓ Same structure, different vehicle                       │
│                                                             │
│  ⏱️ Tiempo: 40 MINUTOS (es una copia paste)               │
│  ✅ Cuando termines: Dos escenas con HUD idéntico         │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│ 5️⃣  PASO 5: VERIFICAR Build Settings (5 min)              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ File → Build Settings                                  │
│  ✓ Add 5 scenes in order:                                 │
│    0: MainMenu.unity                                       │
│    1: Ajustes.unity ⭐ (NEW)                               │
│    2: DefaultScene-Mobile.unity                            │
│    3: PlainTestTrack-Mobile.unity                          │
│    4: Garage.unity                                         │
│                                                             │
│  ⏱️ Tiempo: 5 MINUTOS                                      │
│  ✅ Cuando termines: ¡LISTO PARA JUGAR!                   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              ↓
                    ✨ TODO COMPLETO ✨

TIEMPO TOTAL ESTIMADO: ~2 HORAS Y 5 MINUTOS

Si algo no funciona, revisa TROUBLESHOOTING arriba ☝️
```

---

## 📱 QUICK REFERENCE - LOS 4 ARCHIVOS SCRIPT

Todos estos archivos **YA EXISTEN** en tu proyecto:

```
Assets/Scripts/Settings/
  └─ AjustesController.cs ⭐ (La que hace TODO en Ajustes)

Assets/Scripts/Garage/
  └─ TrackSelector.cs ⭐ (Dropdown en Garage)

Assets/Scripts/Mobile/
  ├─ MobileHUDController.cs ⭐
  ├─ GyroscopeController.cs ⭐
  ├─ CameraButtonMobile.cs 🎥
  ├─ LightButtonMobile.cs 💡
  └─ LightController.cs 💡

Assets/Scripts/UI/
  └─ CuuRacingMenu.cs (Ya modificado)
```

Solo necesitas:
1. Crear los GameObjects en Unity
2. Asignar los scripts
3. Draggear las referencias

¡El código ya está listo! 🎉

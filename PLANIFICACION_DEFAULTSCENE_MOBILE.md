# 📱 PLAN DETALLADO - Configurar DefaultScene-Mobile.unity

## 🗺️ ESTRUCTURA ACTUAL DE TU ESCENA

```
DefaultScene - Mobile (Escena)
├─ _GameController
├─ PostProcessing
├─ Canvas ← AQUÍ YA EXISTE TU CANVAS
├─ MobileCanvas
│  ├─ MobilePreset0
│  ├─ MovementJoystick ← AQUÍ ESTÁ TU JOYSTICK
│  └─ Handle
├─ MobilePreset1
├─ SteeringWheel
├─ FuelPedal
├─ BrakePedal
├─ ClutchPedal
├─ NOS
├─ EventSystem
├─ SecondCanvas
├─ SimpleCamera
├─ Lighting
├─ Environment
├─ TrackBarriers
├─ Terrain
├─ Water
├─ Rocks
├─ AIPath
├─ ZonesContainer
├─ 000_AudioZone
├─ 001_WeatherZone
└─ Vehicles ← AQUÍ ESTÁ TU VEHÍCULO
   └─ Drivable
      ├─ 2020 Porsche Tay ← ESTE ES EL AUTO ACTIVO
      ├─ 2012 Lexus LFA
      └─ 2005 BMW M3 GT
```

---

## ✅ PASO 1: AGREGAR 2 BOTONES EN Ajustes.unity

**Ubicación:** Ajustes.unity → personalizar_btn_panel → controles (PreviewBox)

### 1.1 Crear Btn_Camera (AZUL 📷)

```
Right-click controles → UI → Button
  ✓ Renombrar: "Btn_Camera"
  ✓ Tamaño: 80 x 80
  ✓ Anchor: Center
  ✓ Position X: -150, Y: 50
  ✓ Image Color: BLUE (0, 0, 1, 1)
  ✓ TextMeshPro Text: "📷"
  ✓ Text Size: 40
  ✓ Text Alignment: Center
```

### 1.2 Crear Btn_Lights (AMARILLO 💡)

```
Right-click controles → UI → Button
  ✓ Renombrar: "Btn_Lights"
  ✓ Tamaño: 80 x 80
  ✓ Anchor: Center
  ✓ Position X: 150, Y: 50
  ✓ Image Color: YELLOW (1, 1, 0, 1)
  ✓ TextMeshPro Text: "💡"
  ✓ Text Size: 40
  ✓ Text Alignment: Center
```

**Resultado en Hierarchy:**
```
personalizar_btn_panel
└─ controles (PreviewBox)
   ├─ Btn_Acelerar ▶ (ya existe)
   ├─ Btn_Frenar ⏹ (ya existe)
   ├─ Btn_Camera 📷 ← NUEVO
   ├─ Btn_Lights 💡 ← NUEVO
   ├─ BtnSave ✓
   └─ BtnClose ✕
```

### 1.3 Actualizar AjustesManager

**En Ajustes.unity → AjustesManager → Inspector:**

```
Buttons to Manage
  ✓ Size: 4 → CAMBIAR A 6
  ✓ Element 0: Btn_Acelerar (ya asignado)
  ✓ Element 1: Btn_Frenar (ya asignado)
  ✓ Element 2: Btn_Camera (ya asignado)
  ✓ Element 3: Btn_Lights (ya asignado)
  ✓ Element 4: Arrastra personalizar_btn_panel → controles → Btn_Camera ← NUEVO
  ✓ Element 5: Arrastra personalizar_btn_panel → controles → Btn_Lights ← NUEVO
```

✅ **PASO 1 COMPLETADO**

---

## ✅ PASO 2: CREAR BOTONES EN DefaultScene-Mobile.unity

**Ubicación:** Usa tu **Canvas** existente (NO crees uno nuevo, usa el que ya tienes)

### 2.1 Verificar que Canvas existe

En tu Hierarchy, vemos que ya existe **"Canvas"** (aparece en la lista)

```
✓ Click en Canvas en Hierarchy
✓ Verifica en Inspector:
  - Canvas component (debe estar)
  - Render Mode: Screen Space - Overlay
  - Scale Mode: Scale with Screen Size
```

### 2.2 Crear 4 botones DENTRO del Canvas

**Para cada botón:**

```
Right-click Canvas → UI → Button
  ✓ Tamaño: 80 x 80
  ✓ Image component → Color: (ver arriba por botón)
  ✓ TextMeshPro → Font Size: 40, Alignment: Center
```

#### Botón 1: Btn_Accelerate (VERDE ▶)

```
Renombrar: "Btn_Accelerate"
Anchor: Bottom-Right
Position X: -150, Y: -50
Color: GREEN (0, 1, 0, 1)
Text: ▶
```

#### Botón 2: Btn_Brake (ROJO ⏹)

```
Renombrar: "Btn_Brake"
Anchor: Bottom-Right
Position X: 150, Y: -50
Color: RED (1, 0, 0, 1)
Text: ⏹
```

#### Botón 3: Btn_Camera (AZUL 📷)

```
Renombrar: "Btn_Camera"
Anchor: Bottom-Right
Position X: -150, Y: 50
Color: BLUE (0, 0, 1, 1)
Text: 📷
Add Component: CameraButtonMobile.cs ← IMPORTANTE
```

#### Botón 4: Btn_Lights (AMARILLO 💡)

```
Renombrar: "Btn_Lights"
Anchor: Bottom-Right
Position X: 150, Y: 50
Color: YELLOW (1, 1, 0, 1)
Text: 💡
Add Component: LightButtonMobile.cs ← IMPORTANTE
```

**Resultado en Hierarchy:**
```
Canvas
├─ Btn_Accelerate ▶
├─ Btn_Brake ⏹
├─ Btn_Camera 📷 (con script CameraButtonMobile)
└─ Btn_Lights 💡 (con script LightButtonMobile)
```

✅ **PASO 2 COMPLETADO**

---

## ✅ PASO 3: AGREGAR MobileHUDController

### 3.1 Crear GameObject para el script

```
Right-click Canvas → Create Empty
  ✓ Renombrar: "MobileHUDManager"
```

### 3.2 Agregar script al Canvas

**Opción A (Recomendado): Agregar al Canvas**

```
Click en Canvas
Inspector → Add Component → MobileHUDController.cs
```

**Opción B: Agregar al nuevo GameObject**

```
Click en MobileHUDManager
Inspector → Add Component → MobileHUDController.cs
```

### 3.3 Asignar referencias en el Inspector

**Después de agregar MobileHUDController, llena estos campos:**

```
Mobile Canvas:              Canvas (arrastra desde Hierarchy)

Joystick Panel:            MobileCanvas (ya existe en tu escena)

Camera Button:             Btn_Camera (tu botón nuevo)

Lights Button:             Btn_Lights (tu botón nuevo)

Control Buttons[] Size:     4

Control Buttons[0]:        Btn_Accelerate
Control Buttons[1]:        Btn_Brake
Control Buttons[2]:        Btn_Camera
Control Buttons[3]:        Btn_Lights
```

**Resultado en Inspector:**
```
Mobile HUD Controller (Script)
  ✓ Mobile Canvas: Canvas
  ✓ Joystick Panel: MobileCanvas
  ✓ Camera Button: Btn_Camera
  ✓ Lights Button: Btn_Lights
  ✓ Control Buttons Size: 4
    - Element 0: Btn_Accelerate
    - Element 1: Btn_Brake
    - Element 2: Btn_Camera
    - Element 3: Btn_Lights
```

✅ **PASO 3 COMPLETADO**

---

## ✅ PASO 4: AGREGAR GyroscopeController

### 4.1 Crear GameObject

```
Right-click en la escena (nivel raíz, NO dentro de Canvas)
  → Create Empty
  → Renombrar: "GyroscopeController"
```

### 4.2 Agregar script

```
Click en GyroscopeController
Inspector → Add Component → GyroscopeController.cs
```

**Resultado en Hierarchy:**
```
DefaultScene - Mobile (Escena)
├─ ... (otros elementos)
├─ GyroscopeController ← NUEVO
├─ ... (otros elementos)
└─ Vehicles
```

✅ **PASO 4 COMPLETADO**

---

## ✅ PASO 5: AGREGAR LightController AL VEHÍCULO

### 5.1 Encontrar el vehículo activo

En tu Hierarchy:
```
Vehicles
└─ Drivable
   ├─ 2020 Porsche Tay ← ESTE (es el que ves en la escena)
   ├─ 2012 Lexus LFA
   └─ 2005 BMW M3 GT
```

### 5.2 Agregar script

```
Click en "2020 Porsche Tay" (o el vehículo activo)
Inspector → Add Component → LightController.cs
(Déjalo en blanco, auto-detecta las luces)
```

**Resultado en Hierarchy:**
```
Vehicles
└─ Drivable
   ├─ 2020 Porsche Tay
   │  └─ Has LightController component ← AGREGADO
   ├─ 2012 Lexus LFA
   └─ 2005 BMW M3 GT
```

✅ **PASO 5 COMPLETADO**

---

## 🎯 CHECKLIST - ANTES DE JUGAR

En **Ajustes.unity:**
- [ ] Btn_Camera y Btn_Lights creados en personalizar_btn_panel
- [ ] AjustesManager.Buttons to Manage tamaño: 6
- [ ] Elements 4 y 5 asignados correctamente

En **DefaultScene-Mobile.unity:**
- [ ] 4 botones creados en Canvas (Accelerate, Brake, Camera, Lights)
- [ ] Btn_Camera tiene CameraButtonMobile.cs
- [ ] Btn_Lights tiene LightButtonMobile.cs
- [ ] MobileHUDController agregado y referencias asignadas
- [ ] GyroscopeController creado en nivel raíz
- [ ] LightController agregado al vehículo (2020 Porsche Tay)

---

## 📊 RESUMEN FINAL

| Acción | Ubicación | Estado |
|--------|-----------|--------|
| Crear Btn_Camera 📷 | Ajustes → personalizar_btn_panel | ✅ |
| Crear Btn_Lights 💡 | Ajustes → personalizar_btn_panel | ✅ |
| Actualizar AjustesManager Size | Ajustes → AjustesManager | ✅ |
| Crear 4 botones | DefaultScene → Canvas | ✅ |
| Agregar scripts a botones | DefaultScene → Canvas → Btn_Camera/Lights | ✅ |
| Agregar MobileHUDController | DefaultScene → Canvas | ✅ |
| Crear GyroscopeController | DefaultScene (nivel raíz) | ✅ |
| Agregar LightController | DefaultScene → Vehicles → Porsche | ✅ |

---

## 🚀 DESPUÉS DE COMPLETAR

1. **Guarda la escena** (Ctrl+S)
2. **Juega la escena** (Press Play)
3. **Ve a Ajustes**
4. **Click "Configurar"**
5. **Arrastra los 6 botones** a nuevas posiciones
6. **Click "Guardar"**
7. **Vuelve a carrera**
8. **Botones deben estar en nuevas posiciones** ✅

---

## ⚠️ NOTAS IMPORTANTES

- **No crees un Canvas nuevo**, usa el que ya tienes
- **MobileHUDController busca auto los componentes**, solo asigna referencias
- **CameraButtonMobile y LightButtonMobile son automáticos**, no necesitan configuración adicional
- **GyroscopeController debe estar en nivel raíz**, NO dentro de Canvas

¡Listo! Sigue este plan paso a paso. 🎮

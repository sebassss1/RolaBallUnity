# Unity Scene Setup Guide - Roll a Ball

Esta guía te ayudará a configurar la escena de Unity para el juego Roll a Ball. Sigue estos pasos cuidadosamente.

## 📋 Preparación

Todos los scripts ya están creados en `Assets/Scripts/`. Ahora necesitas configurar la escena en Unity.

---

## 🎮 Paso 1: Configurar el Player (Bola)

1. **Crear la bola del jugador:**
   - En la Jerarquía: Click derecho → 3D Object → Sphere
   - Renombrar a "Player"
   - Position: `(0, 0.5, 0)`
   - Scale: `(1, 1, 1)`

2. **Añadir componentes al Player:**
   - Selecciona "Player"
   - Add Component → Rigidbody
     - Mass: `1`
     - Drag: `0`
     - Angular Drag: `0.5`
     - Constraints: Freeze Rotation X y Z (marcar las casillas)
   - Add Component → Player Controller (el script que creamos)
     - Move Speed: `10`
     - Max Speed: `30`
     - Speed Increase Rate: `0.5`
     - Horizontal Speed: `15`
     - Boundary X: `4`

3. **Añadir Tag "Player":**
   - Con "Player" seleccionado
   - En el Inspector, arriba: Tag → Add Tag
   - Crear nuevo tag: "Player"
   - Volver a seleccionar "Player" y asignar el tag "Player"

---

## 📷 Paso 2: Configurar la Cámara

1. **Seleccionar Main Camera** en la Jerarquía

2. **Posicionar la cámara:**
   - Position: `(0, 5, -8)`
   - Rotation: `(30, 0, 0)`

3. **Añadir script:**
   - Add Component → Camera Controller
   - Player: Arrastra el objeto "Player" desde la Jerarquía
   - Offset: `(0, 5, -8)`
   - Smooth Speed: `0.125`
   - Look At Player: ✓ (marcado)

---

## 🏗️ Paso 3: Crear el Corredor (Corridor Segment)

1. **Crear el segmento del corredor:**
   - Jerarquía: Click derecho → Create Empty
   - Renombrar a "CorridorSegment"
   - Position: `(0, 0, 0)`

2. **Crear el suelo:**
   - Click derecho en "CorridorSegment" → 3D Object → Cube
   - Renombrar a "Floor"
   - Position: `(0, -0.5, 10)`
   - Scale: `(10, 1, 20)`

3. **Crear paredes laterales:**
   - **Pared Izquierda:**
     - Click derecho en "CorridorSegment" → 3D Object → Cube
     - Renombrar a "WallLeft"
     - Position: `(-5, 1, 10)`
     - Scale: `(1, 3, 20)`
   
   - **Pared Derecha:**
     - Click derecho en "CorridorSegment" → 3D Object → Cube
     - Renombrar a "WallRight"
     - Position: `(5, 1, 10)`
     - Scale: `(1, 3, 20)`

4. **Convertir a Prefab:**
   - Arrastra "CorridorSegment" desde la Jerarquía a la carpeta `Assets/Prefabs/`
   - Elimina "CorridorSegment" de la Jerarquía (el prefab ya está guardado)

---

## 📦 Paso 4: Crear el Obstáculo (Obstacle Prefab)

1. **Crear el obstáculo:**
   - Jerarquía: Click derecho → 3D Object → Cube
   - Renombrar a "Obstacle"
   - Position: `(0, 0.5, 0)`
   - Scale: `(1, 1, 1)`

2. **Añadir componentes:**
   - Add Component → Rigidbody
     - Use Gravity: ✗ (desmarcar)
     - Is Kinematic: ✓ (marcar)
   - Add Component → Obstacle (el script que creamos)

3. **Añadir Tag "Obstacle":**
   - Tag → Add Tag → Crear "Obstacle"
   - Asignar tag "Obstacle" al objeto

4. **Convertir a Prefab:**
   - Arrastra "Obstacle" a `Assets/Prefabs/`
   - Elimina "Obstacle" de la Jerarquía

---

## 🎯 Paso 5: Configurar Game Manager

1. **Crear GameObject vacío:**
   - Jerarquía: Click derecho → Create Empty
   - Renombrar a "GameManager"
   - Position: `(0, 0, 0)`

2. **Añadir script:**
   - Add Component → Game Manager
   - Player: Arrastra "Player" desde la Jerarquía
   - UI Manager: (lo asignaremos después)
   - Difficulty Increase Rate: `1`

---

## 🏃 Paso 6: Configurar Corridor Manager

1. **Crear GameObject vacío:**
   - Jerarquía: Click derecho → Create Empty
   - Renombrar a "CorridorManager"
   - Position: `(0, 0, 0)`

2. **Añadir script:**
   - Add Component → Corridor Manager
   - Corridor Segment Prefab: Arrastra el prefab "CorridorSegment" desde `Assets/Prefabs/`
   - Segments Ahead: `5`
   - Segment Length: `20`
   - Player: Arrastra "Player" desde la Jerarquía

---

## 🚧 Paso 7: Configurar Obstacle Spawner

1. **Crear GameObject vacío:**
   - Jerarquía: Click derecho → Create Empty
   - Renombrar a "ObstacleSpawner"
   - Position: `(0, 0, 0)`

2. **Añadir script:**
   - Add Component → Obstacle Spawner
   - Obstacle Prefab: Arrastra el prefab "Obstacle" desde `Assets/Prefabs/`
   - Spawn Distance: `50`
   - Base Spawn Interval: `2`
   - Min Spawn Interval: `0.5`
   - Spawn Range X: `3`
   - Spawn Height: `0.5`
   - Min Scale: `(0.5, 0.5, 0.5)`
   - Max Scale: `(1.5, 2, 1.5)`

---

## 🎨 Paso 8: Configurar UI (Interfaz de Usuario)

### Crear Canvas Principal

1. **Crear Canvas:**
   - Jerarquía: Click derecho → UI → Canvas
   - Renombrar a "GameCanvas"
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: `1920 x 1080`

2. **Añadir Event System** (se crea automáticamente con el Canvas)

### Crear HUD (Pantalla de Juego)

1. **Crear Panel HUD:**
   - Click derecho en "GameCanvas" → UI → Panel
   - Renombrar a "HUD"
   - Rect Transform: Stretch (arriba a la izquierda)
   - Color: Transparente (Alpha = 0)

2. **Crear textos del HUD:**

   **Score Text:**
   - Click derecho en "HUD" → UI → Text - TextMeshPro
   - Renombrar a "ScoreText"
   - Rect Transform:
     - Anchor: Top Left
     - Position: `(20, -20, 0)` desde la esquina superior izquierda
   - Text: "PUNTUACIÓN: 0"
   - Font Size: `36`
   - Color: Blanco
   - Font Style: Bold

   **Distance Text:**
   - Duplicar "ScoreText" (Ctrl+D)
   - Renombrar a "DistanceText"
   - Position: `(20, -70, 0)`
   - Text: "DISTANCIA: 0m"

   **Time Text:**
   - Duplicar "ScoreText"
   - Renombrar a "TimeText"
   - Position: `(20, -120, 0)`
   - Text: "TIEMPO: 00:00"

   **Speed Text:**
   - Duplicar "ScoreText"
   - Renombrar a "SpeedText"
   - Anchor: Top Right
   - Position: `(-20, -20, 0)` desde la esquina superior derecha
   - Text: "VELOCIDAD: 0"
   - Alignment: Right

### Crear Game Over Panel

1. **Crear Panel:**
   - Click derecho en "GameCanvas" → UI → Panel
   - Renombrar a "GameOverPanel"
   - Rect Transform: Stretch completo
   - Color: Negro semi-transparente (R:0, G:0, B:0, A:200)

2. **Crear título "GAME OVER":**
   - Click derecho en "GameOverPanel" → UI → Text - TextMeshPro
   - Renombrar a "GameOverTitle"
   - Rect Transform:
     - Anchor: Top Center
     - Position: `(0, -150, 0)`
   - Text: "GAME OVER"
   - Font Size: `72`
   - Color: Rojo brillante
   - Alignment: Center
   - Font Style: Bold

3. **Crear textos de estadísticas:**

   **Final Score:**
   - Click derecho en "GameOverPanel" → UI → Text - TextMeshPro
   - Renombrar a "FinalScoreText"
   - Position: `(0, -300, 0)` (centrado)
   - Text: "PUNTUACIÓN FINAL: 0"
   - Font Size: `48`
   - Color: Amarillo
   - Alignment: Center

   **Final Distance:**
   - Duplicar "FinalScoreText"
   - Renombrar a "FinalDistanceText"
   - Position: `(0, -370, 0)`
   - Text: "DISTANCIA: 0 metros"
   - Color: Blanco

   **Final Time:**
   - Duplicar "FinalScoreText"
   - Renombrar a "FinalTimeText"
   - Position: `(0, -440, 0)`
   - Text: "TIEMPO: 00:00"
   - Color: Blanco

4. **Crear botón Restart:**
   - Click derecho en "GameOverPanel" → UI → Button - TextMeshPro
   - Renombrar a "RestartButton"
   - Position: `(0, -550, 0)`
   - Width: `300`, Height: `80`
   - Texto del botón: "REINICIAR"
   - Font Size: `36`
   - Color del botón: Verde brillante

5. **Desactivar GameOverPanel:**
   - Selecciona "GameOverPanel"
   - En el Inspector, desmarca la casilla arriba a la izquierda para desactivarlo

---

## 🔗 Paso 9: Conectar UI Manager

1. **Añadir UI Manager al GameCanvas:**
   - Selecciona "GameCanvas"
   - Add Component → UI Manager

2. **Asignar referencias:**
   - Score Text: Arrastra "ScoreText"
   - Distance Text: Arrastra "DistanceText"
   - Time Text: Arrastra "TimeText"
   - Speed Text: Arrastra "SpeedText"
   - Game Over Panel: Arrastra "GameOverPanel"
   - Final Score Text: Arrastra "FinalScoreText"
   - Final Distance Text: Arrastra "FinalDistanceText"
   - Final Time Text: Arrastra "FinalTimeText"
   - Restart Button: Arrastra "RestartButton"

3. **Conectar UI Manager con Game Manager:**
   - Selecciona "GameManager"
   - UI Manager: Arrastra "GameCanvas"

---

## 🎨 Paso 10: Crear Materiales Atractivos

### Material del Player (Bola)

1. **Crear material:**
   - En `Assets/Materials/`: Click derecho → Create → Material
   - Renombrar a "PlayerMaterial"

2. **Configurar:**
   - Shader: Standard
   - Albedo: Color azul neón brillante (R:0, G:150, B:255)
   - Metallic: `0.5`
   - Smoothness: `0.8`
   - Emission: Marcar y usar azul claro (R:0, G:100, B:200)

3. **Asignar al Player:**
   - Arrastra "PlayerMaterial" al objeto "Player"

### Material del Corredor

1. **Crear "CorridorMaterial":**
   - Color: Gris oscuro moderno (R:40, G:40, B:45)
   - Metallic: `0.3`
   - Smoothness: `0.6`

2. **Asignar al prefab:**
   - Abre el prefab "CorridorSegment"
   - Arrastra el material al Floor y las paredes

### Material de Obstáculos

1. **Crear "ObstacleMaterial":**
   - Color: Rojo/Naranja vibrante (R:255, G:80, B:0)
   - Metallic: `0.2`
   - Smoothness: `0.7`
   - Emission: Naranja (R:200, G:50, B:0)

2. **Asignar al prefab:**
   - Abre el prefab "Obstacle"
   - Arrastra el material al cubo

---

## 💡 Paso 11: Configurar Iluminación

1. **Directional Light:**
   - Selecciona "Directional Light" en la Jerarquía
   - Rotation: `(50, -30, 0)`
   - Color: Blanco cálido
   - Intensity: `1.5`

2. **Ambiente:**
   - Window → Rendering → Lighting
   - Environment:
     - Skybox Material: Default (o elige uno atractivo)
     - Ambient Color: Azul oscuro suave
     - Ambient Intensity: `1`

3. **Añadir luz adicional (opcional):**
   - Jerarquía: Light → Point Light
   - Posicionar cerca del Player
   - Color: Azul claro
   - Range: `15`
   - Intensity: `2`
   - Hacer hijo del Player para que lo siga

---

## ✅ Paso 12: Verificación Final

### Checklist de verificación:

- [ ] Player tiene Rigidbody y PlayerController
- [ ] Player tiene tag "Player"
- [ ] Camera tiene CameraController con referencia al Player
- [ ] Prefab CorridorSegment está creado
- [ ] Prefab Obstacle está creado con tag "Obstacle"
- [ ] GameManager tiene referencias a Player y UIManager
- [ ] CorridorManager tiene referencia al prefab y al Player
- [ ] ObstacleSpawner tiene referencia al prefab Obstacle
- [ ] UIManager tiene todas las referencias de UI asignadas
- [ ] GameOverPanel está desactivado al inicio
- [ ] Materiales están asignados y se ven atractivos

---

## 🎮 ¡A Jugar!

1. **Guardar la escena:** File → Save (Ctrl+S)

2. **Probar el juego:** Presiona el botón Play ▶️

3. **Controles:**
   - **A / Flecha Izquierda**: Mover a la izquierda
   - **D / Flecha Derecha**: Mover a la derecha

4. **Objetivo:**
   - Esquiva las cajas rojas
   - Sobrevive el mayor tiempo posible
   - La velocidad aumenta progresivamente
   - ¡Consigue la puntuación más alta!

---

## 🐛 Solución de Problemas

**Si TextMeshPro no aparece:**
- Window → TextMeshPro → Import TMP Essential Resources

**Si los obstáculos no aparecen:**
- Verifica que el prefab Obstacle esté asignado en ObstacleSpawner

**Si el juego no detecta colisiones:**
- Verifica que Player y Obstacle tengan los tags correctos
- Verifica que Obstacle tenga Rigidbody

**Si la UI no se muestra:**
- Verifica que todas las referencias en UIManager estén asignadas
- Verifica que GameOverPanel esté desactivado al inicio

---

¡Disfruta tu juego Roll a Ball! 🎉

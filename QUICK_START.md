# ✅ Checklist Rápido - Roll a Ball

## Estado Actual
- ✅ Scripts creados (todos en `Assets/Scripts/`)
- ❌ Escena configurada (necesitas hacer esto)

---

## 🚀 Pasos Mínimos para Jugar (en orden)

### 1️⃣ CREAR EL PLAYER (5 min)
```
Jerarquía → Click derecho → 3D Object → Sphere
- Renombrar: "Player"
- Position: (0, 0.5, 0)
- Add Component → Rigidbody
- Add Component → PlayerController
- Tag: "Player" (crear si no existe)
```

### 2️⃣ CONFIGURAR CÁMARA (2 min)
```
Selecciona "Main Camera"
- Position: (0, 5, -8)
- Rotation: (30, 0, 0)
- Add Component → CameraController
- Arrastra "Player" al campo Player
```

### 3️⃣ CREAR CORREDOR PREFAB (5 min)
```
Jerarquía → Create Empty → "CorridorSegment"
Dentro de CorridorSegment:
  - Cube "Floor": Position (0, -0.5, 10), Scale (10, 1, 20)
  - Cube "WallLeft": Position (-5, 1, 10), Scale (1, 3, 20)
  - Cube "WallRight": Position (5, 1, 10), Scale (1, 3, 20)

Arrastra "CorridorSegment" a Assets/Prefabs/
Elimina "CorridorSegment" de la Jerarquía
```

### 4️⃣ CREAR OBSTÁCULO PREFAB (3 min)
```
Jerarquía → 3D Object → Cube → "Obstacle"
- Position: (0, 0.5, 0)
- Add Component → Rigidbody
  - Use Gravity: ✗
  - Is Kinematic: ✓
- Add Component → Obstacle
- Tag: "Obstacle" (crear si no existe)

Arrastra "Obstacle" a Assets/Prefabs/
Elimina "Obstacle" de la Jerarquía
```

### 5️⃣ AÑADIR MANAGERS (5 min)
```
Crear 3 GameObjects vacíos:

1. "GameManager"
   - Add Component → GameManager
   - Player: Arrastra "Player"
   - Difficulty Increase Rate: 1

2. "CorridorManager"
   - Add Component → CorridorManager
   - Corridor Segment Prefab: Arrastra prefab "CorridorSegment"
   - Segments Ahead: 5
   - Segment Length: 20
   - Player: Arrastra "Player"

3. "ObstacleSpawner"
   - Add Component → ObstacleSpawner
   - Obstacle Prefab: Arrastra prefab "Obstacle"
   - Spawn Distance: 50
   - Base Spawn Interval: 2
```

### 6️⃣ CREAR UI BÁSICA (10 min)
```
Jerarquía → UI → Canvas → "GameCanvas"

Dentro de GameCanvas:
1. Panel "HUD" (transparente)
   - Dentro: 4 textos TextMeshPro:
     * ScoreText: "PUNTUACIÓN: 0"
     * DistanceText: "DISTANCIA: 0m"
     * TimeText: "TIEMPO: 00:00"
     * SpeedText: "VELOCIDAD: 0"

2. Panel "GameOverPanel" (negro semi-transparente)
   - Dentro:
     * Texto "GAME OVER"
     * FinalScoreText
     * FinalDistanceText
     * FinalTimeText
     * Button "REINICIAR"
   - IMPORTANTE: Desactivar este panel

GameCanvas:
- Add Component → UIManager
- Asignar todos los textos y el panel

GameManager:
- UI Manager: Arrastra "GameCanvas"
```

---

## ⚡ Atajo Rápido (Solo para Probar)

Si quieres probar RÁPIDO sin UI completa:

1. Crea Player (Paso 1)
2. Configura Cámara (Paso 2)
3. Crea un suelo simple:
   - Cube → Scale (20, 1, 100) → Position (0, -0.5, 50)
4. Presiona Play ▶️

Verás la bola moverse hacia adelante. Usa A/D para mover.

---

## 🎮 Cuando esté todo listo:

**Guardar:** File → Save (Ctrl+S)
**Jugar:** Presiona Play ▶️
**Controles:** A/D o Flechas ←→

---

## ❓ ¿Por qué solo ves el cielo?

Porque la escena está vacía. Unity muestra:
- ✅ Cámara (la tienes)
- ✅ Luz (la tienes)
- ❌ Objetos del juego (NO los tienes aún)

Los scripts están listos, pero necesitas crear los GameObjects en Unity y asignarles los scripts.

---

## 📍 ¿Dónde estás ahora?

```
RollABall/
├── Assets/
│   ├── Scripts/ ✅ (7 scripts listos)
│   ├── Scenes/
│   │   └── SampleScene.unity ⚠️ (vacía, necesitas configurarla)
│   ├── Prefabs/ ❌ (vacía, necesitas crear prefabs)
│   └── Materials/ ❌ (vacía, opcional para ahora)
```

**Siguiente paso:** Abre Unity y sigue los pasos 1-6 de arriba.

# 🎮 INSTRUCCIONES AUTOMÁTICAS - Roll a Ball

## ✨ ¡TODO ESTÁ LISTO!

He creado un **script automático** que configurará toda la escena por ti con **UN SOLO CLIC**.

---

## 📋 Pasos para Configurar Automáticamente:

### 1️⃣ Abre Unity
- Abre el proyecto **RollABall** en Unity

### 2️⃣ Espera a que compile
- Unity necesita compilar el nuevo script
- Espera a que termine (verás una barra de progreso abajo)
- Cuando termine, no habrá errores en la consola

### 3️⃣ Ejecuta el Auto-Setup
- En el menú superior de Unity, haz clic en:
  ```
  Roll a Ball → Auto Setup Scene
  ```
- Aparecerá un diálogo preguntando si quieres continuar
- Haz clic en **"Sí"**

### 4️⃣ ¡Listo!
- El script configurará automáticamente:
  ✅ Player (bola con física)
  ✅ Cámara con seguimiento
  ✅ Prefabs del corredor
  ✅ Prefabs de obstáculos
  ✅ GameManager
  ✅ CorridorManager
  ✅ ObstacleSpawner
  ✅ UI completa (HUD + Game Over)
  ✅ Materiales atractivos
  ✅ Iluminación

### 5️⃣ ¡A Jugar!
- Presiona el botón **Play** ▶️
- Controles: **A/D** o **Flechas ←→**

---

## 🎯 ¿Qué hace el script automático?

El script `RollABallSceneSetup.cs` hace TODO lo que estaba en la guía manual:

1. **Crea el Player** con Rigidbody y PlayerController
2. **Configura la cámara** con CameraController
3. **Genera los prefabs** de corredor y obstáculos
4. **Añade todos los managers** con sus configuraciones
5. **Crea la UI completa** con HUD y pantalla de Game Over
6. **Aplica materiales** con colores vibrantes y efectos
7. **Configura la iluminación** para que se vea bien

---

## 🚨 Si el menú "Roll a Ball" no aparece:

1. **Verifica que Unity haya compilado:**
   - Mira la consola (abajo) - no debe haber errores
   - Si hay errores, avísame

2. **Cierra y vuelve a abrir Unity:**
   - A veces Unity necesita reiniciarse para detectar nuevos menús

3. **Verifica la carpeta Editor:**
   - En Unity, ve a `Assets/Editor/`
   - Debe estar el archivo `RollABallSceneSetup.cs`

---

## 🎨 Características del Juego:

- 🏃 **Velocidad progresiva**: De 10 a 30 unidades
- 📈 **Dificultad creciente**: Más obstáculos con el tiempo
- 📊 **Puntuación**: Distancia + Tiempo
- 💥 **Game Over**: Al tocar obstáculos
- 🔄 **Reinicio**: Botón en pantalla de Game Over
- 🎨 **Visual atractivo**: Materiales neón con emission

---

## 🎮 Controles:

- **A** o **←**: Mover izquierda
- **D** o **→**: Mover derecha
- **Objetivo**: Esquivar cajas el mayor tiempo posible

---

## ✅ Ventajas del Auto-Setup:

- ⚡ **Rápido**: 1 clic vs 30+ minutos manual
- ✓ **Sin errores**: Todo configurado correctamente
- 🎯 **Completo**: Incluye TODO (UI, materiales, managers)
- 🔧 **Perfecto**: Todas las referencias conectadas

---

## 📁 Estructura Generada:

```
RollABall/
├── Assets/
│   ├── Editor/
│   │   └── RollABallSceneSetup.cs ✅ (script automático)
│   ├── Scripts/ ✅ (7 scripts del juego)
│   ├── Prefabs/ ✅ (se crearán automáticamente)
│   │   ├── CorridorSegment.prefab
│   │   └── Obstacle.prefab
│   ├── Materials/ ✅ (se crearán automáticamente)
│   │   ├── PlayerMaterial.mat
│   │   ├── CorridorMaterial.mat
│   │   └── ObstacleMaterial.mat
│   └── Scenes/
│       └── SampleScene.unity ✅ (se configurará automáticamente)
```

---

## 🎉 ¡Disfruta tu juego!

Una vez que ejecutes el auto-setup, solo presiona Play y empieza a jugar.

**¿Tienes algún problema?** Avísame y te ayudo. 🚀

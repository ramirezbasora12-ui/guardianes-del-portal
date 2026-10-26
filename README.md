# Guardianes del Portal — Bosque Primordial

Proyecto fuente Unity 2D independiente para Android. **Implementación parcial, pendiente de abrir, compilar y validar en Unity.** No incluye jugabilidad de combate ni un APK validado.

## Compilar desde un teléfono Android

No necesitas un PC Windows. Está preparado un workflow de GitHub Actions con GameCI, Unity 2022.3.62f1, generación, pruebas y APK. El usuario ya creó y autorizó https://github.com/ramirezbasora12-ui/guardianes-del-portal. La ejecución requiere una licencia Unity válida en secretos de GitHub; consulta el informe para el estado real de ejecución. Todavía no hay APK validada.

Sigue [la guía desde Android](Documentation/CLOUD_BUILD_ANDROID.md) y revisa [el estado real del pipeline](CLOUD_PIPELINE_REPORT.md).

## Abrir

1. Instalar Unity **2022.3.62f1 LTS**, o una revisión compatible 2022.3, desde Unity Hub. Añadir Android Build Support, SDK/NDK y OpenJDK.
2. Añadir esta carpeta como proyecto. La primera resolución de paquetes requiere conexión a Unity Package Manager; el juego diseñado no requiere conexión durante la ejecución.
3. Esperar a la compilación de scripts. La inicialización configura Android e importa las hojas originales. Si Unity solicita reiniciar por Input System, aceptar y volver a abrir.
4. La inicialización genera la composición editable en `Assets/Guardianes/WorldMap/Scenes/World1.unity`. Si no termina, ejecutar los comandos **Guardianes > 1**, **2** y **3** en ese orden.
5. Abrir esa escena y pulsar Play. El primer nivel está disponible. Los nueve restantes requieren resultados reales de niveles anteriores.

La escena incluida inicialmente es una escena de arranque con el controlador; la herramienta del Editor la expande y guarda los objetos UI independientes. **No se ha ejecutado esa generación en este entorno.** En modo batch la generación se realiza expresamente por `CloudBuild.PrepareForTests` o por el fixture de pruebas. Antes de regenerar se conserva una copia de la escena en `SceneBackups/`. El controlador también construye la interfaz en ejecución si no encuentra un Canvas.

## Controles y sistemas

- Tocar un nivel disponible abre su ficha. Jugar carga la escena indicada en `World1.asset`; sin escena válida muestra **EN DESARROLLO**, sin conceder progreso.
- Arrastrar el mapa y pellizcar para zoom 1–1,65×. En escritorio: rueda y arrastre. La barra lateral y los controles superiores permanecen fijos.
- Cofres a 10, 20 y 30 estrellas. Las recompensas se configuran en `World1.asset`, incluido como ScriptableObject editable.
- Ajustes: música, efectos, tres perfiles gráficos, indicación de idioma español y regreso al mapa. No hay traducciones adicionales ni pantalla de menú general independiente.
- Guardado local en `Application.persistentDataPath/guardianes-v1.json`; SHA-256 para detectar corrupción, archivo temporal, copia `.bak` y preservación `.corrupt`. No es un sistema antitrampas.

## Integración de niveles

Asignar diez nombres de escenas opcionales en `gameplayScenes` del catálogo y añadir las escenas al Build Settings. Un resultado legítimo se comunica mediante:

```csharp
worldMapController.SubmitLevelResult(numeroDeNivel, estrellas); // 1–10; 1–3
```

Conservar o recuperar el controlador al volver al mapa. El método rechaza niveles bloqueados. Actualmente el cargador cambia de escena; una implementación futura de combate necesitará su servicio de resultados y retorno. No hay botones de victoria de desarrollo en la interfaz de producción. Las pruebas NUnit sí ejercitan el progreso sin inventar jugabilidad.

## Validación y compilación

- **Window > General > Test Runner > EditMode > Run All** ejecuta las pruebas incluidas.
- **Guardianes > 4** comprueba datos y referencias principales.
- **Guardianes > 5** captura una imagen real, exclusivamente en Play Mode, en `Capturas/`.
- **Guardianes > 6** compila `Builds/GuardianesDelPortal.apk` si Android Build Support y sus dependencias están instalados.
- En modo batch, desde esta carpeta:

```bash
Unity -batchmode -projectPath "$PWD" -executeMethod Guardianes.Editor.World1Builder.BatchGenerate -quit -logFile generacion.log
Unity -batchmode -projectPath "$PWD" -runTests -testPlatform EditMode -testResults resultados.xml -logFile pruebas.log
```

El nombre/ruta del ejecutable Unity depende de su instalación. No ejecutar comandos de construcción concurrentemente sobre el mismo proyecto.

Consultar `WORLD_1_IMPLEMENTATION_REPORT.md` para los límites reales y `Documentation/ASSET_INVENTORY.md` para el inventario artístico.

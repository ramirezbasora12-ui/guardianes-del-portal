# Procedencia de activos

- `forest.png`, `ui.png`, `worlds.png`: generados originalmente durante esta sesión mediante la herramienta de imágenes; no descargados de bancos ni extraídos de juegos. `ui.png` recibió una edición de transparencia por la misma herramienta.
- Los archivos fuente generados se conservaron en `/workspace/generated_images`; las copias del proyecto mantienen los PNG íntegros. Unity realiza el recorte como metadatos de importación.
- Los dos WAV se sintetizaron durante esta sesión; no contienen muestras de terceros.
- `LegacyRuntime.ttf` lo proporciona Unity como recurso integrado; no se redistribuye una fuente externa.
- La referencia del usuario se usó para análisis de composición y no se incluyó en el paquete del juego.
- Paquetes Unity/Input System/UGUI/Test Framework: dependencias externas de Unity Package Manager, sujetas a sus licencias. La primera importación necesita descargarlos o tener caché local. La app no usa red para progresión, saldos o recompensas.

No se asigna una licencia de terceros inventada a los activos generados. Revisar las condiciones aplicables de las herramientas y distribución antes de publicar comercialmente.

Las cuatro hojas `environment_*.png` también fueron generadas originalmente durante la continuación de esta sesión, usando `forest.png` como referencia propia. Se copiaron intactas al proyecto, sin reconstrucción programática de ilustraciones. Sus regiones UV y animaciones se describen en `EnvironmentLayers.json`.

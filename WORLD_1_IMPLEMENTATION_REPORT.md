# Mundo 1 — Informe actual de implementación

Fecha: 26 de octubre de 2026. **Fase incompleta: sin validación real Unity, capturas del motor ni APK.** El informe inicial se conserva en `Documentation/History/`.

## Terminado a nivel de fuente y activos preparados

- Proyecto independiente y base ilustrada original conservada sin sustituirla por geometría.
- Nuevas hojas transparentes originales de agua, vegetación, magia y partículas: 16 regiones de efectos. `EnvironmentLayers.json` define 20 capas y sus posiciones, opacidades, calidad y animación. Se integran en el generador y en la construcción de UI en ejecución.
- Cascadas/espuma/reflejos/corrientes, ramas/helechos/vegetación corrupta/niebla, cristales, energía azul/violeta/dorada y pool de hasta 18 luciérnagas/destellos. Se usan 14 tipos de las 16 regiones; hoja/runa quedan preparados para iteración posterior.
- Shader con deformación UV acotada a cada región para evitar mezclar efectos vecinos. Materiales compartidos por tipo, variación de intensidad por calidad, partículas preasignadas y UI decorativa sin interceptar toques.
- Salida de mundo como capa de energía dorada/cian, oculta hasta completar al jefe; informa de Mundo 2 en desarrollo, sin inventar niveles jugables.
- Diez nodos, tres estrellas por nodo, estados, cofres, saldos, ajustes/audio, gestos y guardado. El almacenamiento ahora admite un codec explícito; Unity usa JsonUtility y las pruebas fuera del motor usan System.Text.Json sobre el mismo código de almacenamiento.
- Corrección real de recuperación de archivos dañados: faltaba capturar `InvalidDataException`, descubierto mediante las pruebas ejecutables.
- Herramientas de Editor y pipeline GitHub Actions con Unity 2022.3.62f1, generación, pruebas y construcción APK. Su ejecución remota sigue bloqueada por repositorio/licencia.

Estos elementos están escritos o producidos como archivos; **no equivalen a una escena renderizada y comprobada**.

## Pruebas ejecutadas

- Compilación .NET 8 del modelo y almacenamiento real más harness independiente: 33/33 casos aprobados localmente y en GitHub Actions (ejecución 40264605060). Incluyen diez niveles, bloqueos, máximos, hitos 0/9/10/19/20/29/30, reclamo único, monedas exactas, overflow, guardado/carga y corrupción. No compila Unity ni evalúa su serializer.
- 44 comprobaciones estáticas aprobadas: recursos, estructura, regiones UV, enlaces de capas, metadatos y declaraciones. Sintaxis C# sin errores en 24 archivos, sin comprobación de tipos Unity.
- actionlint sin errores en el workflow. Scripts Python compilados sintácticamente.
- Inspección de imágenes y alfa; verificación de preservación de activos base frente al ZIP anterior.
- Comprobación de imagen Docker GameCI disponible, no ejecución del contenedor/editor.

Resultados: `Validation/core-results.json`, `Validation/static-results.json` y `Validation/asset-preservation.json`.

## Preparado pero requiere pruebas en Unity

- Importación de PNG, recortes UI existentes, texturas de efectos y materiales/shaders.
- Generación y serialización real de la jerarquía editable; la `.unity` entregada sigue siendo de arranque hasta ejecutar el Editor.
- Compilación de ensamblados de runtime/Editor/tests y resolución de paquetes Unity.
- Tests EditMode y PlayMode de referencias, diez botones, estrellas, cofres, guardado con JsonUtility, barra lateral y ajustes.
- Renderizado de efectos, máscaras, proporciones, posiciones y legibilidad de nodos. Los valores de colocación se calibraron sobre la pintura, no sobre una captura Unity.
- Capturas a dos resoluciones desde PlayMode con Camera.Render/RenderTexture. Deben revisarse también porque el Canvas cambia temporalmente a ScreenSpaceCamera para la captura.
- Build Android IL2CPP, firma de depuración, instalación, multitáctil, funcionamiento offline, safe area, memoria y rendimiento en dispositivo real de 4 GB.

## Pendiente de arte y funcionalidad final

- Los grandes árboles, troncos, rocas, ruinas, puentes, cielo y montañas siguen pintados en la base. No hay separación completa de primer plano/medio/fondo ni paralaje final.
- El agua original sigue pintada; las nuevas capas complementan su movimiento. No se ha borrado ni reconstruido el terreno ni se afirma una extracción perfecta de la referencia.
- Revisar las hojas generadas: no cumplen márgenes/cuadrícula perfectos y algunas puntas/halos llegan al borde. Las regiones están definidas expresamente; corregir costuras y posibles recortes después de ver el render real.
- Ajustar el encaje exacto de portales/cristales y los recubrimientos de agua/vegetación. La salida al Mundo 2 ahora tiene una capa propia, pero no cuenta con nueva arquitectura de piedra independiente.
- Falta logotipo ilustrado, animación de tapa de cofres, sonido final, menú principal y selección real de idiomas adicionales. Solo español.
- Sin jugabilidad de niveles, combate, enemigos o guardianes; se mantiene pantalla EN DESARROLLO cuando falta escena jugable.
- Sin URP 2D ni iluminación 2D validada: se conserva Built-in + UGUI. No se ha medido rendimiento ni completado optimización.

## Diferencias visuales con la referencia

La pintura original nueva sigue siendo más panorámica y asciende hacia la esquina superior derecha, frente a la ruta más horizontal y compacta de la referencia. Se preservan su paleta y detalle y las capas añadidas usan esa misma pintura como guía de estilo. No se ha logrado ni declarado equivalencia de composición exacta.

Las nuevas cascadas, cristales y follaje son ilustraciones detalladas independientes, no figuras geométricas finales. Su integración puede alterar densidad/oclusión y necesita corrección con screenshots reales. Los saldos y bloqueos iniciales reflejan datos nuevos, no el 30/30 de la imagen de ejemplo.

## Estado de nube y Android

No había repositorio Git; se inicializó uno local. GitHub permite consultar repositorios autorizados, pero rechazó crear el repositorio dedicado con `Resource not accessible by integration (createRepository)`. No se tocó NEXARA ni proyectos ajenos. El usuario creó después el repositorio dedicado, se conectó `origin`, se hicieron commits y push, y GitHub Actions ejecutó el workflow real. La validación de archivos (44) y las pruebas C# (33) pasaron; el trabajo Unity falló en preflight por credenciales incompletas, antes de abrir el motor. Los artefactos reales están conservados en `Validation/CloudRun-40264605060/`. Estado y nuevas ejecuciones en `CLOUD_PIPELINE_REPORT.md`.

**APK no construida; screenshots Unity no producidos; Mundo 1 no completado.** Consultar `CLOUD_PIPELINE_REPORT.md` y `Documentation/CLOUD_BUILD_ANDROID.md` para pasos desde Android y requisitos de autorización.

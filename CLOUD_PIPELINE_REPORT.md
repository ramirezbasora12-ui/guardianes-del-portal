# Informe de pipeline en nube

Fecha: 26 de octubre de 2026.

## Configurado en archivos

- Git local independiente, rama `main`; no existían remoto ni workflows previos.
- `.github/workflows/unity-android.yml`, acciones fijadas por SHA, Unity **2022.3.62f1** y contenedor Android fijado por digest verificado.
- Validación estática y pruebas .NET del código real de modelo/almacenamiento, separadas del motor.
- Fixture EditMode para configurar Android, importar recursos, generar escena, comprobar referencias y escribir informe.
- Pruebas PlayMode de diez botones, 30 estrellas, bloqueos, cofres persistentes, barra lateral y ajustes.
- Intento de capturas reales con renderizador Unity, con fallo explícito si no hay GPU/renderizador. No usa imágenes generadas como capturas.
- Método de construcción APK ARM64/IL2CPP con firma de depuración, informe de build y comprobación estructural del APK.
- Artefactos de logs, tests, escena generada, PNG reales si se producen y APK únicamente si se construye.
- Comprobación de secretos, saneado de logs, exclusión de archivos de licencia y credenciales. Sin omitir activación.

## Ejecutado realmente en este entorno

- Inspección de proyecto, cuenta/repositorios accesibles y ausencia de repositorio de Guardianes.
- Consulta a Docker Hub: existen las imágenes de Unity 2022.3.62f1 para Android y Linux IL2CPP. No se descargaron ni ejecutaron como Unity.
- Consulta de entradas de acciones GameCI y de su documentación oficial de licencias en GitHub.
- Instalación local de SDK .NET 8.0.427 y compilación de los archivos reales `PlayerData`, `RewardDefinition`, `SaveService`, `SaveCodec` con un ejecutable de pruebas separado.
- **33/33 pruebas de modelo y almacenamiento correctas**. Se usa System.Text.Json como codec de prueba; Unity JsonUtility sigue pendiente de ejecutar en Unity.
- Primera ejecución: dos fallos al recuperar datos dañados. Se corrigió la captura de `InvalidDataException`; la segunda ejecución pasó los 33 casos.
- **43/43 comprobaciones estáticas**, sintaxis sin errores en 24 archivos C# (incluye harness), actionlint sin errores y compilación sintáctica de scripts Python.
- Cuatro hojas originales nuevas, 16 regiones VFX y 20 capas ambientales preparadas; pintura base preservada.

## Fallos y bloqueos reales

El primer intento de creación automática fue rechazado con **Resource not accessible by integration**. El usuario creó después `ramirezbasora12-ui/guardianes-del-portal` y facilitó su URL. Se verificó vacío, público y con permiso de escritura; no había workflows previos. Se conectó como remoto `origin`. El push y la ejecución se registrarán después de realizarlos. No se usó NEXARA ni otro repositorio como sustituto.

La integración no permite consultar la configuración de Actions ni listar secretos de este repositorio: ambas operaciones devolvieron HTTP 403. No se conoce por esa vía si los secretos remotos están presentes; el paso de preflight del workflow lo determinará sin revelar valores.

La página pública `game.ci` devolvió HTTP 403; fue posible consultar la documentación oficial equivalente en el repositorio `game-ci/documentation`.

No hay Unity instalado ni licencia configurada/activada en este entorno. No se ha ejecutado compilación del proyecto en Unity, EditMode/PlayMode, renderizado, Gradle, instalación Android o medición de rendimiento.

## Requiere tu intervención o autorización

1. Repositorio dedicado creado por el usuario y acceso de escritura comprobado. URL: https://github.com/ramirezbasora12-ui/guardianes-del-portal. No requiere reenviar tokens ni volver a crearlo.
2. Configurar en GitHub una licencia válida y sus credenciales Unity: `UNITY_LICENSE` + `UNITY_EMAIL` + `UNITY_PASSWORD`, o `UNITY_SERIAL` + correo + contraseña para la ruta profesional. No pegarlos en el chat.
3. Si no dispones de licencia exportable y solo usas Android, elegir y autorizar un medio legítimo de activación: estación Linux remota con Hub o servicio Unity Cloud apropiado. No se ha contratado ni provisionado ningún servicio.
4. Autorizar Actions si está deshabilitado y confirmar disponibilidad de cuota. Después se podrá disparar y observar una ejecución real.

Pasos detallados, íntegramente utilizables desde navegador Android: `Documentation/CLOUD_BUILD_ANDROID.md`.

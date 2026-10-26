# Compilar desde Android, sin PC Windows

## Estado comprobado

El proyecto se inspeccionó en `/workspace/GuardianesDelPortal`. No tenía `.git`, remoto ni workflows anteriores. La cuenta conectada es `ramirezbasora12-ui`; los repositorios accesibles de otros proyectos no se tocaron. Se inicializó un repositorio Git local independiente.

La creación de `ramirezbasora12-ui/guardianes-del-portal` privado fue **rechazada por GitHub**: `Resource not accessible by integration (createRepository)`. Después de ese rechazo, el usuario creó y autorizó `ramirezbasora12-ui/guardianes-del-portal`, configurado como público. Ya se conectó el remoto `origin`. El estado de push y ejecución se recoge en `CLOUD_PIPELINE_REPORT.md`. No es un fallo de Unity ni prueba de que la APK compile.

El workflow preparado es `.github/workflows/unity-android.yml`. Está validado con actionlint. Las imágenes GameCI de Unity 2022.3.62f1 para Android y Linux fueron consultadas en Docker Hub; la imagen Android se fija por digest. Verificar su existencia no equivale a ejecutar Unity.

## 1. Repositorio autorizado

El usuario ya completó este paso: https://github.com/ramirezbasora12-ui/guardianes-del-portal. Está vacío al inspeccionarlo y no contenía workflows. Los pasos siguientes sirven de referencia para crear otro repositorio dedicado si fuera necesario.

### Creación desde Android

1. En el navegador Android, iniciar sesión en GitHub y abrir https://github.com/new.
2. Propietario: tu cuenta. Nombre recomendado: `guardianes-del-portal`. Visibilidad: **Private**. Dejar sin README, sin `.gitignore` y sin licencia inicial, porque el proyecto ya los incluye o define su propio contenido.
3. Crear el repositorio y copiar su URL. Comunicar la URL a Codex; no es necesario subir el ZIP ni copiar archivos manualmente.
4. Si la integración usa repositorios seleccionados, abrir https://github.com/settings/installations, localizar la integración GitHub utilizada por Codex/ChatGPT, pulsar **Configure** y añadir el nuevo repositorio. Si se administra como conexión OAuth o desde la app, actualizar la conexión para incluirlo. No modificar permisos de NEXARA.
5. Codex podrá comprobar el acceso, añadir el remoto y subir los commits. Si GitHub deniega archivos de `.github/workflows`, habrá que autorizar escritura de workflows para esa integración o crear ese archivo mediante el editor web de GitHub; no enviar tokens por el chat.

Este paso requiere tu intervención porque la integración actual no tiene permiso `createRepository`. El commit local y el paquete fuente ya pueden prepararse sin ella.

## 2. Licencia de Unity: requisito independiente del repositorio

GameCI debe activar Unity legalmente. GitHub no proporciona automáticamente una licencia de Unity. No se han creado, copiado de otros proyectos ni inventado credenciales.

### Opción A — Cuenta con licencia Personal válida y archivo `.ulf`

GameCI documenta estos secretos:

| Secreto | Contenido |
|---|---|
| `UNITY_LICENSE` | Texto completo de un archivo de licencia `.ulf` obtenido por activación legítima de Unity Hub |
| `UNITY_EMAIL` | Correo de la cuenta Unity correspondiente |
| `UNITY_PASSWORD` | Contraseña de esa cuenta Unity |

La guía actual de GameCI parte de una activación de Unity Hub y advierte que las opciones de la interfaz pueden variar. **Un teléfono Android por sí solo no ejecuta Unity Hub ni garantiza obtener ese archivo.** No se promete un flujo web antiguo `.alf`→`.ulf` que ya no esté disponible.

Si no tienes un `.ulf` válido y solo dispones del teléfono, puedes autorizar una estación Linux remota con escritorio accesible por navegador: instalar Unity Hub oficial allí, iniciar tú la sesión, aceptar las condiciones y activar Personal si tu cuenta es elegible. No hace falta Windows. En Linux, GameCI indica `~/.local/share/unity3d/Unity/Unity_lic.ulf`. Comprobar que Hub realmente genera el archivo; si no existe esa opción, consultar a Unity/GameCI. Ese servicio remoto puede requerir cuenta y coste; no se ha contratado ni provisionado ninguno. Tampoco se ha instalado Hub automáticamente ni iniciado sesión en tu nombre.

### Opción B — Licencia profesional que permite activación por serial

Añadir `UNITY_SERIAL`, `UNITY_EMAIL` y `UNITY_PASSWORD`, obtenidos legítimamente de tu suscripción Unity. Se utiliza esta opción **en lugar de** `UNITY_LICENSE`; no mezclar las dos rutas. Verificar disponibilidad de activación CI y puestos con Unity. No se ha comprado ninguna suscripción.

### Alternativa con administración web

Si la activación Personal no está disponible sin Hub, evaluar Unity Build Automation desde Unity Cloud: crear organización/proyecto, conectar este repositorio, elegir Android y Unity compatible, definir `Guardianes.Editor.CloudBuild.PrepareForTests` como generación previa y configurar pruebas/construcción según el servicio. Requiere autorizar la conexión y revisar su plan/cuotas/licenciamiento. No se afirma que esté contratado ni configurado, ni que todos sus pasos sean iguales a GitHub Actions. La configuración entregada y validada es la de **GameCI en GitHub Actions**.

Documentación consultada: https://github.com/game-ci/documentation/blob/main/docs/03-github/02-activation.mdx. La web `game.ci` devolvió HTTP 403 en este entorno; se consultó el documento oficial del mismo proyecto en GitHub.

## 3. Añadir secretos desde Android

1. Abrir el repositorio → **Settings → Secrets and variables → Actions → New repository secret**. Si el menú no aparece bien, activar «Sitio para ordenadores» en el navegador móvil.
2. Crear los tres secretos de una de las rutas anteriores. Pegar los valores solo en GitHub, nunca en issues, código o mensajes.
3. No añadir archivos `.ulf`, `.alf`, `.keystore`, `.jks` ni `.env` al repositorio. Están excluidos en `.gitignore`.
4. En **Settings → Actions → General**, permitir Actions y el uso de las acciones `actions/*` y `game-ci/*` incluidas. Verificar que hay cuota/minutos disponibles; el runner solicitado es `ubuntu-22.04`. No se han contratado minutos ni servicios de pago.

El workflow solo comprueba que los nombres tienen valores; la aceptación real de la licencia la realiza Unity/GameCI. Si faltan, falla antes de descargar la imagen grande y publica un estado explícito. No cambia a `skipActivation` ni evita la licencia.

## 4. Ejecutar y descargar desde el teléfono

1. En **Actions**, abrir **Unity Android — Bosque Primordial**.
2. Pulsar **Run workflow**, seleccionar la rama y dejar «Intentar capturas…» activado para la primera prueba. Los cambios en `main` también disparan automáticamente el flujo para código/activos/configuración.
3. El trabajo **Archivos y pruebas C# sin Unity** compila y ejecuta lógica pura con .NET. No sustituye las pruebas del motor.
4. El trabajo **Unity real** activa Unity, compila los scripts al abrir el proyecto, genera/valida la escena mediante el fixture EditMode, ejecuta pruebas EditMode y PlayMode y construye una APK ARM64 de desarrollo con IL2CPP. Cada paso debe terminar correctamente antes del siguiente.
5. Al terminar, abrir **Artifacts** y descargar:
   - `validacion-codigo-sin-unity`: resultados JSON de estructura y lógica.
   - `unity-registros-pruebas-escena`: logs, informes NUnit XML, informe de generación, escena generada, informe de build y capturas si realmente se generaron.
   - `guardianes-android-apk`: APK, solo cuando exista y los pasos hayan tenido éxito.
6. Extraer la APK en Android. Para instalarla, Android puede pedir permiso a tu navegador/gestor de archivos para instalar aplicaciones de esa fuente. La APK usa firma de depuración y no es una publicación en Play Store. Aún debe probarse en el dispositivo, sin red, y registrar errores.

Los artefactos se conservan 14 días. La primera descarga/importación puede tardar bastante: la imagen Android consultada ocupa aproximadamente 6,84 GB comprimida. Si el runner se queda sin espacio o memoria, conservar el log y ajustar recursos. No se ha demostrado aún la duración ni la compatibilidad completa.

## 5. Capturas: qué se intenta y qué no se afirma

Las pruebas PlayMode cargan **la escena real**, ejecutan sus scripts y usan `Camera.Render` sobre `RenderTexture` a 1920×1080 y 2400×1080. Para incluir UGUI, cambian temporalmente el Canvas de Overlay a ScreenSpaceCamera. El archivo `procedencia.txt` documenta Unity y el renderizador utilizado.

El script oficial de GameCI Test Runner inspeccionado no impone `-nographics` en la llamada EditMode/PlayMode al Editor. Esto hace razonable intentar renderizado con el entorno gráfico del contenedor, pero **no prueba que funcione**. Si `SystemInfo.graphicsDeviceType` es Null, la prueba falla y no inventa capturas. Si falla exclusivamente la captura, se puede repetir manualmente con esa opción desactivada: las pruebas funcionales siguen ejecutándose y queda constancia de la prueba visual omitida.

Estos PNG serían renders reales de Unity en nube, **no capturas de un Android físico**. Las ilustraciones de `Resources/Art` nunca se publican como screenshots. En este entorno todavía no se ha producido ningún PNG de ejecución Unity.

## Diagnóstico por etapas

| Falla | Interpretación y siguiente paso |
|---|---|
| Crear repositorio | Falta permiso de la integración; crear y conceder acceso desde el navegador |
| Trabajo source | Revisar JSON de validación y salida .NET; no culpar a licencia Unity |
| Credenciales ausentes | Añadir secretos; el motor no se ejecutó |
| Activación rechazada | Revisar licencia, cuenta, suscripción y puestos con Unity; no saltarse activación |
| Compilación C# o UPM | Revisar log EditMode; corregir código/dependencias y repetir |
| Generación/referencias/shader | Consultar fallo de fixture y recursos; no tratar la escena como validada |
| PlayMode | Revisar el caso concreto de botones, cofres o captura |
| Gradle/IL2CPP/SDK | Revisar build.log y registros completos de Actions; corregir antes de afirmar APK válida |
| APK descargada pero no instala | Recoger mensaje Android, versión y dispositivo; la estructura ZIP no demuestra instalación |

Los registros del motor se saneen antes de subirse como artefactos. Nunca se suben archivos de activación. Los logs completos de cada paso también están en **Actions → ejecución → Download log archive**.

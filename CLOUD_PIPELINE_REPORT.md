# Resultado de compilación en nube — Guardianes del Portal

Fecha: 26 de octubre de 2026.

## Estado actual

Repositorio real: https://github.com/ramirezbasora12-ui/guardianes-del-portal, configurado como **público** por su creador. Proyecto y workflow subidos a `main`. No se modificó NEXARA ni ningún repositorio ajeno.

Ejecución comprobada: https://github.com/ramirezbasora12-ui/guardianes-del-portal/actions/runs/40264605060, commit `0ac09553c61c31cea63ef26fa968c3e49748835a`.

| Etapa | Resultado real |
|---|---|
| Checkout, Python y .NET en GitHub Actions | Correcto |
| Validación estática de archivos/recursos/sintaxis | **44/44 comprobaciones correctas en nube** |
| Compilación y pruebas del modelo/almacenamiento C# con .NET | **33/33 casos correctos en nube** |
| Preflight de credenciales Unity | **Fallo: faltan secretos requeridos** |
| Apertura/compilación del proyecto en Unity | No ejecutada |
| Generación real de escena, EditMode y PlayMode | No ejecutados |
| Capturas reales de Unity | No producidas |
| APK Android e instalación en teléfono | No construida / no probada |

El resultado global de esa ejecución es **failure**, por el bloqueo de credenciales. No debe presentarse como una compilación Unity exitosa.

Los artefactos reales de esa ejecución se recuperaron y conservaron en `Validation/CloudRun-40264605060/`: resultados C#, resultados estáticos, metadatos de trabajos y el texto de bloqueo de licencia. `unity-registros-pruebas-escena` contiene en esta ejecución únicamente el estado de bloqueo; no contiene logs de un motor que no se abrió ni una escena generada.

## Configurado y subido

- Workflow `.github/workflows/unity-android.yml`, activación por push en `main` y ejecución manual desde Android.
- Acciones fijadas por SHA, Unity **2022.3.62f1**, imagen Android GameCI fijada por digest verificado en Docker Hub. El registro también confirmó una imagen Linux IL2CPP; no se descargaron ni ejecutaron aquí.
- Módulos Unity usados por audio, JSON, imágenes, captura y UI declarados explícitamente. SpriteAtlas se configura mediante las APIs del Editor y no mediante una dependencia separada no verificada `com.unity.2d.spriteatlas`.
- Compilación de scripts al abrir Unity, generación/validación por fixture EditMode y artefacto de escena editable.
- Tests PlayMode de diez botones, estrellas, bloqueos, cofres persistentes, barra lateral y ajustes.
- Intento de captura mediante render real Unity a 1920×1080 y 2400×1080; falla si el renderizador es Null. No usa ilustraciones como screenshots.
- Construcción Android ARM64/IL2CPP de desarrollo con firma de depuración y comprobación estructural de APK. No publicación en Play Store.
- Artefactos separados de resultados, logs, escena, capturas si se producen y APK solo si existe. Retención de 14 días.
- Comprobación de credenciales antes de descargar el contenedor, saneado de logs y exclusión de archivos de licencia/keystore. No se omite activación.

Tras la ejecución comprobada se ajustaron las dependencias y el contenido del artefacto de validación en el commit `a494d5b`. Esto disparó otra ejecución: https://github.com/ramirezbasora12-ui/guardianes-del-portal/actions/runs/40265035976. **También terminó con el trabajo de código correcto y el trabajo Unity bloqueado en preflight por credenciales incompletas.** Sus metadatos reales están en `Validation/CloudRun-40265035976/jobs.json`. No hubo apertura de Unity en ninguna de las dos ejecuciones comprobadas.

## Incidencias encontradas y corregidas

1. El proyecto no tenía Git, remoto ni workflows. La creación automática del repositorio fue denegada con `Resource not accessible by integration (createRepository)`. El usuario creó el repositorio dedicado y facilitó acceso; después se realizaron commits y push reales.
2. Las primeras dos ejecuciones remotas fallaron al validar cinco carpetas vacías que Git no conserva. Se añadieron documentos de organización y sus metadatos; la siguiente ejecución superó la etapa. También se detectó y corrigió una regla heredada de `.gitignore` que excluía el `.csproj` del harness.
3. Las primeras pruebas locales detectaron dos fallos reales de recuperación de guardados corruptos. Se corrigió la captura de `InvalidDataException`, y los 33 casos pasaron después tanto localmente como en la nube.
4. La integración devuelve HTTP 403 al consultar configuración de Actions/listar secretos. Esto no impidió subir el workflow, observar los trabajos ni recuperar artefactos mediante la conexión. El propio preflight del runner confirmó que el conjunto de credenciales requerido está incompleto.
5. La web `game.ci` devolvió HTTP 403; se consultó su documentación oficial equivalente en GitHub. La descarga de artefactos por CLI también devolvió 403; se recuperaron mediante el plugin conectado, sin eludir controles de acceso.

## Intervención necesaria desde el teléfono

El acceso al repositorio ya está resuelto. El bloqueo actual es una licencia Unity legítima y las credenciales asociadas, que debes configurar en:

https://github.com/ramirezbasora12-ui/guardianes-del-portal/settings/secrets/actions

- Ruta Personal compatible: **UNITY_LICENSE**, **UNITY_EMAIL**, **UNITY_PASSWORD**.
- Ruta profesional con serial autorizado: **UNITY_SERIAL**, **UNITY_EMAIL**, **UNITY_PASSWORD**, en lugar de UNITY_LICENSE.

No enviar contraseñas, licencias ni tokens por el chat. Si no tienes un archivo Personal `.ulf` válido, Android no ejecuta Unity Hub: la guía explica una estación Linux remota accesible por navegador o una alternativa de servicio Unity Cloud, con cuentas/costes y autorización propios. No se ha contratado ningún servicio ni generado una licencia ficticia.

Después, abrir Actions → Unity Android — Bosque Primordial → Run workflow. La activación y los pasos Unity todavía pueden revelar errores de compilación, importación, renderizado o Gradle; se corregirán con sus logs reales. Una APK construida deberá instalarse y probarse en el teléfono antes de declararla funcional.

Guía completa: `Documentation/CLOUD_BUILD_ANDROID.md`. **No hay APK funcional ni Mundo 1 terminado.**

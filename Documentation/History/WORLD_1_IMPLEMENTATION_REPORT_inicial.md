# Informe de implementación — Mundo 1: Bosque Primordial

Fecha: 26 de octubre de 2026. **Estado: proyecto fuente parcial; no validado en Unity ni terminado para producción.**

## Entorno inspeccionado

`/workspace` no contenía un proyecto Unity existente. Se creó únicamente `/workspace/GuardianesDelPortal`, independiente de NEXARA. No había ejecutable Unity, Unity Hub, compilador C# ni Android Build Support disponible. Se eligió como destino Unity 2022.3.62f1 LTS; esa versión no está instalada aquí. No se afirma que se haya creado un proyecto mediante el asistente de Unity.

Se consultó la guía del entorno de nube y su política de red. Se utilizaron generación de imágenes, Python y análisis sintáctico C#. No se intentó simular Unity con otro motor. La referencia llegó después del primer encargo de ilustración; se analizó e incorporó a la distribución de UI, pero la pintura original conserva su propia topología.

## Funciones implementadas en código

- Carpetas modulares solicitadas y ensamblados Runtime, Editor y Tests. Guardians, Enemies y Animations quedan preparados sin contenido ficticio.
- Escena Unity de arranque y generador de escena editable: Canvas, área segura, mapa, diez nodos, seis botones de mundo, cabecera, saldos, ajustes, cofres, panel de nivel y botón Entrar. La generación dentro de Unity queda pendiente.
- Diez nombres de nivel, dificultad y posiciones configurables en el ScriptableObject incluido `Resources/World1.asset`. Estados bloqueado/disponible/seleccionado/completado/perfecto mediante interacción, medallón, estrellas y tinte.
- Bloqueo secuencial; mejor resultado de hasta tres estrellas; apertura del siguiente mundo al completar el jefe. Solo el primer mundo tiene mapa.
- Carga asíncrona de escenas configuradas y pantalla explícita EN DESARROLLO si no existen. Ningún nivel de combate está implementado.
- Monedas y cristales en el modelo persistente, inicialmente cero; sin compras ni servicios de pago.
- Recompensas configurables a 10/20/30 estrellas, tres estados y protección de reclamo repetido. Persistencia antes de mostrar la recompensa y reversión en memoria si el guardado falla.
- Formato de guardado v1, validación de rangos y progresión, comprobación SHA-256, temporal con flush, copia de respaldo y recuperación de corrupción. El reemplazo usa respaldo y movimiento de archivos; no se promete atomicidad transaccional del sistema de archivos ni protección frente a manipulación deliberada.
- Música y efectos con volumen, tres perfiles de calidad y guardado al salir de ajustes/pausar. Solo español; falta selector multilingüe y menú general independiente.
- Input System, arrastre/zoom limitado del área de mapa, máscara, UI fuera del mapa, escalado y área segura. Pendiente de verificar sobre dispositivos.
- Shader móvil sencillo para ondulación sobre agua turquesa y brillo violeta, pulso de selección y animación de sacudida de cofre.
- Atlas y compresión ASTC 6×6 configurados por Editor; instrumentación de FPS, peor tiempo de fotograma y memoria Unity cada diez segundos. No son medidas ya realizadas.
- Comandos Editor para importar/slice/atlas, generar escena, validar, capturar Play Mode y construir APK.

## Activos creados

Tres PNG originales: bosque ilustrado, hoja de doce elementos UI y hoja de seis miniaturas. Dos WAV originales de síntesis. Inventario completo en `Documentation/ASSET_INVENTORY.md`. No se incluyen imágenes de juegos comerciales.

## Pruebas ejecutadas aquí

- Inspección de disponibilidad de herramientas y ausencia de proyecto previo.
- Análisis de sintaxis con Tree-sitter de los 12 archivos C#: sin errores sintácticos. **No es compilación, comprobación de tipos ni validación de API Unity.**
- Validación estática reproducible con `Tools/validate_source.py`: estructura, paquetes JSON, metadatos/referencia de escena, catálogo de diez niveles y seis mundos, doce regiones de UI y seis miniaturas, audio y dimensiones/alfa de PNG. Resultado en `Validation/static-results.json`.
- Revisión visual de las imágenes generadas y comparación cualitativa con la referencia suministrada; no comparación de capturas Unity.

## Pruebas incluidas, aún no ejecutadas

NUnit EditMode: estado inicial, restricciones, desbloqueo de diez niveles, conservación de máximos, resultados inválidos, hitos y reclamo doble, cofres bloqueados, catálogo, guardado/carga, recuperación con y sin respaldo, versión futura, progresión inválida y ruta de desarrollo sin escena jugable. No se presentaron como aprobadas.

## Pruebas no ejecutadas

Compilación C#/shaders en Unity, resolución real de UPM, importación/slicing/atlas, generación y carga de escena, interacción con los diez botones, ajustes/audio, gestos multitáctiles, escalado 16:9/18:9/20:9/tabletas, pérdida de foco y proceso en Android, renderizado de recortes UI, referencias tras importación, ejecución offline, rendimiento, captura de Unity, compilación e instalación APK. No hay dispositivo Android ni pruebas de 4 GB de RAM.

## Diferencias visuales reales

Se conserva la idea de barra lateral de seis mundos, cabecera de madera, ruta de diez niveles, bosque con cascadas, portal azul, zona violeta de jefe y recompensas inferiores. La ilustración nueva es más panorámica y sus terrazas ascienden hacia la esquina superior derecha; la referencia tiene un recorrido más horizontal y concentrado. Por coherencia con el terreno original generado, los nodos se colocan sobre sus plataformas, no en las coordenadas exactas de la referencia.

La identidad tipográfica no reproduce el logotipo ilustrado. Los marcos, paneles y cofres son diferentes. Los recortes de sprites pueden requerir ajuste de bordes y proporciones; no están inspeccionados en Unity. El mapa usa proporción 16:9 dentro del área disponible y puede dejar márgenes según pantalla. Se prioriza no deformar el fondo y los iconos; los paneles y botones usan nueve segmentos configurados para adaptar sus superficies y todavía necesitan revisión artística. El saldo cero y los niveles bloqueados son datos iniciales reales; la referencia presenta 30/30 y todos los niveles completos. No se afirma fidelidad píxel a píxel.

## Problemas conocidos y faltantes

- Falta validar la compilación y generar de verdad la escena completa en Unity. La `.unity` entregada contiene el controlador de arranque; el Editor debe expandir y guardar la jerarquía. La interfaz también se construye en Awake como respaldo.
- El fondo ambiental es una ilustración integrada. Faltan capas independientes, vegetación animada, nubes, partículas/luciérnagas con pooling y cascadas completas animadas.
- Solo hay un portal inicial y un portal de jefe diferenciados en la pintura; falta una salida al mundo siguiente claramente identificada y activable tras progresión.
- No hay jugabilidad de niveles, enemigos, guardianes, combate ni escenas de nivel. La conexión arquitectónica de carga y resultado está preparada.
- La pantalla de ajustes vuelve al mapa; falta menú principal. Solo existe idioma español.
- La apertura de cofres no tiene sprites de tapa abierta; usa sacudida/escala. El sonido es un ambiente tonal sencillo.
- Built-in 2D + UGUI, sin URP 2D: no se pudo verificar ni configurar un pipeline URP con el Editor ausente. Evaluar migración después de importar y medir; no hay iluminación 2D implementada.
- Sin medidas reales de rendimiento. No se garantiza aún estabilidad en dispositivos de 4 GB.
- La suite cubre lógica y persistencia, no sustituye pruebas PlayMode/Android ni revisión visual.

## Android y capturas

**APK no generado. Capturas de Unity no generadas.** Falta Unity con Android Build Support y licencia/configuración de ejecución apropiada. La herramienta de compilación está incluida; su presencia no demuestra una compilación. Las ilustraciones PNG son activos, no capturas del juego.

## Trabajo restante prioritario

1. Abrir en Unity LTS, resolver UPM, corregir cualquier incompatibilidad y ejecutar generación/validación/tests.
2. Revisar cada sprite recortado y su tamaño, todos los estados de UI y el encaje de nodos. Capturar en Unity y comparar con la referencia.
3. Producir capas ambientales, salida de mundo, logotipo y animaciones faltantes; completar audio y menú/idiomas.
4. Implementar al menos una escena de juego y su retorno/resultados antes de denominar los niveles jugables.
5. Probar offline, multitáctil, corrupción de guardado y relaciones de aspecto. Medir dispositivo Android ARM64 de 4 GB, ajustar efectos/texturas y compilar/instalar APK.

El proyecto no se declara completo: faltan validación crítica y trabajo visual sustancial.

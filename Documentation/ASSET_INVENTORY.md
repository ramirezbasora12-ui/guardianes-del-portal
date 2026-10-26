# Inventario artístico — Mundo 1

Referencia recibida: `1-12974.jpg`, 1280 × 853. Se analizó su composición; no se redistribuye ni se usa como textura del juego porque su procedencia/licencia no está establecida.

| Recurso original | Archivo | Resolución | Integración |
|---|---|---|---|
| Bosque, árbol ancestral, valles, islas, cascadas, río, puentes, ruinas, cristales y portales azul/violeta | Resources/Art/forest.png | 1672 × 941 | Fondo ilustrado, independiente de la UI; distorsión y brillo por shader |
| Placa de madera, medallones azul/gris/verde | Resources/Art/ui.png | 1536 × 1024, RGBA | Subsprites, recortes explícitos en Editor |
| Cofres bronce/plata/oro, estrella, botón verde, cristales azul/violeta, moneda | misma hoja ui.png | misma | Botones e iconos independientes |
| Seis miniaturas originales de los mundos | Resources/Art/worlds.png | 1536 × 1024 | Seis subsprites; los futuros mundos no tienen escenas jugables |
| Ambiente tonal suave | Resources/Audio/bosque_ambiente.wav | 16 s, mono 22050 Hz | Síntesis original, bucle musical sencillo |
| Sonido de selección | Resources/Audio/seleccion.wav | 0,19 s, mono 22050 Hz | Síntesis original, sin muestras externas |

Las tres ilustraciones se produjeron con la herramienta de generación de imágenes disponible. La hoja de UI recibió una segunda pasada de fondo transparente. Los PNG originales se conservan sin recortar: Unity importa las regiones como sprites. El alfa se comprobó en los archivos. La hoja no obedece una cuadrícula perfecta; los rectángulos están calibrados manualmente y todavía requieren inspección de bordes dentro de Unity.

## Capas y elementos pendientes

- Separar árboles de primer plano, vegetación y nubes del fondo; no se ha extraído mágicamente la ilustración de referencia.
- Sprites independientes de cascadas, espuma y reflejos. El shader actual anima el color y UV del agua del fondo, no una simulación de cascada.
- Capas de energía de ambos portales, salida al mundo 2 claramente diferenciada del portal del jefe.
- Luciérnagas ilustradas con pooling, partículas y balance por calidad.
- Animación de apertura de tapa para los tres cofres. Actualmente hay sacudida y escala antes del texto de recompensa.
- Logotipo ilustrado original. La identidad actual es tipográfica.
- Iconografía específica de ajustes y dificultades. Se usan texto y medallón ilustrado existentes.
- Paisaje sonoro de bosque y música compuesta; el ambiente tonal original no sustituye una banda sonora final.

El entorno no está compuesto enteramente por sprites ambientales independientes. La interfaz, niveles, cofres e iconos sí se componen por separado. No se han usado imágenes de otros juegos ni descargas de bancos sin licencia.

## Ampliación de capas independientes

Cuatro hojas originales nuevas: `environment_water.png`, `environment_foliage.png`, `environment_magic.png` (1254×1254) y `environment_particles.png` (1672×941), todas RGBA con transparencia comprobada. No se alteraron los tres PNG originales.

| Hoja | Regiones originales preparadas | Uso |
|---|---|---|
| Agua | velo de cascada, espuma, ondas, corriente | Capas UV con movimiento localizado |
| Vegetación | dosel colgante, helecho, rama corrupta, niebla | Balanceo UV y deriva suave |
| Magia | cristales, energía inicial, energía del jefe, salida dorada/cian | Pulsos/deformación y desbloqueo por progreso |
| Partículas | luciérnaga, destello cian, hoja, runa violeta | Pool de luciérnagas/destellos; hoja/runa todavía sin colocar |

Los recursos se generaron tomando la pintura propia como referencia de estilo. El plan utiliza regiones de textura, objetos UI independientes y materiales compartidos por tipo. No depende de extraer perfectamente el JPG del usuario. Son recursos preparados e integrados en código; su calidad de recorte y su alineación aún requieren render real de Unity. El inventario previo de faltantes se conserva como histórico: ya hay recursos nuevos de agua, vegetación, cristales, energía y partículas, pero su validación visual sigue pendiente.

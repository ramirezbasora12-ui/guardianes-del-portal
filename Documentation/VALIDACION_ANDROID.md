# Plan pendiente de validación real

1. Importar en Unity 2022.3, conservar logs y ejecutar todas las pruebas EditMode. Confirmar que no hay errores de scripts, materiales ni referencias.
2. Generar escena y revisar la jerarquía editable. Probar diez nodos en estados bloqueado/desbloqueado/seleccionado/completado/perfecto con un guardado de pruebas aislado, sin enviarlo al usuario final.
3. Verificar con resultados reales o fixture local: estrellas 0/9/10/19/20/29/30, tres cofres y repetición de reclamos tras reinicio. Probar archivo corrupto, respaldo corrupto, carpeta no escribible y cierre durante guardado.
4. Game View a 1600×1000, 1920×1080, 2160×1080 y 2400×1080, además de Device Simulator con notch. Revisar márgenes, texto, mapa sin deformación, botones y panel modal.
5. En Android ARM64: dos dedos, arrastre desde botón, gesto cerca del borde, pausa/reanudación, botón Atrás, volúmenes y reinicio sin conexión.
6. Capturar desde Play Mode y Android. Comparar posiciones, distribución, contraste y lectura con `1-12974.jpg`. Documentar diferencias sin llamar capturas a los activos fuente.
7. En dispositivo de 4 GB, medir al menos 10 minutos por perfil con Unity Profiler y `RuntimePerformance`. Registrar modelo, Android, resolución, temperatura, FPS, percentiles de tiempos, memoria y carga. Objetivos orientativos: 30 FPS estables en Baja, 60 si el hardware lo permite. No están demostrados.
8. Construir APK, instalar en dispositivo, volver a comprobar completamente offline y conservar logs/resultados/capturas junto al informe.

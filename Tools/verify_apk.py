"""Comprueba estructura de una APK Unity real; no ejecuta Android ni verifica su firma."""
from pathlib import Path
import hashlib,json,sys,zipfile
apk=Path(sys.argv[1]); output=Path(sys.argv[2]); output.parent.mkdir(parents=True,exist_ok=True)
if not apk.is_file():raise SystemExit('No existe una APK para verificar')
with zipfile.ZipFile(apk) as archive:
    names=archive.namelist()
    required=['AndroidManifest.xml','classes.dex','lib/arm64-v8a/libunity.so','lib/arm64-v8a/libil2cpp.so']
    missing=[name for name in required if name not in names]
    if archive.testzip() is not None:raise SystemExit('APK ZIP corrupta')
    if missing:raise SystemExit('Faltan entradas Unity ARM64: '+str(missing))
result={'apk':str(apk),'size_bytes':apk.stat().st_size,'sha256':hashlib.sha256(apk.read_bytes()).hexdigest(),'structure_validated':True,'signature_verified':False,'installed_on_android':False}
output.write_text(json.dumps(result,indent=2)+'\n');print('Estructura APK Unity ARM64 correcta; instalación y firma aún pendientes.')

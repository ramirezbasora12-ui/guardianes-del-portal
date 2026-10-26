"""No imprime secretos ni copia archivos de activación a los artefactos."""
from pathlib import Path
import os
secrets=[os.environ.get(key,'') for key in ('UNITY_LICENSE','UNITY_EMAIL','UNITY_PASSWORD','UNITY_SERIAL')]
values=sorted({part for value in secrets if value for part in [value,*value.splitlines()] if len(part)>=4},key=len,reverse=True)
count=0
for folder in ('CIArtifacts','artifacts'):
    if not Path(folder).exists():continue
    for path in Path(folder).rglob('*'):
        if path.is_file() and path.suffix.lower() in ('.log','.txt','.json','.xml'):
            text=path.read_text(errors='replace')
            for value in values:text=text.replace(value,'[SECRETO OCULTO]')
            path.write_text(text);count+=1
print('Registros revisados antes de subir:',count)

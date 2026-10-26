"""Validación estática; no ejecuta ni compila Unity. Python 3 + Pillow; Tree-sitter opcional."""
from pathlib import Path
import json,re,sys,wave
from PIL import Image
root=Path(__file__).resolve().parents[1]
checks=[]
def check(name,condition,detail=''):
 checks.append({'comprobacion':name,'correcto':bool(condition),'detalle':detail})
for folder in ['Core','WorldMap','Levels','Guardians','Enemies','UI','Audio','VisualEffects','SaveSystem','Art','Animations','Editor','Tests']:
 check('Carpeta '+folder,(root/'Assets/Guardianes'/folder).is_dir())
manifest=json.loads((root/'Packages/manifest.json').read_text())
check('Input System declarado','com.unity.inputsystem' in manifest['dependencies'])
check('Paquetes JSON válidos',all(json.loads(f.read_text()) for f in root.rglob('*.asmdef')))
catalog=(root/'Assets/Guardianes/Core/WorldCatalog.cs').read_text()
check('Diez definiciones de nivel',len(re.findall(r'new LevelDefinition\("',catalog))==10)
check('Seis mundos',all(x in catalog for x in ['Bosque Primordial','Volcán Carmesí','Hielo Eterno','Desierto Ancestral','Tierras Sombrías','Dimensión Celestial']))
check('Hitos 10/20/30',all('stars='+str(x) in catalog for x in [10,20,30]))
for name,size,alpha in [('forest',(1672,941),False),('ui',(1536,1024),True),('worlds',(1536,1024),False)]:
 f=root/'Assets/Guardianes/Resources/Art'/f'{name}.png'
 with Image.open(f) as im:
  check('Imagen '+name,im.size==size,f'{im.size}, {im.mode}')
  if alpha:check('Alfa transparente en UI',im.mode=='RGBA' and im.getchannel('A').getextrema()[0]==0)
scene=(root/'Assets/Guardianes/WorldMap/Scenes/World1.unity').read_text()
meta=(root/'Assets/Guardianes/WorldMap/WorldMapController.cs.meta').read_text()
guid=re.search(r'guid: ([a-f0-9]{32})',meta).group(1)
check('Escena enlaza controlador real',guid in scene)
missing=[str(f.relative_to(root)) for f in (root/'Assets').rglob('*') if f.suffix!='.meta' and not Path(str(f)+'.meta').exists()]
check('Metadatos de activos y carpetas',not missing,str(missing))
for f in (root/'Assets/Guardianes/Resources/Audio').glob('*.wav'):
 with wave.open(str(f)) as w:check('Audio '+f.name,w.getnframes()>0 and w.getframerate()==22050,str(w.getnframes()/w.getframerate())+' s')
builder=(root/'Assets/Guardianes/Editor/World1Builder.cs').read_text()
check('Doce subsprites UI declarados',all('"'+x+'"' in builder for x in ['plaque','node','locked','selected','bronze','silver','gold','star','play','blue','purple','coin']))
check('Seis recortes de miniaturas','j<6' in builder)
check('Captura exige Play Mode','if(!Application.isPlaying)' in builder)
check('Herramienta APK incluida','BuildPipeline.BuildPlayer' in builder)
check('Informe en español',(root/'WORLD_1_IMPLEMENTATION_REPORT.md').exists())

plan=json.loads((root/'Assets/Guardianes/Resources/EnvironmentLayers.json').read_text())
keys={x['key'] for x in plan['effects']}
check('Dieciséis efectos ilustrados',len(keys)==16)
check('Veinte capas ambientales',len(plan['layers'])==20)
check('Capas enlazadas a efectos conocidos',all(layer['effect'] in keys for layer in plan['layers']))
check('Regiones UV dentro de hojas',all(0<=effect['u'] and 0<=effect['v'] and effect['w']>0 and effect['h']>0 and effect['u']+effect['w']<=1.00001 and effect['v']+effect['h']<=1.00001 for effect in plan['effects']))
for file in (root/'Assets/Guardianes/Resources/Art').glob('environment*.png'):
 with Image.open(file) as image:
  check('Transparencia '+file.name,image.mode=='RGBA' and image.getchannel('A').getextrema()[0]==0)
check('Capa ambiental conectada al mapa','EnvironmentComposer.Build(map)' in (root/'Assets/Guardianes/UI/MapView.cs').read_text())
check('Guardado incluye captura de InvalidDataException','e is InvalidDataException' in (root/'Assets/Guardianes/SaveSystem/SaveService.cs').read_text())
check('Workflow Unity presente',(root/'.github/workflows/unity-android.yml').exists())

syntax={'ejecutado':False,'nota':'Tree-sitter no instalado. No equivale a compilación Unity.'}
try:
 from tree_sitter import Language,Parser
 import tree_sitter_c_sharp
 parser=Parser(Language(tree_sitter_c_sharp.language()))
 files=list((root/'Assets').rglob('*.cs'))+[root/'Tools/CoreTests/Program.cs']
 errors=[str(f.relative_to(root)) for f in files if parser.parse(f.read_bytes()).root_node.has_error]
 syntax={'ejecutado':True,'archivos':len(files),'errores':errors,'nota':'Solo sintaxis; no comprueba tipos ni APIs de Unity.'}
 check('Sintaxis C# sin errores',not errors,str(len(files))+' archivos')
except ImportError:pass
result={'alcance':'Validación estática de archivos, no ejecución Unity ni Android','comprobaciones':checks,'sintaxis_csharp':syntax,'unity_ejecutado':False,'android_ejecutado':False,'correcto':all(x['correcto'] for x in checks)}
(root/'Validation').mkdir(exist_ok=True)
(root/'Validation/static-results.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n')
print(json.dumps({'comprobaciones':len(checks),'correctas':sum(x['correcto'] for x in checks),'fallos':[x for x in checks if not x['correcto']],'sintaxis_csharp':syntax},ensure_ascii=False,indent=2))
sys.exit(0 if result['correcto'] else 1)

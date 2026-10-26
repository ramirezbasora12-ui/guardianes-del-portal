using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
namespace Guardianes.Editor {
 public static class World1Builder {
  const string Root="Assets/Guardianes/";const string ScenePath=Root+"WorldMap/Scenes/World1.unity";
  [MenuItem("Guardianes/1 · Configurar proyecto Android")]
  public static void Configure(){
   PlayerSettings.bundleVersion="0.1.0";PlayerSettings.Android.bundleVersionCode=1;
   PlayerSettings.companyName="Portal Independiente";PlayerSettings.productName="Guardianes del Portal";
   PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.portalindependiente.guardianes");
   PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.allowedAutorotateToPortrait=false;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
   PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevelAuto;
   PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
   PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,ApiCompatibilityLevel.NET_Standard);
   PlayerSettings.colorSpace=ColorSpace.Gamma;
   var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);var input=settings.FindProperty("activeInputHandler");if(input!=null){input.intValue=1;settings.ApplyModifiedPropertiesWithoutUndo();}
   // Se usan los tres perfiles configurados en QualitySettings.asset.
   Debug.Log("Android configurado: landscape, ARM64, IL2CPP, Input System. Se requiere reiniciar si Unity lo solicita.");
  }
  [MenuItem("Guardianes/2 · Importar arte y crear atlas")]
  public static void ImportArt(){
   foreach(string file in Directory.GetFiles(Root+"Resources/Art","*.png")){
    var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.isReadable=false;importer.maxTextureSize=2048;importer.spritePixelsPerUnit=100;importer.filterMode=FilterMode.Bilinear;
    string key=Path.GetFileNameWithoutExtension(file);var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(file);
    if(key=="ui"||key=="worlds"){
     importer.spriteImportMode=SpriteImportMode.Multiple;var sprites=new System.Collections.Generic.List<SpriteMetaData>();
     if(key=="worlds"){for(int j=0;j<6;j++){float w=tex.width/2f,h=tex.height/3f;sprites.Add(Meta("world"+j,new Rect((j%2)*w,tex.height-(j/2+1)*h,w,h)));}}
     else {
      // Explicit rectangles, calibrated to the original sheet; no assumption of perfect extraction from the reference.
      string[] names={"plaque","node","locked","selected","bronze","silver","gold","star","play","blue","purple","coin"};
      float[] edges={0,550,880,1210,1536};
      for(int j=0;j<12;j++){int row=j/4,col=j%4;float x,w;if(row==0||row==2){x=edges[col];w=edges[col+1]-x;}else{float[] middle={0,410,820,1210,1536};x=middle[col];w=middle[col+1]-x;}
       if(row==2){float[] bottom={0,580,870,1190,1536};x=bottom[col];w=bottom[col+1]-x;}
       float y=row==0?0:row==1?342:682;float h=row==2?342:340;
       sprites.Add(Meta(names[j],new Rect(x*tex.width/1536f,tex.height-(y+h)*tex.height/1024f,w*tex.width/1536f,h*tex.height/1024f)));
      }
     }
#pragma warning disable 618
     importer.spritesheet=sprites.ToArray();
#pragma warning restore 618
    }else importer.spriteImportMode=SpriteImportMode.Single;
    var android=new TextureImporterPlatformSettings{name="Android",overridden=true,maxTextureSize=2048,format=TextureImporterFormat.ASTC_6x6,compressionQuality=75};importer.SetPlatformTextureSettings(android);importer.SaveAndReimport();
   }
   string atlasPath=Root+"Art/World1.spriteatlas";var atlas=AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);if(!atlas){atlas=new SpriteAtlas();AssetDatabase.CreateAsset(atlas,atlasPath);}
   var old=atlas.GetPackables();if(old.Length>0)atlas.Remove(old);
   atlas.Add(new UnityEngine.Object[]{AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Resources/Art/ui.png"),AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Resources/Art/worlds.png")});
   var packing=atlas.GetPackingSettings();packing.enableRotation=false;packing.enableTightPacking=false;packing.padding=4;atlas.SetPackingSettings(packing);
   atlas.SetPlatformSettings(new TextureImporterPlatformSettings{name="Android",overridden=true,maxTextureSize=2048,format=TextureImporterFormat.ASTC_6x6});
   string materialPath=Root+"Resources/WaterAtmosphere.mat";var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);if(!material){material=new Material(Shader.Find("Guardianes/ForestWater"));AssetDatabase.CreateAsset(material,materialPath);}CreateEnvironmentMaterials();AssetDatabase.SaveAssets();MapView.ReloadArt();
  }
  static void CreateEnvironmentMaterials(){
   Directory.CreateDirectory(Root+"Resources/Effects");
   var shader=Shader.Find("Guardianes/EnvironmentLayer");if(!shader)throw new InvalidOperationException("Falta shader EnvironmentLayer");
   foreach(var effect in EnvironmentPlan.Load().effects){
    string path=Root+"Resources/Effects/"+effect.key+".mat";
    var material=AssetDatabase.LoadAssetAtPath<Material>(path);
    if(!material){material=new Material(shader);AssetDatabase.CreateAsset(material,path);}
    material.shader=shader;material.SetVector("_Region",new Vector4(effect.u,effect.v,effect.w,effect.h));
    material.SetFloat("_Mode",effect.mode);material.SetFloat("_Speed",effect.speed);material.SetFloat("_Intensity",effect.intensity);EditorUtility.SetDirty(material);
   }
  }
  static SpriteMetaData Meta(string name,Rect rect){return new SpriteMetaData{name=name,rect=rect,pivot=new Vector2(.5f,.5f),alignment=9,border=name=="plaque"?new Vector4(80,100,80,100):name=="play"?new Vector4(80,60,80,60):Vector4.zero};}
  [MenuItem("Guardianes/3 · Generar escena editable Mundo 1")]
  public static void Generate(){
   if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
   GenerateInternal();
  }
  public static void GenerateInternal(){
   if(File.Exists(ScenePath)){Directory.CreateDirectory("SceneBackups");File.Copy(ScenePath,"SceneBackups/World1-"+DateTime.Now.ToString("yyyyMMdd-HHmmss-fff")+".unity",false);}
   ImportArt();var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
   var camera=new GameObject("Camara",typeof(Camera));camera.tag="MainCamera";camera.GetComponent<Camera>().orthographic=true;camera.GetComponent<Camera>().backgroundColor=new Color(.02f,.04f,.07f);camera.AddComponent<AudioListener>();
   string dataPath=Root+"Resources/World1.asset";var catalog=AssetDatabase.LoadAssetAtPath<WorldCatalog>(dataPath);if(!catalog){catalog=ScriptableObject.CreateInstance<WorldCatalog>();AssetDatabase.CreateAsset(catalog,dataPath);}
   var root=new GameObject("Mundo1");var controller=root.AddComponent<WorldMapController>();controller.catalog=catalog;root.AddComponent<RuntimePerformance>();MapView.Build(root.transform,catalog);
   var music=new GameObject("Musica",typeof(AudioSource));music.transform.SetParent(root.transform);music.GetComponent<AudioSource>().loop=true;music.GetComponent<AudioSource>().playOnAwake=false;
   var sfx=new GameObject("EfectosAudio",typeof(AudioSource));sfx.transform.SetParent(root.transform);sfx.GetComponent<AudioSource>().playOnAwake=false;
   Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));EditorSceneManager.SaveScene(scene,ScenePath);
   EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};AssetDatabase.SaveAssets();Validate();
  }
  [MenuItem("Guardianes/4 · Validar escena y datos")]
  public static void Validate(){
   int errors=0;var root=UnityEngine.Object.FindObjectOfType<WorldMapController>();if(!root||!root.catalog){Debug.LogError("Escena o catálogo ausentes");return;}
   string invalid=root.catalog.ValidateData();if(invalid!=null){Debug.LogError(invalid);errors++;}
   foreach(var image in root.GetComponentsInChildren<UnityEngine.UI.Image>(true)){
    if(image.gameObject.name=="Boton"||image.gameObject.name.StartsWith("Mundo_")||image.gameObject.name.StartsWith("Cofre_")||image.gameObject.name=="Ilustracion")if(!image.sprite){Debug.LogError("Falta sprite: "+image.name,image);errors++;}
   }
   if(root.GetComponentsInChildren<MapGestures>(true).Length!=1){errors++;Debug.LogError("Gestos no configurados");}
   Debug.Log("Validación Mundo 1: "+errors+" errores. Ejecuta Tests para validar progresión y guardado.");
  }
  [MenuItem("Guardianes/5 · Capturar ejecución (Play Mode)")]
  public static void Screenshot(){if(!Application.isPlaying){Debug.LogError("La captura requiere Play Mode; no se fabrican capturas de ejecución.");return;}Directory.CreateDirectory("Capturas");ScreenCapture.CaptureScreenshot("Capturas/Mundo1-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".png",1);}
  [MenuItem("Guardianes/6 · Compilar APK Android")]
  public static void Android(){
   if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android,BuildTarget.Android))throw new InvalidOperationException("Falta Android Build Support");Configure();Directory.CreateDirectory("Builds");
   if(!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android))throw new InvalidOperationException("No se pudo activar Android");
   var r=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/GuardianesDelPortal.apk",BuildTarget.Android,BuildOptions.None);if(r.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new InvalidOperationException("La compilación Android falló: "+r.summary.result);
  }
  public static void BatchGenerate(){Configure();GenerateInternal();}
 }
 [InitializeOnLoad] public static class FirstImport {
  static FirstImport(){EditorApplication.delayCall+=()=>{if(Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode||File.Exists("ProjectSettings/Guardianes.initialized"))return;
   try{World1Builder.Configure();World1Builder.GenerateInternal();File.WriteAllText("ProjectSettings/Guardianes.initialized","1");}catch(Exception e){Debug.LogError("La inicialización no terminó. Usa Guardianes > Generar escena. "+e);}
  };}
 }
}

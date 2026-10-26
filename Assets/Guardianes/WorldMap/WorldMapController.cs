using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Guardianes {
 public class WorldMapController:MonoBehaviour {
  public WorldCatalog catalog; PlayerData data;SaveService saves;Transform modal,panel;bool ready;string mode;AudioSource musicAudio,effectsAudio;AudioClip click;GameObject settings;
  Transform Find(string n){foreach(var t in GetComponentsInChildren<Transform>(true))if(t.name==n)return t;throw new InvalidOperationException("Falta referencia UI: "+n);}
  T Get<T>(string n)where T:Component{return Find(n).GetComponent<T>();}
  void Awake(){
   Application.targetFrameRate=60;Screen.orientation=ScreenOrientation.LandscapeLeft;
   if(!catalog)catalog=Resources.Load<WorldCatalog>("World1");if(!catalog)catalog=ScriptableObject.CreateInstance<WorldCatalog>();
   string invalid=catalog.ValidateData();if(invalid!=null)throw new InvalidOperationException(invalid);
   if(!GetComponentInChildren<Canvas>())MapView.Build(transform,catalog);
   modal=Find("Modal");panel=Find("PanelModal");
   saves=new SaveService(SaveLocation.Directory,new UnitySaveCodec());data=saves.Load();
   for(int j=0;j<10;j++){int level=j+1;Find("Nivel_"+level).GetComponentInChildren<Button>().onClick.AddListener(()=>SelectLevel(level));}
   for(int j=0;j<6;j++){int world=j;Get<Button>("Mundo_"+j).onClick.AddListener(()=>{if(world==0){Close();return;}Show(catalog.worlds[world],data.worlds[world]?"Mundo desbloqueado. Su mapa y sus niveles se implementarán en una fase futura.":"Mundo bloqueado. Completa el mundo anterior para avanzar.","ENTENDIDO",Close);});}
   for(int j=0;j<3;j++){int chest=j;Get<Button>("Cofre_"+j).onClick.AddListener(()=>Claim(chest));}
   Get<Button>("PortalSiguiente").onClick.AddListener(()=>Show("VOLCÁN CARMESÍ","Has abierto el siguiente portal. El Mundo 2 todavía está en desarrollo.","VOLVER",Close));
   Get<Button>("Entrar").onClick.AddListener(()=>SelectLevel(data.selected));Get<Button>("Cerrar").onClick.AddListener(Close);Get<Button>("Ajustes").onClick.AddListener(OpenSettings);
   SetupAudio();foreach(var button in GetComponentsInChildren<Button>(true))button.onClick.AddListener(PlayClick);
   ready=true;ApplySettings();Refresh();if(!string.IsNullOrEmpty(saves.RecoveryMessage))Show("Guardado recuperado",saves.RecoveryMessage,"CONTINUAR",Close);
  }
  void SetupAudio(){
   if(!FindObjectOfType<AudioListener>()){var listener=new GameObject("EscuchaAudio",typeof(AudioListener));listener.transform.SetParent(transform);}
   foreach(var audio in GetComponentsInChildren<AudioSource>(true)){if(audio.name=="Musica")musicAudio=audio;if(audio.name=="EfectosAudio")effectsAudio=audio;}
   if(!musicAudio){var go=new GameObject("Musica",typeof(AudioSource));go.transform.SetParent(transform);musicAudio=go.GetComponent<AudioSource>();}
   if(!effectsAudio){var go=new GameObject("EfectosAudio",typeof(AudioSource));go.transform.SetParent(transform);effectsAudio=go.GetComponent<AudioSource>();}
   musicAudio.clip=Resources.Load<AudioClip>("Audio/bosque_ambiente");musicAudio.loop=true;musicAudio.volume=data.settings.music;if(musicAudio.clip)musicAudio.Play();click=Resources.Load<AudioClip>("Audio/seleccion");
  }
  void PlayClick(){if(effectsAudio&&click)effectsAudio.PlayOneShot(click);}
  void ApplySettings(){QualitySettings.SetQualityLevel(data.settings.quality,true);Application.targetFrameRate=data.settings.quality==0?30:60;Shader.SetGlobalFloat("_GuardianesQuality",data.settings.quality==0?.4f:data.settings.quality==1?.7f:1f);
   foreach(var a in GetComponentsInChildren<AudioSource>(true))a.volume=a.gameObject.name.Contains("Musica")?data.settings.music:data.settings.sfx;
  }
  bool Persist(){try{saves.Save(data);return true;}catch(Exception e){Debug.LogError("No se pudo guardar: "+e.Message);Show("No se pudo guardar","Comprueba el espacio disponible. El progreso de esta sesión sigue en memoria.","CERRAR",Close);return false;}}
  void Refresh(){
   Find("PortalSiguiente").gameObject.SetActive(data.worlds[1]);
   Get<Text>("Saldo0").text=data.gold.ToString("N0");Get<Text>("Saldo1").text=data.blue.ToString("N0");Get<Text>("Saldo2").text=data.purple.ToString("N0");
   Get<Text>("TotalEstrellas").text=data.TotalStars+" / 30\nESTRELLAS";((RectTransform)Find("Progreso")).anchorMax=new Vector2(data.TotalStars/30f,1);
   for(int j=0;j<10;j++){
    var root=Find("Nivel_"+(j+1));var b=root.GetComponentInChildren<Button>();bool unlocked=data.IsUnlocked(j+1);b.interactable=unlocked;
    bool selected=data.selected==j+1;int stars=data.stars[j];b.image.sprite=MapView.Art(!unlocked?"locked":selected?"selected":"node");b.image.color=stars==3?new Color(1,.88f,.48f):Color.white;
    root.Find("Boton/Numero").GetComponent<Text>().text=unlocked?(j+1).ToString():"×";
    for(int k=0;k<3;k++)root.Find("Estrellas/Estrella_"+k).GetComponent<Image>().color=k<stars?Color.white:new Color(.3f,.35f,.4f);
    var pulse=b.GetComponent<Atmosphere>();if(selected){if(!pulse)pulse=b.gameObject.AddComponent<Atmosphere>();pulse.motion=Atmosphere.Motion.Pulse;pulse.speed=2;pulse.enabled=true;}else if(pulse)pulse.enabled=false;
   }
   for(int j=0;j<3;j++){
    var b=Get<Button>("Cofre_"+j);bool available=data.TotalStars>=catalog.rewards[j].stars&&!data.claimed[j];
    b.image.color=data.claimed[j]?new Color(.65f,.8f,.65f):available?Color.white:new Color(.4f,.45f,.5f);
    b.transform.Find("Estado").GetComponent<Text>().text=data.claimed[j]?"RECLAMADO":available?"ABRIR":catalog.rewards[j].stars+" ★";
   }
   for(int j=1;j<6;j++)Find("Mundo_"+j).Find("Bloqueo").GetComponent<Text>().text=data.worlds[j]?"PRÓXIMAMENTE":"BLOQUEADO";
  }
  void SelectLevel(int level){if(!data.IsUnlocked(level))return;data.selected=level;if(!Persist()){Refresh();return;}Refresh();var l=catalog.levels[level-1];
   Show("NIVEL "+level+" · "+l.name,"Dificultad: "+new string('★',l.difficulty)+"\nEstrellas: "+data.stars[level-1]+" / 3\nRecompensa: estrellas para los cofres del mundo.\n"+(level==10?"Encuentro final con el Guardián Antiguo.":"Explora el Bosque Primordial."),"JUGAR",()=>Play(level));
  }
  void Play(int level){if(LevelLoader.TryLoad(catalog,data,level))return;
   Show("NIVEL "+level+" — EN DESARROLLO","El mapa es interactivo. La jugabilidad de este nivel todavía no está implementada.\nEsta pantalla no concede estrellas ni monedas.","VOLVER AL MAPA",Close);
  }
  // Called only by a real gameplay result; no production UI grants fake completion.
  public bool SubmitLevelResult(int level,int stars){if(!data.IsUnlocked(level)||stars<1||stars>3)return false;data.Complete(level,stars);bool saved=Persist();Refresh();return saved;}
  void Show(string title,string body,string action,UnityEngine.Events.UnityAction callback){
   mode="dialog";if(settings)settings.SetActive(false);modal.gameObject.SetActive(true);Get<Text>("ModalTitulo").text=title;Get<Text>("ModalTexto").text=body;Get<Text>("ModalTexto").gameObject.SetActive(true);
   var b=Get<Button>("ModalAccion");b.gameObject.SetActive(true);b.GetComponentInChildren<Text>().text=action;b.onClick.RemoveAllListeners();b.onClick.AddListener(callback);
  }
  void Close(){if(mode=="settings"){mode="";if(!Persist())return;}modal.gameObject.SetActive(false);if(settings)settings.SetActive(false);mode="";}
  void Claim(int i){var r=catalog.rewards[i];if(data.claimed[i]){Show("Cofre reclamado","Esta recompensa ya forma parte de tu inventario.","CERRAR",Close);return;}
   if(data.TotalStars<r.stars){Show("Cofre bloqueado","Necesitas "+r.stars+" estrellas. Progreso: "+data.TotalStars+" / 30.","CERRAR",Close);return;}
   // Roll back the reward if it could not be persisted.
   int gold=data.gold,blue=data.blue,purple=data.purple;
   if(!data.Claim(i,r))return;if(!Persist()){data.claimed[i]=false;data.gold=gold;data.blue=blue;data.purple=purple;Refresh();return;}
   Refresh();Show("¡RECOMPENSA OBTENIDA!","+"+r.gold+" oro\n+"+r.blue+" cristales azules\n+"+r.purple+" cristales morados","CONTINUAR",Close);StartCoroutine(OpenChest(i));
  }
  IEnumerator OpenChest(int i){var chest=MapView.Picture("Apertura",panel,MapView.Art(new[]{"bronze","silver","gold"}[i]),new Vector2(.36f,.36f),new Vector2(.64f,.66f));Get<Text>("ModalTexto").gameObject.SetActive(false);
   float t=0;while(t<1.1f){t+=Time.unscaledDeltaTime;chest.transform.localScale=Vector3.one*(1+Mathf.Sin(t*12)*.13f);chest.transform.localRotation=Quaternion.Euler(0,0,Mathf.Sin(t*24)*6*(1-t/1.1f));yield return null;}Destroy(chest.gameObject);if(mode=="dialog")Get<Text>("ModalTexto").gameObject.SetActive(true);
  }
  void OpenSettings(){
   Show("AJUSTES","","VOLVER AL MENÚ",()=>{Close();Refresh();});mode="settings";Get<Text>("ModalTexto").gameObject.SetActive(false);
   if(!settings){settings=MapView.Rect("ControlesAjustes",panel,new Vector2(.14f,.32f),new Vector2(.86f,.77f)).gameObject;
    AddSlider("Música",.78f,data.settings.music,v=>{data.settings.music=v;ApplySettings();});
    AddSlider("Efectos",.53f,data.settings.sfx,v=>{data.settings.sfx=v;ApplySettings();});
    var quality=MapView.Button("Calidad",settings.transform,MapView.Art("plaque"),"",new Vector2(0,.17f),new Vector2(1,.40f));
    quality.onClick.AddListener(()=>{data.settings.quality=(data.settings.quality+1)%3;quality.GetComponentInChildren<Text>().text="Calidad: "+new[]{"Baja","Media","Alta"}[data.settings.quality];ApplySettings();});
    MapView.Label("Texto",quality.transform,"Calidad: "+new[]{"Baja","Media","Alta"}[data.settings.quality],Vector2.zero,Vector2.one,24);
    MapView.Label("Idioma",settings.transform,"Idioma: Español · Otros idiomas pendientes",new Vector2(0,-.06f),new Vector2(1,.16f),20);
   }settings.SetActive(true);
  }
  void AddSlider(string label,float y,float value,UnityEngine.Events.UnityAction<float> change){
   var row=MapView.Rect(label,settings.transform,new Vector2(0,y),new Vector2(1,y+.2f));MapView.Label("Etiqueta",row,label,new Vector2(0,0),new Vector2(.35f,1),23,TextAnchor.MiddleLeft);
   var track=MapView.Picture("Pista",row,null,new Vector2(.39f,.25f),new Vector2(.98f,.75f),new Color(.08f,.18f,.22f));track.raycastTarget=true;
   var area=MapView.Rect("Area",track.transform,Vector2.zero,Vector2.one);area.offsetMin=new Vector2(12,0);area.offsetMax=new Vector2(-12,0);
   var handle=MapView.Picture("Control",area,MapView.Art("coin"),Vector2.zero,Vector2.one);handle.raycastTarget=true;handle.rectTransform.sizeDelta=new Vector2(34,34);
   var slider=track.gameObject.AddComponent<Slider>();slider.targetGraphic=handle;slider.handleRect=handle.rectTransform;slider.direction=Slider.Direction.LeftToRight;slider.minValue=0;slider.maxValue=1;slider.value=value;slider.onValueChanged.AddListener(change);
  }
  void Update(){if(UnityEngine.InputSystem.Keyboard.current!=null&&UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame){if(modal.gameObject.activeSelf){Close();}else OpenSettings();}}
  void OnApplicationPause(bool paused){if(paused&&ready)Persist();}void OnApplicationQuit(){if(ready)Persist();}
 }
}

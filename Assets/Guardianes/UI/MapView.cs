using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
namespace Guardianes {
 public static class MapView {
  static readonly Color ink=new Color(.025f,.065f,.09f,.94f);
  public static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max){var g=new GameObject(name,typeof(RectTransform));var r=(RectTransform)g.transform;r.SetParent(parent,false);r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;return r;}
  public static Image Picture(string name,Transform parent,Sprite sprite,Vector2 min,Vector2 max,Color? tint=null){var r=Rect(name,parent,min,max);var i=r.gameObject.AddComponent<Image>();i.sprite=sprite;if(sprite&&(sprite.name=="plaque"||sprite.name=="play"))i.type=Image.Type.Sliced;i.preserveAspect=sprite&&(sprite.name!="plaque"&&sprite.name!="play");i.color=tint??Color.white;i.raycastTarget=false;return i;}
  public static Text Label(string name,Transform parent,string text,Vector2 min,Vector2 max,int size=24,TextAnchor align=TextAnchor.MiddleCenter){var r=Rect(name,parent,min,max);var t=r.gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=size;t.alignment=align;t.color=new Color(1,.96f,.82f);t.resizeTextForBestFit=true;t.resizeTextMinSize=12;t.resizeTextMaxSize=size;t.raycastTarget=false;var o=r.gameObject.AddComponent<Outline>();o.effectColor=new Color(0,0,0,.8f);o.effectDistance=new Vector2(1,-1);return t;}
  public static Button Button(string name,Transform parent,Sprite sprite,string text,Vector2 min,Vector2 max,int size=24){var i=Picture(name,parent,sprite,min,max);i.raycastTarget=true;var b=i.gameObject.AddComponent<Button>();b.targetGraphic=i;var c=b.colors;c.highlightedColor=new Color(1,.95f,.72f);c.pressedColor=new Color(.68f,.85f,.93f);b.colors=c;if(!string.IsNullOrEmpty(text))Label("Texto",i.transform,text,Vector2.zero,Vector2.one,size);return b;}
  static System.Collections.Generic.Dictionary<string,Sprite> art;
  public static void ReloadArt(){art=null;}
  public static Sprite Art(string key){
   if(art==null){art=new System.Collections.Generic.Dictionary<string,Sprite>();foreach(var sp in Resources.LoadAll<Sprite>("Art"))art[sp.name]=sp;}
   return art.TryGetValue(key,out var found)?found:null;
  }
  public static void Build(Transform owner,WorldCatalog catalog){
   var cg=new GameObject("Interfaz",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));cg.transform.SetParent(owner,false);var canvas=cg.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
   var sc=cg.GetComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1600,1000);sc.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
   Picture("Fondo",cg.transform,null,Vector2.zero,Vector2.one,ink);
   var safe=Rect("AreaSegura",cg.transform,Vector2.zero,Vector2.one);safe.gameObject.AddComponent<SafeArea>();
   var viewport=Rect("MapaViewport",safe,new Vector2(.155f,.135f),new Vector2(1,.89f));viewport.gameObject.AddComponent<RectMask2D>();var hit=viewport.gameObject.AddComponent<Image>();hit.color=Color.clear;
   var map=Rect("Mapa",viewport,Vector2.zero,Vector2.one);var fit=map.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=16f/9;
   var bg=Picture("Ilustracion",map,Art("forest"),Vector2.zero,Vector2.one);var mat=Resources.Load<Material>("WaterAtmosphere");if(mat)bg.material=mat;
   EnvironmentComposer.Build(map);
   var gestures=viewport.gameObject.AddComponent<MapGestures>();gestures.content=map;
   for(int j=0;j<10;j++){
    var pos=catalog.levels[j].position;var r=Rect("Nivel_"+(j+1),map,pos,pos);r.sizeDelta=new Vector2(76,98);
    var b=Button("Boton",r,Art("node"),"" ,new Vector2(.04f,.24f),new Vector2(.96f,.96f));
    Label("Numero",b.transform,(j+1).ToString(),Vector2.zero,Vector2.one,30);
    var stars=Rect("Estrellas",r,new Vector2(-.12f,0),new Vector2(1.12f,.27f));
    for(int k=0;k<3;k++)Picture("Estrella_"+k,stars,Art("star"),new Vector2(k/3f,0),new Vector2((k+1)/3f,1),new Color(.3f,.35f,.4f));
    if(j==9)Label("Jefe",r,"JEFE",new Vector2(-.1f,-.24f),new Vector2(1.1f,0),20);
   }
   for(int j=0;j<6;j++){
    float top=.883f-j*.123f;var b=Button("Mundo_"+j,safe,Art("world"+j),"",new Vector2(.01f,top-.115f),new Vector2(.146f,top));
    if(j==0){var outline=b.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.2f,1,1,1);outline.effectDistance=new Vector2(3,-3);}
    Picture("Sombra",b.transform,null,new Vector2(0,0),new Vector2(1,.42f),new Color(0,.025f,.04f,.85f));
    Label("Nombre",b.transform,(j+1)+" · "+catalog.worlds[j],new Vector2(.02f,.01f),new Vector2(.98f,.4f),21);
    if(j>0)Label("Bloqueo",b.transform,"BLOQUEADO",new Vector2(0,.42f),new Vector2(1,.65f),17);
   }
   Label("Identidad",safe,"GUARDIANES\nDEL PORTAL",new Vector2(.01f,.895f),new Vector2(.18f,.995f),30);
   var title=Picture("Titulo",safe,Art("plaque"),new Vector2(.21f,.898f),new Vector2(.60f,.995f));
   Label("MundoTitulo",title.transform,"MUNDO 1\nBOSQUE PRIMORDIAL",new Vector2(.08f,.12f),new Vector2(.92f,.87f),28);
   string[] keys={"coin","blue","purple"};for(int j=0;j<3;j++){
    float x=.61f+j*.111f;Picture("SaldoPanel"+j,safe,null,new Vector2(x,.932f),new Vector2(x+.104f,.984f),ink);
    Picture("IconoSaldo"+j,safe,Art(keys[j]),new Vector2(x,.932f),new Vector2(x+.032f,.984f));
    Label("Saldo"+j,safe,"0",new Vector2(x+.031f,.932f),new Vector2(x+.10f,.984f),24);
   }
   Button("Ajustes",safe,Art("node"),"⚙",new Vector2(.951f,.916f),new Vector2(.994f,.99f),32);
   var bottom=Picture("Recompensas",safe,Art("plaque"),new Vector2(.16f,.015f),new Vector2(.765f,.13f));
   Label("TotalEstrellas",bottom.transform,"0 / 30\nESTRELLAS",new Vector2(.025f,.17f),new Vector2(.22f,.82f),26);
   var bar=Picture("ProgresoFondo",bottom.transform,null,new Vector2(.25f,.22f),new Vector2(.94f,.32f),new Color(.1f,.12f,.11f));
   var fill=Picture("Progreso",bar.transform,null,Vector2.zero,new Vector2(0,1),new Color(1,.71f,.13f));
   string[] chests={"bronze","silver","gold"};for(int j=0;j<3;j++){
    float x=.28f+j*.25f;var b=Button("Cofre_"+j,bottom.transform,Art(chests[j]),"",new Vector2(x,.25f),new Vector2(x+.16f,.96f));
    Label("Estado",b.transform,((j+1)*10)+" ★",new Vector2(-.1f,-.25f),new Vector2(1.1f,.1f),20);
   }
   Button("Entrar",safe,Art("play"),"ENTRAR",new Vector2(.78f,.015f),new Vector2(.99f,.13f),36);
   Label("Ayuda",safe,"Arrastra para explorar · Pellizca para acercar",new Vector2(.30f,.135f),new Vector2(.85f,.16f),14);
   var veil=Picture("Modal",safe,null,Vector2.zero,Vector2.one,new Color(.005f,.018f,.03f,.88f));veil.raycastTarget=true;
   var panel=Picture("PanelModal",veil.transform,Art("plaque"),new Vector2(.24f,.18f),new Vector2(.81f,.85f));
   Label("ModalTitulo",panel.transform,"",new Vector2(.1f,.75f),new Vector2(.9f,.91f),32);
   Label("ModalTexto",panel.transform,"",new Vector2(.12f,.32f),new Vector2(.88f,.75f),24);
   Button("ModalAccion",panel.transform,Art("play"),"JUGAR",new Vector2(.22f,.12f),new Vector2(.78f,.29f),27);
   Button("Cerrar",panel.transform,Art("node"),"×",new Vector2(.86f,.83f),new Vector2(.96f,.95f),32);
   veil.gameObject.SetActive(false);
   if(!Object.FindObjectOfType<EventSystem>()){
    var es=new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));es.transform.SetParent(owner,false);es.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
   }
  }
 }
}

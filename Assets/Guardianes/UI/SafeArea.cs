using UnityEngine;
namespace Guardianes {
 [ExecuteAlways] public class SafeArea:MonoBehaviour {
  Rect last; Vector2Int size;
  void Update(){Rect a=Screen.safeArea;var s=new Vector2Int(Screen.width,Screen.height);if(s==size&&a==last)return;last=a;size=s;if(s.x==0||s.y==0)return;var t=(RectTransform)transform;t.anchorMin=a.position/new Vector2(s.x,s.y);t.anchorMax=(a.position+a.size)/new Vector2(s.x,s.y);t.offsetMin=t.offsetMax=Vector2.zero;}
 }
}

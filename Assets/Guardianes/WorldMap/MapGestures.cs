using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Guardianes {
 public class MapGestures:MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IScrollHandler {
  public RectTransform content; Vector2 previous; float previousDistance; bool dragging; int fingers;
  public void OnBeginDrag(PointerEventData e){dragging=true;previous=e.position;}
  public void OnDrag(PointerEventData e){if(!dragging||fingers>1)return;var c=GetComponentInParent<Canvas>();content.anchoredPosition+=(e.position-previous)/c.scaleFactor;previous=e.position;Clamp();}
  public void OnEndDrag(PointerEventData e){dragging=false;}
  public void OnScroll(PointerEventData e){Zoom(content.localScale.x+e.scrollDelta.y*.08f);}
  void Update(){var t=Touchscreen.current;if(t==null)return;fingers=0;Vector2 a=default,b=default;foreach(var touch in t.touches)if(touch.press.isPressed){if(fingers==0)a=touch.position.ReadValue();if(fingers==1)b=touch.position.ReadValue();fingers++;}
   if(fingers==2){var rt=(RectTransform)transform;if(!RectTransformUtility.RectangleContainsScreenPoint(rt,a)||!RectTransformUtility.RectangleContainsScreenPoint(rt,b)){previousDistance=0;return;}float d=Vector2.Distance(a,b);if(previousDistance>0)Zoom(content.localScale.x*d/previousDistance);previousDistance=d;}else previousDistance=0;
  }
  void Zoom(float z){content.localScale=Vector3.one*Mathf.Clamp(z,1,1.65f);Clamp();}
  void Clamp(){var view=(RectTransform)transform;Vector2 max=Vector2.Max(Vector2.zero,(content.rect.size*content.localScale.x-view.rect.size)*.5f);content.anchoredPosition=new Vector2(Mathf.Clamp(content.anchoredPosition.x,-max.x,max.x),Mathf.Clamp(content.anchoredPosition.y,-max.y,max.y));}
 }
}

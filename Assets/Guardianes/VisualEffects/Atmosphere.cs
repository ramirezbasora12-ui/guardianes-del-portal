using UnityEngine;
using UnityEngine.UI;
namespace Guardianes {
 public class Atmosphere:MonoBehaviour {
  public enum Motion{Pulse,Float,Drift} public Motion motion; public float amount=6,speed=1; Vector3 origin; Image image; Color color;
  void OnEnable(){origin=transform.localPosition;image=GetComponent<Image>();if(image)color=image.color;}
  void Update(){float t=Time.unscaledTime*speed;float budget=QualitySettings.GetQualityLevel()==0?.45f:1;
   if(motion==Motion.Pulse){if(image)image.color=new Color(color.r,color.g,color.b,color.a*(.72f+.28f*Mathf.Sin(t)));}
   else transform.localPosition=origin+new Vector3(Mathf.Sin(t*.4f)*amount*budget,Mathf.Sin(t)*amount*budget,0);
  }
 }
}

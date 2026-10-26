using System;
using System.Linq;
namespace Guardianes {
 [Serializable] public class PlayerSettings { public float music=.7f,sfx=.8f; public int quality=1; public string language="es"; }
 [Serializable] public class PlayerData {
  public int version=1; public int[] stars=new int[10]; public bool[] claimed=new bool[3];
  public bool[] worlds={true,false,false,false,false,false};
  public int gold,blue,purple,selected=1; public PlayerSettings settings=new PlayerSettings();
  public int TotalStars {get{return stars.Sum();}}
  public bool IsUnlocked(int level){return level>=1&&level<=10&&(level==1||stars[level-2]>0);}
  public void Validate(){
   if(version!=1)throw new InvalidOperationException("Versión de guardado no compatible");
   if(stars==null||stars.Length!=10||claimed==null||claimed.Length!=3||worlds==null||worlds.Length!=6||settings==null)throw new InvalidOperationException("Guardado incompleto");
   for(int i=0;i<10;i++){if(stars[i]<0||stars[i]>3)throw new InvalidOperationException("Estrellas inválidas");if(i>0&&stars[i]>0&&stars[i-1]==0)throw new InvalidOperationException("Progresión inválida");}
   if(gold<0||blue<0||purple<0||selected<1||selected>10||!IsUnlocked(selected))throw new InvalidOperationException("Valores inválidos");
   if(float.IsNaN(settings.music)||float.IsInfinity(settings.music)||float.IsNaN(settings.sfx)||float.IsInfinity(settings.sfx))throw new InvalidOperationException("Volumen inválido");
   settings.music=Math.Max(0,Math.Min(1,settings.music));settings.sfx=Math.Max(0,Math.Min(1,settings.sfx));settings.quality=Math.Max(0,Math.Min(2,settings.quality));settings.language="es";
   worlds[0]=true;for(int i=1;i<6;i++)worlds[i]=(i==1&&stars[9]>0);
   for(int i=0;i<3;i++)if(claimed[i]&&TotalStars<(i+1)*10)throw new InvalidOperationException("Cofre sin progreso");
  }
  public void Complete(int level,int earned){
   if(!IsUnlocked(level))throw new InvalidOperationException("Nivel bloqueado");
   if(earned<1||earned>3)throw new ArgumentOutOfRangeException(nameof(earned));
   stars[level-1]=Math.Max(stars[level-1],earned);if(level==10)worlds[1]=true;
  }
  public bool Claim(int index,RewardDefinition reward){
   if(index<0||index>=3||reward==null||reward.stars!=(index+1)*10||reward.gold<0||reward.blue<0||reward.purple<0||claimed[index]||TotalStars<reward.stars)return false;
   int g=checked(gold+reward.gold),b=checked(blue+reward.blue),p=checked(purple+reward.purple);
   gold=g;blue=b;purple=p;claimed[index]=true;return true;
  }
 }
}

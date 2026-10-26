using System;
using UnityEngine;
namespace Guardianes {
 [Serializable] public class LevelDefinition {
  public string name; public int difficulty; public Vector2 position;
  public LevelDefinition(string n,int d,float x,float y){name=n;difficulty=d;position=new Vector2(x,y);}
 }
 [CreateAssetMenu(menuName="Guardianes/Catálogo de mundos")]
 public class WorldCatalog:ScriptableObject {
  public string[] worlds={"Bosque Primordial","Volcán Carmesí","Hielo Eterno","Desierto Ancestral","Tierras Sombrías","Dimensión Celestial"};
  public LevelDefinition[] levels={
   new LevelDefinition("Entrada al Bosque Primordial",1,0.2f,0.245f),new LevelDefinition("Sendero Antiguo",1,0.345f,0.3f),
   new LevelDefinition("Cascadas de Cristal",2,0.465f,0.37f),new LevelDefinition("Puente Roto",2,0.58f,0.47f),
   new LevelDefinition("Ruinas Olvidadas",2,0.73f,0.36f),new LevelDefinition("Bosque Susurrante",3,0.835f,0.55f),
   new LevelDefinition("Puesto del Guardián",3,0.66f,0.565f),new LevelDefinition("Río Sagrado",3,0.775f,0.645f),
   new LevelDefinition("Bosque Corrupto",4,0.855f,0.73f),new LevelDefinition("El Guardián Antiguo — Jefe",5,0.91f,0.84f)};
  public RewardDefinition[] rewards={new RewardDefinition{stars=10,gold=100,blue=5},new RewardDefinition{stars=20,gold=250,blue=15},new RewardDefinition{stars=30,gold=500,blue=30,purple=3}};
  public string[] gameplayScenes=new string[10];
  public string ValidateData(){
   if(worlds==null||worlds.Length!=6||levels==null||levels.Length!=10||rewards==null||rewards.Length!=3)return "Cantidad de mundos, niveles o cofres incorrecta";
   for(int i=0;i<10;i++)if(levels[i]==null||string.IsNullOrWhiteSpace(levels[i].name)||levels[i].difficulty<1||levels[i].difficulty>5||levels[i].position.x<0||levels[i].position.x>1||levels[i].position.y<0||levels[i].position.y>1)return "Nivel inválido: "+i;
   for(int i=0;i<3;i++)if(rewards[i]==null||rewards[i].stars!=(i+1)*10||rewards[i].gold<0||rewards[i].blue<0||rewards[i].purple<0)return "Recompensa inválida";
   return null;
  }
 }
}

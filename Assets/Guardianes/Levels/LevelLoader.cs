using UnityEngine.SceneManagement;
namespace Guardianes {
 public static class LevelLoader {
  public static bool TryLoad(WorldCatalog catalog,PlayerData data,int level){
   if(!data.IsUnlocked(level))return false;
   var scenes=catalog.gameplayScenes;string scene=scenes!=null&&scenes.Length>=level?scenes[level-1]:null;
   if(string.IsNullOrEmpty(scene)||!UnityEngine.Application.CanStreamedLevelBeLoaded(scene))return false;
   SceneManager.LoadSceneAsync(scene);return true;
  }
 }
}

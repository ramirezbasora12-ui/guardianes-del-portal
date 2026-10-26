using UnityEngine;
using UnityEngine.Profiling;
namespace Guardianes {
 public class RuntimePerformance:MonoBehaviour {
  float elapsed;int frames;float worst;
  void Update(){elapsed+=Time.unscaledDeltaTime;frames++;worst=Mathf.Max(worst,Time.unscaledDeltaTime);if(elapsed<10)return;
   Debug.Log($"[Guardianes rendimiento real] FPS medios: {frames/elapsed:F1}; peor fotograma: {worst*1000:F1} ms; memoria asignada Unity: {Profiler.GetTotalAllocatedMemoryLong()/1048576} MB; dispositivo: {SystemInfo.deviceModel}");elapsed=0;frames=0;worst=0;
  }
 }
}

using System;
using UnityEngine;
namespace Guardianes
{
    [Serializable] public class EffectDefinition
    {
        public string key, sheet;
        public float u, v, w, h, speed, intensity;
        public int mode;
        public Rect Region { get { return new Rect(u, v, w, h); } }
    }
    [Serializable] public class LayerPlacement
    {
        public string name, effect;
        public float x, y, width, height, alpha, rotation;
        public int minQuality;
    }
    [Serializable] public class EnvironmentPlan
    {
        public int version;
        public EffectDefinition[] effects;
        public LayerPlacement[] layers;
        public EffectDefinition Effect(string key)
        {
            foreach (var effect in effects) if (effect.key == key) return effect;
            throw new InvalidOperationException("Efecto desconocido: " + key);
        }
        public static EnvironmentPlan Load()
        {
            var json = Resources.Load<TextAsset>("EnvironmentLayers");
            if (!json) throw new InvalidOperationException("Falta EnvironmentLayers.json");
            return JsonUtility.FromJson<EnvironmentPlan>(json.text);
        }
    }
}

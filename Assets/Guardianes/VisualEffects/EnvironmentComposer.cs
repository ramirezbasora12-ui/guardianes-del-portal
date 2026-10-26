using UnityEngine;
using UnityEngine.UI;
namespace Guardianes
{
    public static class EnvironmentComposer
    {
        public static RawImage EffectImage(string name, Transform parent, EffectDefinition effect)
        {
            var rect = MapView.Rect(name, parent, Vector2.zero, Vector2.one);
            var image = rect.gameObject.AddComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>("Art/" + effect.sheet);
            image.uvRect = effect.Region;
            image.material = Resources.Load<Material>("Effects/" + effect.key);
            image.raycastTarget = false;
            return image;
        }
        public static void Build(RectTransform map)
        {
            var plan = EnvironmentPlan.Load();
            var root = MapView.Rect("CapasAmbientales", map, Vector2.zero, Vector2.one);
            foreach (var layer in plan.layers)
            {
                var effect = EffectImage(layer.name, root, plan.Effect(layer.effect));
                var rect = effect.rectTransform;
                rect.anchorMin = new Vector2(layer.x - layer.width * .5f, layer.y - layer.height * .5f);
                rect.anchorMax = new Vector2(layer.x + layer.width * .5f, layer.y + layer.height * .5f);
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                rect.localRotation = Quaternion.Euler(0, 0, layer.rotation);
                effect.color = new Color(1, 1, 1, layer.alpha);
                var quality = rect.gameObject.AddComponent<LayerQuality>();
                quality.minimum = layer.minQuality;
                if (layer.effect == "mist")
                {
                    var drift = rect.gameObject.AddComponent<Atmosphere>();
                    drift.motion = Atmosphere.Motion.Drift; drift.amount = 8; drift.speed = .22f;
                }
                if (layer.name == "PortalSiguiente")
                {
                    effect.raycastTarget = true;
                    var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = effect;
                    // Kept on a separate layer above the decorative overlays, hidden by progression at runtime.
                }
            }
            var particles = MapView.Rect("ParticulasReutilizables", map, Vector2.zero, Vector2.one);
            particles.gameObject.AddComponent<AmbientPool>();
        }
    }
}

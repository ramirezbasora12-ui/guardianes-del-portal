using UnityEngine;
using UnityEngine.UI;
namespace Guardianes
{
    // Fixed allocation at startup, reused for the entire map session. No per-frame Instantiate/Destroy.
    public sealed class AmbientPool : MonoBehaviour
    {
        sealed class Mote { public RawImage image; public Vector2 origin; public float phase, life, age; }
        readonly Mote[] pool = new Mote[18];
        System.Random random;
        float Next() { return (float)random.NextDouble(); }
        void Start()
        {
            random = new System.Random(1471);
            var plan = EnvironmentPlan.Load();
            for (int i = 0; i < pool.Length; i++)
            {
                var image = EnvironmentComposer.EffectImage("Luciérnaga_" + i, transform,
                    plan.Effect(i % 4 == 0 ? "sparkle" : "firefly"));
                var mote = new Mote { image = image }; pool[i] = mote; Reset(mote);
                mote.age = Next() * mote.life;
            }
        }
        void Reset(Mote mote)
        {
            mote.origin = new Vector2(.08f + Next() * .84f, .06f + Next() * .80f);
            mote.phase = Next() * 6.28f; mote.life = 5 + Next() * 7; mote.age = 0;
            float size = .007f + Next() * .004f;
            mote.image.rectTransform.sizeDelta = Vector2.zero;
            mote.image.rectTransform.anchorMin = mote.origin;
            mote.image.rectTransform.anchorMax = mote.origin + new Vector2(size, size * 1.7778f);
        }
        void Update()
        {
            int count = QualitySettings.GetQualityLevel() == 0 ? 6 : QualitySettings.GetQualityLevel() == 1 ? 12 : 18;
            for (int i = 0; i < pool.Length; i++)
            {
                var mote = pool[i]; if (mote == null) continue;
                mote.image.enabled = i < count;
                if (i >= count) continue;
                mote.age += Time.unscaledDeltaTime;
                if (mote.age > mote.life) Reset(mote);
                float t = mote.age / mote.life;
                mote.image.rectTransform.anchoredPosition = new Vector2(Mathf.Sin(mote.age + mote.phase) * 7, t * 16);
                mote.image.color = new Color(1, 1, 1, Mathf.Sin(t * Mathf.PI) * .42f);
            }
        }
    }
}

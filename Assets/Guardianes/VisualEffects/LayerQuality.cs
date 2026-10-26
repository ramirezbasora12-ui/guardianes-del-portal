using UnityEngine;
using UnityEngine.UI;
namespace Guardianes
{
    public sealed class LayerQuality : MonoBehaviour
    {
        public int minimum;
        RawImage image; int previous = -1;
        void OnEnable() { image = GetComponent<RawImage>(); previous = -1; }
        void Update()
        {
            int quality = QualitySettings.GetQualityLevel();
            if (previous == quality) return;
            previous = quality; image.enabled = quality >= minimum;
        }
    }
}

using MD.Diggable.Gem;
using UnityEngine;

namespace MD.VisualEffects
{
    public class GemObtainParticles : MonoBehaviour
    {
        [System.Serializable]
        public struct GemEntry
        {
            public DiggableType type;
            public Texture sprite;
        }

        private string TEXTURE_NAME = "_MainTex";

        [SerializeField]
        private ParticleSystem obtainParticles = null;

        [SerializeField]
        private Material gemMaterial = null;

        [SerializeField]
        private GemEntry[] spriteTable = null;

        private void Start() 
        {
            EventSystems.EventManager.Instance.StartListening<GemObtainData>(OnGemDig);
        }

        private void OnDisable() 
        {
            EventSystems.EventManager.Instance.StopListening<GemObtainData>(OnGemDig);
        }

        private void OnGemDig(GemObtainData gemObtainData)
        {
            spriteTable.Find(entry => entry.type.Equals(gemObtainData.type)).Match(matched => gemMaterial.SetTexture(TEXTURE_NAME, matched.sprite), () => {});
            obtainParticles.Play();
        }   
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace MD.UI
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField]
        private Slider slider = null;

        private void Start()
        {
            EventSystems.EventConsumer.GetOrAttach(gameObject).StartListening<Character.HPChangeData>(UpdateView);
        }

        private void UpdateView(Character.HPChangeData data)
        {
            slider.value =  (float) data.curHP / (float) data.maxHP;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace MD.VisualEffects
{
    public class TextPopup : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 60f;

        [SerializeField]
        private float _fadeSpeed = 1f;

        [SerializeField]
        private Vector2 _offset = Vector2.zero;

        [SerializeField]
        private Text _text = null;

        private System.Action<TextPopup> OnFadeOut;

        public void Play(string value, Vector2 pos, Color color, System.Action<TextPopup> onFadeOutCallback)
        {
            _text.color = new Color(color.r, color.g, color.b, 1f);
            _text.rectTransform.position = pos + _offset;
            OnFadeOut = onFadeOutCallback;
        }

        void Update()
        {
            if (_text.color.a <= 0f)
            {                      
                return;
            }

            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 
                            Mathf.MoveTowards(_text.color.a, 0f, _fadeSpeed * Time.deltaTime));

            _text.rectTransform.position += new Vector3(0f, _moveSpeed * Time.deltaTime, 0f);
                
            if (_text.color.a <= 0f)
            {                
                OnFadeOut?.Invoke(this);
            }        
        }
    }
}

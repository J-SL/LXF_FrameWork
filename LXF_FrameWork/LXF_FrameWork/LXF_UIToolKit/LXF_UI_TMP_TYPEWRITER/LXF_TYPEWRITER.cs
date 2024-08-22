using Cysharp.Threading.Tasks;
using LXF_Framework.MonoYield;
using System.Text;
using TMPro;
using UnityEngine;

namespace LXF_UI_TMP_TYPEWRITER
{
    public class LXF_TYPEWRITER : LXF_MonoYield
    {
        private TMP_Text m_Text;

        [Header("Typewriter Settings")]
        [SerializeField]
        [Range(0.05f, 0.5f)]
        private float _typewriterSpeed = 0.05f;
        [Space(20)]
        [SerializeField]
        private string _originalText = "Original Text";

        [SerializeField]
        private string ShowText = "Show Text";




        private void Awake()
        {         
            if (!TryGetComponent(out m_Text))
            {
                Debug.LogError("LXF_TYPEWRITER: TextMeshPro component not found on object " + gameObject.name);
                return;
            }      
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// Typewriter effect with character swap other characters 
        /// </summary>
        public async UniTask TypewriterSwapAsync(char[] recordtext)
        {
            StringBuilder sb = new StringBuilder(m_Text.text);
            int letterIndex = 0;
            foreach (char letter in recordtext)
            {
                if (letterIndex < sb.Length)
                {
                    sb[letterIndex] = letter;
                }
                letterIndex++;

                m_Text.text = sb.ToString();


                await UniTask.Delay((int)(_typewriterSpeed * 1000));
            }           
        }


        /// <summary>
        /// simple typewriter effect which might be let text offset by one character
        /// </summary>
        public async UniTask TypewriterAsync(string text)
        {
            m_Text.text = _originalText;

            foreach (char letter in text)
            {            
                m_Text.text += letter; 
                await UniTask.Delay((int)(_typewriterSpeed * 1000)); 
            }
        }

        [ContextMenu("Typewriter")]
        public async UniTask TypewriterAsync() => await TypewriterAsync(ShowText);
      
    }
}
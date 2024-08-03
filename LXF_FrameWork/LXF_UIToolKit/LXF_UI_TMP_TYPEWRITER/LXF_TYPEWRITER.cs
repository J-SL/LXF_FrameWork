using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace LXF_UI_TMP_TYPEWRITER
{
    public class LXF_TYPEWRITER : MonoBehaviour
    {
        private TMP_Text m_Text;

        [Header("Typewriter Settings")]
        [SerializeField]
        [Range(0.05f, 0.5f)]
        private float _typewriterSpeed = 0.05f;
        [Space(20)]
        [SerializeField]
        private string _originalText = "Original Text";





        private void Awake()
        {         
            if (!TryGetComponent(out m_Text))
            {
                Debug.LogError("LXF_TYPEWRITER: TextMeshPro component not found on object " + gameObject.name);
                return;
            }      
        }

        public async UniTask TypewriterAsync(string text)
        {
            m_Text.text = _originalText;

            foreach (char letter in text)
            {            
                m_Text.text += letter; 
                await UniTask.Delay((int)(_typewriterSpeed * 1000)); 
            }
        }
    }
}
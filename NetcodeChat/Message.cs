using TMPro;
using UnityEngine;

namespace NetcodeChat
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textSource;

        public void SetText(string text)
        {
            _textSource.text = text;
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NetcodeChat
{
    public class ChatWindow : MonoBehaviour
    {
        [SerializeField] private Message _messageTemplate;
        [SerializeField] private TMP_InputField _inputField;

        private ChatHandler _chatHandler;
        private ScrollRect _scrollRect;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void Init(ChatHandler chatHandler)
        {
            _chatHandler = chatHandler;
            _chatHandler.MessageRecived += AddMessage;
        }

        private void OnEnable()
        {
            _inputField.onSubmit.AddListener(PushMessage);
        }

        private void OnDisable()
        {
            _inputField.onSubmit.RemoveListener(PushMessage);
        }

        private void OnDestroy()
        {
            if (_chatHandler != null)
                _chatHandler.MessageRecived -= AddMessage;
        }

        public void PushMessage(string message)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _inputField.ActivateInputField();
                _inputField.MoveTextEnd(false);
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
                return;

            StartCoroutine(ResetInput());
            _chatHandler.SendMessageServerRpc(message);
        }

        public void AddMessage(string senderName, string message, Color senderNameColor)
        {
            var messageObject = Instantiate(_messageTemplate, _scrollRect.content);
            messageObject.SetText($"[<color=#{ColorUtility.ToHtmlStringRGB(senderNameColor)}>{senderName}</color>]: {message}");
            StartCoroutine(ScrollDown());
        }

        private IEnumerator ResetInput()
        {
            yield return null;
            _inputField.SetTextWithoutNotify(null);
            yield return null;
            _inputField.ActivateInputField();
        }

        private IEnumerator ScrollDown()
        {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 0;
        }
    }
}
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NetcodeChat.Examples
{
    public class Example : NetworkBehaviour
    {
        [SerializeField] private bool _isHost = true;
        [SerializeField] private ChatWindow _chatWindow;
        [SerializeField] private LobbyExample _lobbyExampleTemplate;
        [SerializeField] private ChatHandler _chatHandlerTemplate;

        private NetworkVariable<NetworkBehaviourReference> _chatHandlerReference = 
            new NetworkVariable<NetworkBehaviourReference>();

        private void Start()
        {
            if (_isHost == false)
            {
                NetworkManager.Singleton.StartClient();
                return;
            }

            var started = NetworkManager.Singleton.StartHost();
            if (started == false)
                return;

            var chatHandler = GameObject.Instantiate(_chatHandlerTemplate);
            var lobby = GameObject.Instantiate(_lobbyExampleTemplate);

            lobby.ClientConnected += InitClient;
            lobby.ClientConnected += (clientId) => chatHandler.AddUser(lobby.CreateChatUser(clientId));
            lobby.ClientDisconnected += chatHandler.RemoveUser;

            chatHandler.NetworkObject.Spawn();
            lobby.NetworkObject.Spawn();

            chatHandler.Init(
                new List<ChatCommand>() {
                    new ChatCommand { Predicate = "votekick", RequiredVotes = VotingType.Half, Action = lobby.Kick }
                }
            );

            _chatHandlerReference.Value = chatHandler;
            lobby.StartLobby();
        }

        private void InitClient(ulong clientId)
        {
            var target = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            OnClientConnectedClientRpc(target);
        }

        [ClientRpc]
        private void OnClientConnectedClientRpc(ClientRpcParams clientRpcParams)
        {
            _chatHandlerReference.Value.TryGet(out ChatHandler chatHandler);
            _chatWindow.Init(chatHandler);
        }
    }
}
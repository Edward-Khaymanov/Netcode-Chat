using System;
using Unity.Netcode;

namespace NetcodeChat.Examples
{
    public class LobbyExample : NetworkBehaviour
    {
        public event Action<ulong> ClientConnected;
        public event Action<ulong> ClientDisconnected;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (base.NetworkManager.IsServer)
            {
                base.NetworkManager.OnClientConnectedCallback += OnClientConnected;
                base.NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (base.NetworkManager.IsServer)
            {
                base.NetworkManager.OnClientConnectedCallback -= OnClientConnected;
                base.NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }

        public void StartLobby()
        {
            // Some actions 
            // ...
            // ...
            OnClientConnected(NetworkManager.ServerClientId);
        }

        public void Kick(ulong clientId)
        {
            if (clientId == ulong.MaxValue || clientId == NetworkManager.ServerClientId)
                return;

            NetworkManager.Singleton.DisconnectClient(clientId, "Voting");
            // When you manually disconnect client NetworkManager.OnClientDisconnectCallback doesn't invoking
            OnClientDisconnected(clientId);
        }

        public ChatUser CreateChatUser(ulong clientId)
        {
            return new ChatUser()
            {
                NetworkClieintId = clientId,
                Name = UnityEngine.Random.Range(0, 1111).ToString(),
                Color = UnityEngine.Random.ColorHSV()
            };
        }

        private void OnClientConnected(ulong clientId)
        {
            // Some actions 
            // ...
            // ...
            ClientConnected?.Invoke(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            // Some actions 
            // ...
            // ...
            ClientDisconnected?.Invoke(clientId);
        }
    }
}

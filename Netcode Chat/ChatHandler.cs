using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;
using UnityEngine;

namespace NetcodeChat
{
    public class ChatHandler : NetworkBehaviour
    {
        public Color SystemMessageColor = Color.red;
        public Color OwnerMessageColor = Color.green;
        public bool ShowOnJoinMessage = true;
        public bool ShowOnLeaveMessage = true;
        public string OnJoinMessage = "joined";
        public string OnOnLeaveMessage = "left";

        private readonly List<ChatUser> _users = new List<ChatUser>();
        private readonly CommandHandler _commandHandler = new CommandHandler();
        private List<ChatCommand> _commands;

        public event Action<string, string, Color> MessageRecived;

        public void Init(List<ChatCommand> chatCommands = null)
        {
            _commands = chatCommands;
        }

        public void AddUser(ChatUser chatUser)
        {
            _users.Add(chatUser);
            _commandHandler.AddUser();
            if (ShowOnJoinMessage)
                SystemMessage($"{chatUser.Name} {OnJoinMessage}");
        }

        public void RemoveUser(ulong networkClientId)
        {
            var user = _users.FirstOrDefault(x => x.NetworkClieintId == networkClientId);
            _users.Remove(user);
            _commandHandler.RemoveUser(networkClientId);
            if (ShowOnLeaveMessage)
                SystemMessage($"{user.Name} {OnOnLeaveMessage}");
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendMessageServerRpc(string message, ServerRpcParams rpcParams = default)
        {
            var requireUser = _users.FirstOrDefault(x => x.NetworkClieintId == rpcParams.Receive.SenderClientId);
            if (requireUser == null)
                return;

            message = FilterMessage(message);
            var request = TryGetRequest(message, requireUser);

            ReceiveMessageClientRpc($"{requireUser.NetworkClieintId}_{requireUser.Name}", message, requireUser.Color, GetAllClientsExcept(requireUser.NetworkClieintId));
            ReceiveMessageClientRpc($"<b>{requireUser.NetworkClieintId}_{requireUser.Name}</b>", message, OwnerMessageColor, GetClient(requireUser.NetworkClieintId));

            if (request != null)
                _commandHandler.AddRequest(request);
        }

        [ClientRpc]
        private void ReceiveMessageClientRpc(string senderName, string message, Color senderNameColor, ClientRpcParams rpcParams)
        {
            MessageRecived?.Invoke(senderName, message, senderNameColor);
        }

        private string FilterMessage(string message)
        {
            var result = message;

            result = Regex.Replace(result, @"[ ]{2,}", " ", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^[ ]", string.Empty, RegexOptions.Multiline);
            result = Regex.Replace(result, @"\v{3,}", "\v\v", RegexOptions.Multiline);
            result = Regex.Replace(result, @"^\n$", string.Empty, RegexOptions.Multiline);
            result = result.TrimStart('\n');

            return result;
        }

        private void SystemMessage(string message)
        {
            ReceiveMessageClientRpc("<b>System</b>", message, SystemMessageColor, GetAllClients());
        }

        private CommandRequest TryGetRequest(string message, ChatUser requireUser)
        {
            if (_commands == null || _commands.Count == 0)
                return null;

            var match = Regex.Match(message, @"^(/\w*) (\d{1,})(?:.*)");
            if (match.Success == false)
                return null;

            var command = _commands.FirstOrDefault(x => match.Groups[1].Value == $"/{x.Predicate}");
            if (command == null)
                return null;

            var isClientId = ulong.TryParse(match.Groups[2]?.Value, out ulong clientId);
            if (isClientId == false)
                return null;

            var targetUser = _users.FirstOrDefault(x => x.NetworkClieintId == clientId);
            if (targetUser == null)
                return null;

            var request = new CommandRequest()
            {
                ChatCommand = command,
                TargetUser = targetUser,
                RequireUser = requireUser,
                OptionalData = match.Groups[3].Value ?? null
            };

            return request;
        }

        private ClientRpcParams GetAllClients()
        {
            var target = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds
                }
            };

            return target;
        }

        private ClientRpcParams GetAllClientsExcept(ulong exceptedClientId)
        {
            var target = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != exceptedClientId).ToArray()
                }
            };

            return target;
        }

        private ClientRpcParams GetClient(ulong clientId)
        {
            var target = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            return target;
        }
    }
}
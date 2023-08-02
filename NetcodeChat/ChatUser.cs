using UnityEngine;

namespace NetcodeChat
{
    public class ChatUser
    {
        public ulong NetworkClieintId { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
    }
}
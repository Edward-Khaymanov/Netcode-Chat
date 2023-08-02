using System;

namespace NetcodeChat
{
    public class CommandRequest : IEquatable<CommandRequest>
    {
        public ChatCommand ChatCommand { get; set; }
        public ChatUser TargetUser { get; set; }
        public ChatUser RequireUser { get; set; }
        public string OptionalData { get; set; }

        public bool Equals(CommandRequest other)
        {
            var result = false;
            if (other == null)
                return result;

            result = this.ChatCommand.Predicate == other.ChatCommand.Predicate;
            if (result == false)
                return result;
            result = this.TargetUser.NetworkClieintId == other.TargetUser.NetworkClieintId;
            if (result == false)
                return result;
            result = this.RequireUser.NetworkClieintId == other.RequireUser.NetworkClieintId;
            if (result == false)
                return result;
            result = this.OptionalData == other.OptionalData;

            return result;
        }
    }
}
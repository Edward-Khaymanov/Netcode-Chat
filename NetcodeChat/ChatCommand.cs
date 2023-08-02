using System;

namespace NetcodeChat
{
    public class ChatCommand
    {
        public string Predicate { get; set; }
        public VotingType RequiredVotes { get; set; }
        public Action<ulong> Action { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetcodeChat
{
    public class CommandHandler
    {
        private readonly List<CommandRequest> _requests = new List<CommandRequest>();
        private int _maxUsers;

        public void AddUser()
        {
            _maxUsers++;
        }

        public void RemoveUser(ulong clientId)
        {
            _maxUsers--;
            _requests.RemoveAll(x => x.RequireUser.NetworkClieintId == clientId || x.TargetUser.NetworkClieintId == clientId);
            CheckRequests();
        }

        public void AddRequest(CommandRequest request)
        {
            var isExist = _requests.Any(x => x.Equals(request));
            if (isExist)
                return;

            _requests.Add(request);
            TryExecuteRequest(request);
        }

        private bool TryExecuteRequest(CommandRequest request)
        {
            var targetRequests = _requests
                .Where(x =>
                    x.ChatCommand.Predicate == request.ChatCommand.Predicate &&
                    x.TargetUser == request.TargetUser)
                .ToList();

            var requiredVotes = GetRequiredVotesCount(request.ChatCommand.RequiredVotes);
            var canExecute = targetRequests.Count() >= requiredVotes;
            if (canExecute)
            {
                request.ChatCommand.Action?.Invoke(request.TargetUser.NetworkClieintId);
                targetRequests.ForEach(x => _requests.Remove(x));
            }

            return canExecute;
        }

        private void CheckRequests()
        {
            var commands = _requests.Select(x => x.ChatCommand).Distinct().ToList();
            var targetUsers = _requests.Select(x => x.TargetUser).Distinct().ToList();

            foreach (var command in commands)
            {
                var requiredVotes = GetRequiredVotesCount(command.RequiredVotes);
                foreach (var user in targetUsers)
                {
                    var targetRequests = _requests.Where(x => x.ChatCommand == command && x.TargetUser == user);
                    var canExecute = targetRequests.Count() >= requiredVotes;
                    if (canExecute)
                    {
                        command.Action?.Invoke(user.NetworkClieintId);
                        targetUsers.RemoveAll(x => x.NetworkClieintId == user.NetworkClieintId);
                    }
                }
            }
        }

        private int GetRequiredVotesCount(VotingType type)
        {
            switch (type)
            {
                case VotingType.All:
                    return _maxUsers;
                case VotingType.ExceptTarget:
                    return _maxUsers - 1;
                case VotingType.Half:
                    return (int)MathF.Round(_maxUsers / 2f, MidpointRounding.AwayFromZero);
                case VotingType.Quarter:
                    return (int)MathF.Round(_maxUsers / 4f, MidpointRounding.AwayFromZero);
                case VotingType.One:
                    return 1;
                default:
                    return int.MaxValue;
            }
        }
    }
}
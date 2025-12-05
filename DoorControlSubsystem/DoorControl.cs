using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EV_SCADA.Modules
{
    public enum DoorState { Locked, Unlocking, Unlocked, Locking, Fault }

    public class DoorControl
    {
        public DoorState State { get; private set; } = DoorState.Locked;

        private readonly Dictionary<string, AccessToken> _tokens = new();
        public List<AccessEvent> Logs { get; } = new();

        private Timer? _autoRelockTimer;
        private int _failedAttempts = 0;

        public int FailedAttempts => _failedAttempts;

        public event Action<DoorState>? OnStateChanged;

        public bool RevokeToken(string tokenId)
        {
            if (_tokens.ContainsKey(tokenId))
            {
                _tokens.Remove(tokenId);
                Logs.Add(new AccessEvent { TokenId = tokenId, Action = "RevokeToken", Result = "Success" });
                return true;
            }

            Logs.Add(new AccessEvent { TokenId = tokenId, Action = "RevokeToken", Result = "Failed" });
            return false;
        }

        public AccessToken GenerateToken(TimeSpan duration)
        {
            var token = new AccessToken
            {
                Id = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.Add(duration)
            };
            _tokens[token.Id] = token;
            return token;
        }

        public List<AccessToken> GetActiveTokens()
        {
            var list = new List<AccessToken>();
            foreach (var t in _tokens.Values)
            {
                if (t.IsValid() && !t.IsUsed)
                    list.Add(t);
            }
            return list;
        }

        public async Task<bool> UnlockDoorAsync(string tokenId)
        {
            if (!_tokens.ContainsKey(tokenId) || !_tokens[tokenId].IsValid())
            {
                _failedAttempts++;
                Logs.Add(new AccessEvent { TokenId = tokenId, Action = "Unlock", Result = "Denied" });
                return false;
            }

            _tokens[tokenId].IsUsed = true;
            Logs.Add(new AccessEvent { TokenId = tokenId, Action = "Unlock", Result = "Success" });

            State = DoorState.Unlocking;
            OnStateChanged?.Invoke(State);

            await Task.Delay(700);

            State = DoorState.Unlocked;
            OnStateChanged?.Invoke(State);

            StartAutoRelock(5);
            return true;
        }

        private void StartAutoRelock(int seconds)
        {
            _autoRelockTimer?.Dispose();
            _autoRelockTimer = new Timer(async _ =>
            {
                await LockDoorAsync();
            }, null, seconds * 1000, Timeout.Infinite);
        }

        public async Task LockDoorAsync()
        {
            State = DoorState.Locking;
            OnStateChanged?.Invoke(State);

            await Task.Delay(700);

            State = DoorState.Locked;
            OnStateChanged?.Invoke(State);

            Logs.Add(new AccessEvent { Action = "Lock", Result = "Success" });
        }

        public void ManualOverride()
        {
            State = DoorState.Unlocked;
            Logs.Add(new AccessEvent { Action = "ManualOverride", Result = "Unlocked" });
            OnStateChanged?.Invoke(State);
        }

        public void TriggerFault()
        {
            State = DoorState.Fault;
            Logs.Add(new AccessEvent { Action = "Fault", Result = "SystemError" });
            OnStateChanged?.Invoke(State);
        }
    }
}

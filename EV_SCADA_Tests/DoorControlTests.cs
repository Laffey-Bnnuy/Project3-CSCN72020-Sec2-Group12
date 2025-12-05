using System;
using System.Threading.Tasks;
using EV_SCADA.Modules;
using Xunit;

namespace EV_SCADA_Tests
{
    public class DoorControlTests
    {
        [Fact]
        public void Door_InitiallyLocked()
        {
            var door = new DoorControl();
            Assert.Equal(DoorState.Locked, door.State);
        }

        [Fact]
        public async Task Unlock_Succeeds_WithValidToken()
        {
            var door = new DoorControl();
            var token = door.GenerateToken(TimeSpan.FromSeconds(5));

            bool result = await door.UnlockDoorAsync(token.Id);

            Assert.True(result);
            Assert.Equal(DoorState.Unlocked, door.State);
        }

        [Fact]
        public async Task Unlock_Fails_WithInvalidToken()
        {
            var door = new DoorControl();

            bool result = await door.UnlockDoorAsync("bad-id");

            Assert.False(result);
            Assert.Equal(1, door.FailedAttempts);
        }

        [Fact]
        public void TokenRevocation_Works()
        {
            var door = new DoorControl();
            var token = door.GenerateToken(TimeSpan.FromSeconds(10));

            bool revoked = door.RevokeToken(token.Id);

            Assert.True(revoked);
            Assert.Empty(door.GetActiveTokens());
        }

        [Fact]
        public void GetActiveTokens_ReturnsOnlyValidTokens()
        {
            var door = new DoorControl();
            var token1 = door.GenerateToken(TimeSpan.FromSeconds(5));
            var token2 = door.GenerateToken(TimeSpan.FromSeconds(5));
            token1.IsUsed = true;

            var active = door.GetActiveTokens();

            Assert.Single(active);
        }
    }
}

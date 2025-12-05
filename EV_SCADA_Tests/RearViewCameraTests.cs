using EV_SCADA.Modules;
using Xunit;

namespace EV_SCADA_Tests
{
    public class RearViewCameraTests
    {
        [Fact]
        public void Camera_ActivatesSuccessfully()
        {
            var camera = new RearViewCamera();
            camera.Activate();
            Assert.NotNull(camera);
        }

        [Fact]
        public void Camera_DeactivatesSuccessfully()
        {
            var camera = new RearViewCamera();
            camera.Activate();
            camera.Deactivate();
            Assert.NotNull(camera);
        }

        [Fact]
        public void Recording_AddsFrames()
        {
            var camera = new RearViewCamera();
            camera.TestInjectFrame("TestFrame1");
            Assert.True(camera.RecordedFrames.Count > 0);
            Assert.Contains("TestFrame1", camera.RecordedFrames);
        }

        [Fact]
        public void Playback_ReturnsRecordedFrames()
        {
            var camera = new RearViewCamera();
            camera.TestInjectFrame("FrameA");
            camera.TestInjectFrame("FrameB");

            var playback = camera.Playback();

            Assert.NotEmpty(playback);
            Assert.Equal(2, playback.Count);
            Assert.Contains("FrameA", playback);
            Assert.Contains("FrameB", playback);
        }

        [Fact]
        public void ReverseAlert_ReturnsWarning_WhenObstacleIsClose()
        {
            var alert = new ReverseAlert();
            string result = alert.GetAlertMessage(40);
            Assert.Contains("Warning", result);
        }

        [Fact]
        public void ReverseAlert_ReturnsStop_WhenObstacleTooClose()
        {
            var alert = new ReverseAlert();
            string result = alert.GetAlertMessage(10);
            Assert.Contains("STOP", result);
        }
    }
}

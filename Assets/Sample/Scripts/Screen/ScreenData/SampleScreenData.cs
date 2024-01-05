using Sabanishi.ScreenSystem;

namespace Sabanishi.ScreenSystemSample
{
    public class SampleScreenData : IScreenData
    {
        public string Message { get; }

        public SampleScreenData(string message)
        {
            Message = message;
        }
    }
}
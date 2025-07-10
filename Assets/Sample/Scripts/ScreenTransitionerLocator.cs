using Sabanishi.ScreenSystem;

namespace Sabanishi.ScreenSystemSample
{
    /// <summary>
    /// ScreenTransitionをシングルトンとして公開するためのクラス
    /// </summary>
    public class ScreenTransitionerLocator
    {
        private static ScreenTransitionerLocator _instance;
        public static ScreenTransitionerLocator Instance => _instance;

        public ScreenTransitioner TopLayerTransitioner { get; private set; }

        public void Initialize()
        {
            _instance = this;
            TopLayerTransitioner = new ScreenTransitioner(null, null);
        }
    }
}
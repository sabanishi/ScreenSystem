using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystemSample
{
    public class SceneInitializer : MonoBehaviour
    {
        private void Start()
        {
            var topLayerTransitioner = new ScreenTransitionerLocator();
            topLayerTransitioner.Initialize();
            var to = ScreenGenerator.Generate<TitleScreen>();
            
            ScreenTransitionerLocator.Instance.TopLayerTransitioner.Move<TitleScreen>(
                to,
                SampleScreenTransitionAnimation.Instance.CloseAnimation,
                SampleScreenTransitionAnimation.Instance.OpenAnimation).Forget();
        }
    }
}
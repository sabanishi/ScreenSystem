using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystemSample
{
    public class SceneInitializer : MonoBehaviour
    {
        private void Start()
        {
            ScreenTransitionLocator.Instance.Move<TitleScreen>(
                SampleScreenTransitionAnimation.Instance.CloseAnimation,
                SampleScreenTransitionAnimation.Instance.OpenAnimation).Forget();
        }
    }
}
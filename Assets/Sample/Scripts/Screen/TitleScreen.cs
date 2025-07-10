using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Screen = Sabanishi.ScreenSystem.Screen;

namespace Sabanishi.ScreenSystemSample
{
    public class TitleScreen : Screen
    {
        [SerializeField] private Button button;

        protected override UniTask InitializeInternal(CancellationToken token)
        {
            button.OnClickAsObservable().Subscribe(_ =>
            {
                //SendMessageScreenへ遷移
                var to = ScreenGenerator.Generate<SendMessageScreen>();
                ScreenTransitionerLocator.Instance.TopLayerTransitioner.Move<SendMessageScreen>(
                    to,
                    SampleScreenTransitionAnimation.Instance.CloseAnimation, 
                    SampleScreenTransitionAnimation.Instance.OpenAnimation).Forget();
            }).AddTo(gameObject);
            return UniTask.CompletedTask;
        }
    }
}
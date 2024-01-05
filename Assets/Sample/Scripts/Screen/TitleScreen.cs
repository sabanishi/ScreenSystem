using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanishi.ScreenSystem;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sabanishi.ScreenSystemSample
{
    public class TitleScreen : BaseScreen
    {
        [SerializeField] private Button button;

        protected override UniTask InitializeInternal(CancellationToken token)
        {
            button.OnClickAsObservable().Subscribe(_ =>
            {
                //SendMessageScreenへ遷移
                ScreenTransitionLocator.Instance.Move<SendMessageScreen>(
                    SampleScreenTransitionAnimation.Instance.CloseAnimation, 
                    SampleScreenTransitionAnimation.Instance.OpenAnimation).Forget();
            }).AddTo(gameObject);
            return UniTask.CompletedTask;
        }
    }
}
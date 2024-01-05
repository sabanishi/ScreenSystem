using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanishi.ScreenSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sabanishi.ScreenSystemSample
{
    public class SendMessageScreen : BaseScreen
    {
        [SerializeField] private Button titleScreenButton;
        [SerializeField] private Button receiveMessageScreenButton;
        [SerializeField] private TMP_InputField inputField;

        protected override UniTask InitializeInternal(CancellationToken token)
        {
            titleScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                ScreenTransitionLocator.Instance.Move<TitleScreen>(null, null).Forget();
            }).AddTo(gameObject);
            receiveMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                ScreenTransitionLocator.Instance.Move<ReceiveMessageScreen>(null, null, (nextScreen) =>
                {
                    nextScreen.SetMessage(inputField.text);
                }).Forget();
            }).AddTo(gameObject);

            return UniTask.CompletedTask;
        }
    }
}
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
                //TitleScreenへ遷移する
                ScreenTransitionLocator.Instance.Move<TitleScreen>(
                    SampleScreenTransitionAnimation.Instance.CloseAnimation,
                    SampleScreenTransitionAnimation.Instance.OpenAnimation).Forget();
            }).AddTo(gameObject);

            receiveMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                //ReceiveMessageScreenへ遷移する
                ScreenTransitionLocator.Instance.Move<ReceiveMessageScreen>(
                    SampleScreenTransitionAnimation.Instance.CloseAnimation,
                    SampleScreenTransitionAnimation.Instance.OpenAnimation,
                    //遷移先の画面にメッセージを渡すためのデリゲート
                    nextScreen => nextScreen.SetMessage(inputField.text)
                ).Forget();
            }).AddTo(gameObject);

            return UniTask.CompletedTask;
        }
    }
}
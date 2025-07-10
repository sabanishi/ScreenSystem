using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Screen = Sabanishi.ScreenSystem.Screen;

namespace Sabanishi.ScreenSystemSample
{
    public class SendMessageScreen : Screen
    {
        [SerializeField] private Button titleScreenButton;
        [SerializeField] private Button receiveMessageScreenButton;
        [SerializeField] private TMP_InputField inputField;

        protected override UniTask InitializeInternal(CancellationToken token)
        {
            titleScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                //TitleScreenへ遷移する
                var to = ScreenGenerator.Generate<TitleScreen>();
                ScreenTransitionerLocator.Instance.TopLayerTransitioner.Move<TitleScreen>(
                    to,
                    SampleScreenTransitionAnimation.Instance.CloseAnimation,
                    SampleScreenTransitionAnimation.Instance.OpenAnimation).Forget();
            }).AddTo(gameObject);

            receiveMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                //ReceiveMessageScreenへ遷移する
                var to = ScreenGenerator.Generate<ReceiveMessageScreen>();
                ScreenTransitionerLocator.Instance.TopLayerTransitioner.Move<ReceiveMessageScreen>(
                    to,
                    //遷移先の画面にメッセージを渡すためのデリゲート
                    nextScreen => nextScreen.SetMessage(inputField.text),
                    SampleScreenTransitionAnimation.Instance.CloseAnimation,
                    SampleScreenTransitionAnimation.Instance.OpenAnimation
                ).Forget();
            }).AddTo(gameObject);

            return UniTask.CompletedTask;
        }
    }
}
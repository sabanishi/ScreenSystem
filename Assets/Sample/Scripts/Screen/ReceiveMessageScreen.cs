using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Screen = Sabanishi.ScreenSystem.Screen;

namespace Sabanishi.ScreenSystemSample
{
    public class ReceiveMessageScreen : Screen
    {
        [SerializeField] private Button sendMessageScreenButton;
        [SerializeField] private TMP_Text messageText;

        protected override UniTask InitializeInternal(CancellationToken token)
        {
            sendMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
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
        
        /// <summary>
        /// メッセージを受け取る
        /// </summary>
        public void SetMessage(string message)
        {
            messageText.text = message;
        }
    }
}
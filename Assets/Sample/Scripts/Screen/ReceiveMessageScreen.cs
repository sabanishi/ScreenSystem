using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanishi.ScreenSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sabanishi.ScreenSystemSample
{
    public class ReceiveMessageScreen : BaseScreen
    {
        [SerializeField] private Button sendMessageScreenButton;
        [SerializeField] private TMP_Text messageText;

        protected override UniTask InitializeInternal(CancellationToken token)
        {
            sendMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                //SendMessageScreenへ遷移
                ScreenTransitionLocator.Instance.Move<SendMessageScreen>(
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
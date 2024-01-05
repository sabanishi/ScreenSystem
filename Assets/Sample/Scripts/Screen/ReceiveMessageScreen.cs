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

        protected override UniTask InitializeInternal(IScreenData data, CancellationToken token)
        {
            sendMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                ScreenTransitionLocator.Instance.Move<SendMessageScreen>(null, null).Forget();
            }).AddTo(gameObject);

            //受け取ったデータを表示
            if (data is SampleScreenData sampleScreenData)
            {
                messageText.text = sampleScreenData.Message;
            }

            return UniTask.CompletedTask;
        }
    }
}
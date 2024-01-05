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

        protected override UniTask InitializeInternal(IScreenData data, CancellationToken token)
        {
            titleScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                ScreenTransitionLocator.Instance.Move<TitleScreen>(null, null).Forget();
            }).AddTo(gameObject);
            receiveMessageScreenButton.OnClickAsObservable().Subscribe(_ =>
            {
                ScreenTransitionLocator.Instance.Move<ReceiveMessageScreen>(null, null).Forget();
            }).AddTo(gameObject);

            return UniTask.CompletedTask;
        }

        protected override UniTask<IScreenData> DisposeInternal(CancellationToken token)
        {
            //入力された文字列を次の画面に渡す
            var data = new SampleScreenData(inputField.text);
            return UniTask.FromResult<IScreenData>(data);
        }
    }
}
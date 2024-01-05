using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanishi.ScreenSystem;
using UnityEngine.UI;

namespace Sabanishi.ScreenSystemSample
{
    public class SampleCloseAnimation:ITransitionAnimation
    {
        private readonly Image _image;
        public SampleCloseAnimation(Image image)
        {
            _image = image;
        }
        
        public async UniTask Play(CancellationToken token)
        {
            _image.gameObject.SetActive(true);
            //0.5秒かけて暗転させる
            await _image.DOFade(1, 0.5f).ToUniTask(cancellationToken: token);
        }
    }
}
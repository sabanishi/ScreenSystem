using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanishi.ScreenSystem;
using UnityEngine.UI;

namespace Sabanishi.ScreenSystemSample
{
    public class SampleOpenAnimation:ITransitionAnimation
    {
        private readonly Image _image;
        public SampleOpenAnimation(Image image)
        {
            _image = image;
        }
        
        public async UniTask Play(CancellationToken token)
        {
            //0.5秒かけて暗転を解除する
            await _image.DOFade(0, 0.5f).ToUniTask(cancellationToken: token);
            _image.gameObject.SetActive(false);
        }
    }
}
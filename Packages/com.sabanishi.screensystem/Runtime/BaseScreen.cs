using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystem
{
    /// <summary>
    /// Screenの基底クラス
    /// </summary>
    public abstract class BaseScreen : MonoBehaviour,IScreen
    {
        /// <summary>
        /// Screen生成直後に呼ばれる関数
        /// </summary>
        public async UniTask Initialize(IScreenData data, CancellationToken token)
        {
            await InitializeInternal(data, token);
        }

        /// <summary>
        /// Screen破棄直前に呼ばれる関数
        /// </summary>
        public async UniTask<IScreenData> Dispose(CancellationToken token)
        {
            return await DisposeInternal(token);
        }

        /// <summary>
        /// Screenが閉じられる直前に呼ばれる関数
        /// </summary>
        public async UniTask Close(CancellationToken token)
        {
            await CloseInternal(token);
        }

        /// <summary>
        /// Screenが開かれた直後に呼ばれる関数
        /// </summary>
        public async UniTask Open(CancellationToken token)
        {
            await OpenInternal(token);
        }
        
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        
        protected virtual UniTask InitializeInternal(IScreenData data, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask<IScreenData> DisposeInternal(CancellationToken token)
        {
            return UniTask.FromResult<IScreenData>(null);
        }

        protected virtual UniTask CloseInternal(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OpenInternal(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}
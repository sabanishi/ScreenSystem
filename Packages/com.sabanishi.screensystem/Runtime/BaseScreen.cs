using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystem
{
    /// <summary>
    /// IScreenの具象クラス
    /// </summary>
    public abstract class Screen : MonoBehaviour, IScreen
    {
        private bool _isInitialized;
        private bool _isActive;
        private bool _isOpen;
        private bool _isDonePostBridgeFunc;

        protected bool IsInitialized => _isInitialized;
        protected bool IsActive => _isActive;
        protected bool IsOpen => _isOpen;

        private CancellationTokenSource _initializeTokenSource;
        private CancellationTokenSource _activeTokenSource;

        /// <summary>親Screen</summary>
        public Screen Parent { get; private set; }

        /// <summary>自身が登録されているScreenTransitioner</summary>
        public ScreenTransitioner ParentTransitioner { get; private set; }

        /// <summary>自身が管理している子ScreenTransitioner</summary>
        protected ScreenTransitioner ChildTransitioner { get; private set; }

        public async UniTask Initialize(ScreenTransitioner parentTransitioner)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            ParentTransitioner = parentTransitioner;
            var thisTransform = transform;
            ChildTransitioner = new ScreenTransitioner(this, thisTransform);
            Parent = parentTransitioner.Parent;
            _initializeTokenSource = new CancellationTokenSource();

            await InitializeInternal(GetInitializeToken());
        }

        public async UniTask Dispose()
        {
            if (!_isInitialized) return;
            _isInitialized = false;
            if (_isOpen)
            {
                await Close();
                _isOpen = false;
            }

            if (_isActive)
            {
                await Deactivate();
                _isActive = false;
            }

            await DisposeInternal();
            await ChildTransitioner.Dispose();
            _initializeTokenSource.Cancel();
            _initializeTokenSource.Dispose();
            _initializeTokenSource = null;
        }

        public async UniTask Activate()
        {
            if (_isActive) return;
            if (Parent != null && !Parent.IsActive) return;

            _isActive = true;
            _activeTokenSource = new CancellationTokenSource();
            await ActivateInternal(GetActiveToken());

            //自身が管理しているScreenをActivate
            await ChildTransitioner.Activate();
        }

        public async UniTask Deactivate()
        {
            if (!_isActive) return;

            _isActive = false;
            _isDonePostBridgeFunc = false;
            //自身が管理しているScreenをDeactivate
            await ChildTransitioner.Deactivate();

            await DeactivateInternal();

            _activeTokenSource.Cancel();
            _activeTokenSource.Dispose();
            _activeTokenSource = null;
        }

        public async UniTask PostBridgeFunc()
        {
            if (!_isActive) return;
            if (_isDonePostBridgeFunc) return;
            _isDonePostBridgeFunc = true;

            await PostBridgeFuncInternal(GetActiveToken());

            await ChildTransitioner.PostBridgeFunc();
        }

        public async UniTask Open()
        {
            if (_isOpen) return;
            _isOpen = true;

            await OpenInternal();

            await ChildTransitioner.Open();
        }

        public async UniTask Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            await ChildTransitioner.Close();

            await CloseInternal();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public virtual void DestroyGameObject()
        {
            Destroy(gameObject);
        }

        public CancellationToken GetInitializeToken()
        {
            var destroyToken = this.GetCancellationTokenOnDestroy();
            return CancellationTokenSource.CreateLinkedTokenSource(_initializeTokenSource.Token, destroyToken).Token;
        }

        public CancellationToken GetActiveToken()
        {
            var destroyToken = this.GetCancellationTokenOnDestroy();
            return CancellationTokenSource.CreateLinkedTokenSource(_activeTokenSource.Token, destroyToken).Token;
        }

        protected virtual UniTask InitializeInternal(CancellationToken initializeToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask PostBridgeFuncInternal(CancellationToken activeToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask DisposeInternal()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask ActivateInternal(CancellationToken activeToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask DeactivateInternal()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OpenInternal()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask CloseInternal()
        {
            return UniTask.CompletedTask;
        }
    }
}
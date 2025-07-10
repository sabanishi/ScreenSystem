using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystem
{
    /// <summary>
    /// 画面遷移機構を提供するクラス
    /// </summary>
    public class ScreenTransitioner
    {
        private CancellationTokenSource _activeTokenSource;

        ///<summary>現在管理しているScreen</summary>
        private readonly List<IScreen> _screenList;

        /// <summary>親ScreenのTransform</summary>
        private Transform _parentTransform;

        /// <summary>親Screen</summary>
        public Screen Parent { get; }

        private IScreen _currentScreen;
        private bool _isTransitioning;

        /// <summary>trueの時、Screen生成時に_parentTransformを使用しない</summary>
        private bool _isUseDefaultTransform;

        public ScreenTransitioner(Screen parent, Transform parentTransform)
        {
            Parent = parent;
            _parentTransform = parentTransform;
            _screenList = new List<IScreen>();
            _activeTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 現在積まれているScreenを全て破棄し、遷移中の場合は遷移を中断する
        /// </summary>
        public async UniTask Dispose()
        {
            //全てのScreenを破棄/破壊する
            await DestroyAllScreen();
            _activeTokenSource.Cancel();
            _activeTokenSource = new CancellationTokenSource();
            _currentScreen = null;
        }

        /// <summary>
        /// 自身が管理している全てのScreenをActivateする
        /// </summary>
        public async UniTask Activate()
        {
            if (_currentScreen != null)
            {
                await _currentScreen.Activate();
            }
        }

        /// <summary>
        /// 自身が管理している全てのScreenをDeactivateする
        /// </summary>
        public async UniTask Deactivate()
        {
            if (_currentScreen != null)
            {
                await _currentScreen.Deactivate();
            }
        }

        public async UniTask PostBridgeFunc()
        {
            if (_currentScreen != null)
            {
                await _currentScreen.PostBridgeFunc();
            }
        }
        
        public async UniTask Open()
        {
            if (_currentScreen != null)
            {
                await _currentScreen.Open();
            }
        }
        
        public async UniTask Close()
        {
            if (_currentScreen != null)
            {
                await _currentScreen.Close();
            }
        }

        public void SetIsUseDefaultTransform(bool isUseDefaultTransform)
        {
            _isUseDefaultTransform = isUseDefaultTransform;
        }

        /// <summary>
        /// 前のScreenを残さずに遷移する(Screen間でのデータ受け渡しなし、デフォルトのFadeアニメーション)
        /// </summary>
        public async UniTask Jump<T>(IScreen to) where T : IScreen
        {
            await Jump<T>(to, null, null, null);
        }

        /// <summary>
        /// 前のScreenを残さずに遷移する(Screen間でのデータ受け渡しなし)
        /// </summary>
        public async UniTask Jump<T>(IScreen to,
            ITransitionAnimation closeAnimation,
            ITransitionAnimation openAnimation) where T : IScreen
        {
            await Jump<T>(to, null, closeAnimation, openAnimation);
        }

        /// <summary>
        /// 前のScreenを残さずに遷移する(デフォルトのFadeアニメーション)
        /// </summary>
        public async UniTask Jump<T>(IScreen to, Action<T> bridgeAction) where T : IScreen
        {
            await Jump<T>(to, bridgeAction, null, null);
        }

        /// <summary>
        /// 前のScreenを残さずに遷移する
        /// </summary>
        public async UniTask Jump<T>(IScreen to, Action<T> bridgeAction,
            ITransitionAnimation closeAnimation,
            ITransitionAnimation openAnimation) where T : IScreen
        {
            if (_isTransitioning)
            {
                Debug.LogError("[ScreenTransitioner]遷移中なので遷移できません");
                to.DestroyGameObject();
                return;
            }

            if (to == _currentScreen) return;

            _isTransitioning = true;
            to.GetGameObject().SetActive(false);

            //現在のScreenを閉じて破棄する
            var from = _currentScreen;
            if (from != null)
            {
                await CloseAndDisposeScreen(from, closeAnimation);
            }
            else if (closeAnimation != null)
            {
                await PlayAnimation(closeAnimation, _activeTokenSource.Token);
            }

            //全てのScreenを破棄/破壊する
            await DestroyAllScreen();

            //次のScreenを生成
            await InitializeAndOpenScreen<T>(to, bridgeAction, openAnimation);

            //前のScreenを破壊する
            if (from != null)
            {
                from.DestroyGameObject();
            }

            _currentScreen = to;
            _isTransitioning = false;
        }

        /// <summary>
        /// 前のScreenを残して遷移する(Screen間でのデータ受け渡しなし、デフォルトのFadeアニメーション)
        /// </summary>
        public async UniTask Move<T>(IScreen to) where T : IScreen
        {
            await Move<T>(to, null, null, null);
        }

        /// <summary>
        /// 前のScreenを残して遷移する(Screen間でのデータ受け渡しなし)
        /// </summary>
        public async UniTask Move<T>(IScreen to,
            ITransitionAnimation closeAnimation,
            ITransitionAnimation openAnimation) where T : IScreen
        {
            await Move<T>(to, null, closeAnimation, openAnimation);
        }

        /// <summary>
        /// 前のScreenを残して遷移する(デフォルトのFadeアニメーション)
        /// </summary>
        public async UniTask Move<T>(IScreen to, Action<T> bridgeAction) where T : IScreen
        {
            await Move<T>(to, bridgeAction, null, null);
        }

        /// <summary>
        /// 前のScreenを残して遷移する
        /// </summary>
        public async UniTask Move<T>(IScreen to, Action<T> bridgeAction,
            ITransitionAnimation closeAnimation,
            ITransitionAnimation openAnimation) where T : IScreen
        {
            if (_isTransitioning)
            {
                Debug.LogError("[ScreenTransitioner]遷移中なので遷移できません");
                to.DestroyGameObject();
                return;
            }

            if (to == _currentScreen) return;

            _isTransitioning = true;
            to.GetGameObject().SetActive(false);

            //現在のScreenを閉じる
            var from = _currentScreen;
            if (from != null)
            {
                await CloseScreen(from, closeAnimation);
            }
            else if (closeAnimation != null)
            {
                await PlayAnimation(closeAnimation, _activeTokenSource.Token);
            }

            //次のScreenを生成
            await InitializeAndOpenScreen<T>(to, bridgeAction, openAnimation);
            _currentScreen = to;

            _isTransitioning = false;
        }

        /// <summary>
        /// 直前のScreenに遷移する(Screen間でのデータ受け渡しなし、デフォルトのFadeアニメーション)
        /// </summary>
        public async UniTask Back()
        {
            await Back(bridgeAction: null, closeAnimation: null, openAnimation: null);
        }

        /// <summary>
        /// 直前のScreenに遷移する(デフォルトのFadeアニメーション)
        /// </summary>
        public async UniTask Back(Action<IScreen> bridgeAction)
        {
            await Back(bridgeAction: bridgeAction, closeAnimation: null, openAnimation: null);
        }

        /// <summary>
        /// 直前のScreenに遷移する(Screen間でのデータ受け渡しなし)
        /// </summary>
        public async UniTask Back(ITransitionAnimation closeAnimation, ITransitionAnimation openAnimation)
        {
            await Back(bridgeAction: null, closeAnimation: closeAnimation, openAnimation: openAnimation);
        }

        /// <summary>
        /// 直前のScreenに遷移する
        /// </summary>
        public async UniTask Back(Action<IScreen> bridgeAction,
            ITransitionAnimation closeAnimation,
            ITransitionAnimation openAnimation)
        {
            if (_isTransitioning)
            {
                Debug.LogError("[ScreenTransitioner]遷移中なので遷移できません");
                return;
            }

            _isTransitioning = true;

            var from = _currentScreen;
            if (from != null)
            {
                await CloseAndDisposeScreen(from, closeAnimation);
                //前のScreenを破壊
                from.DestroyGameObject();
            }
            else if (closeAnimation != null)
            {
                await PlayAnimation(closeAnimation, _activeTokenSource.Token);
            }

            //直前に生成されていたScreenが存在すれば開く
            if (_screenList.Count > 0)
            {
                var to = _screenList[^1];
                await OpenScreen<IScreen>(to, bridgeAction, openAnimation);
                _currentScreen = to;
            }
            else
            {
                await PlayAnimation(openAnimation, _activeTokenSource.Token);
                _currentScreen = null;
            }

            _isTransitioning = false;
        }

        /// <summary>
        /// screenListに載っている全てのScreenを逆順で破棄/破壊する
        /// </summary>
        private async UniTask DestroyAllScreen()
        {
            for (var i = _screenList.Count - 1; i >= 0; i--)
            {
                if (_activeTokenSource.Token.IsCancellationRequested) break;
                var screen = _screenList[i];
                await screen.Dispose();
                screen.DestroyGameObject();
            }

            _screenList.Clear();
        }

        /// <summary>
        /// 引数のScreenを閉じて破棄し、_screenListからRemoveする(破壊は行わない)
        /// </summary>
        private async UniTask CloseAndDisposeScreen(IScreen screen,
            ITransitionAnimation closeAnimation)
        {
            await CloseScreen(screen, closeAnimation);
            await screen.Dispose();
            _screenList.Remove(screen);
        }

        /// <summary>
        /// 引数のScreenを閉じる
        /// </summary>
        private async UniTask CloseScreen(IScreen screen, ITransitionAnimation closeAnimation)
        {
            await screen.Close();
            await PlayAnimation(closeAnimation, _activeTokenSource.Token);
            await screen.Deactivate();
            screen.GetGameObject().SetActive(false);
        }

        /// <summary>
        /// 引数のScreenを初期化して開く
        /// </summary>
        private async UniTask InitializeAndOpenScreen<T>(IScreen to, Action<T> bridgeAction,
            ITransitionAnimation openAnimation) where T : IScreen
        {
            var gameObject = to.GetGameObject();
            if (!_isUseDefaultTransform)
            {
                gameObject.transform.SetParent(_parentTransform, false);
            }

            await to.Initialize(this);
            _screenList.Add(to);
            await OpenScreen(to, bridgeAction, openAnimation);
        }

        /// <summary>
        /// 引数のScreenを開く
        /// </summary>
        private async UniTask OpenScreen<T>(IScreen screen, Action<T> bridgeAction, ITransitionAnimation openAnimation)
            where T : IScreen
        {
            screen.GetGameObject().SetActive(true);
            await screen.Activate();
            bridgeAction?.Invoke((T)screen);
            await screen.PostBridgeFunc();
            await PlayAnimation(openAnimation, _activeTokenSource.Token);
            await screen.Open();
        }

        /// <summary>
        /// nullチェックしつつITransitionAnimationを再生する
        /// </summary>
        private async UniTask PlayAnimation(ITransitionAnimation animation, CancellationToken token)
        {
            if (animation != null)
            {
                await animation.Play(token);
            }
        }
    }
}
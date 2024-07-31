using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystem
{
    /// <summary>
    /// 画面遷移を管理するクラス
    /// </summary>
    public class ScreenTransition
    {
        private Dictionary<Type, GameObject> _screenPrefabDict;
        private bool _isTransitioning;

        private CancellationTokenSource _cts;
        private IScreen _currentScreen;
        public IScreen CurrentScreen => _currentScreen;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="screenPrefabDict">Screen型とそのプレハブを対応づけるDictionary</param>
        public void Initialize(Dictionary<Type, GameObject> screenPrefabDict)
        {
            SetScreenPrefabDict(screenPrefabDict);
            _isTransitioning = false;
        }

        public void SetScreenPrefabDict(Dictionary<Type, GameObject> screenPrefabDict)
        {
            _screenPrefabDict = screenPrefabDict;
        }

        public async UniTask Move<T>(
            ITransitionAnimation closeAnimation,
            ITransitionAnimation openAnimation,
            Action<T> bridgeAction = null)
            where T : IScreen
        {
            // 画面遷移中は何もしない
            if (_isTransitioning) return;
            _isTransitioning = true;

            if (_screenPrefabDict == null)
            {
                _isTransitioning = false;
                throw new NullReferenceException("ScreenTransition is not setup");
            }

            if (!_screenPrefabDict.TryGetValue(typeof(T), out var screenPrefab))
            {
                _isTransitioning = false;
                throw new KeyNotFoundException($"ScreenPrefab of {typeof(T)} is not found");
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            // 現在の画面がある場合は破棄する
            if (_currentScreen != null)
            {
                var currentScreenObject = _currentScreen.GetGameObject();
                var currentScreenCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token,
                    currentScreenObject.GetCancellationTokenOnDestroy());
                await _currentScreen.Close(currentScreenCts.Token);
                //画面を閉じるアニメーションを再生する
                if (closeAnimation != null)
                {
                    await closeAnimation.Play(currentScreenCts.Token);
                }

                await _currentScreen.Dispose(currentScreenCts.Token);
                GameObject.Destroy(currentScreenObject);
            }

            //次の画面を生成する
            var nextScreenObject = GameObject.Instantiate(screenPrefab);
            var nextScreen = nextScreenObject.GetComponent<T>();
            if (nextScreen == null)
            {
                _isTransitioning = false;
                throw new NullReferenceException($"ScreenPrefab of {typeof(T)} does not have {typeof(T)} component");
            }

            // 次の画面のセットアップ
            var nextScreenCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token,
                nextScreenObject.GetCancellationTokenOnDestroy());

            await nextScreen.Initialize(nextScreenCts.Token);
            //前の画面から渡されたデータを伝える
            bridgeAction?.Invoke(nextScreen);

            //画面を開くアニメーションを再生する
            if (openAnimation != null)
            {
                await openAnimation.Play(nextScreenCts.Token);
            }

            await nextScreen.Open(nextScreenCts.Token);

            _currentScreen = nextScreen;
            _isTransitioning = false;
        }
    }
}

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
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="screenPrefabDict">Screen型とそのプレハブを対応づけるDictionary</param>
        public void Initialize(Dictionary<Type,GameObject> screenPrefabDict)
        {
            _screenPrefabDict = screenPrefabDict;
            _isTransitioning = false;
        }
        
        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (_currentScreen != null)
            {
                UniTask.Void(async () =>
                {
                    await _currentScreen.Close(CancellationToken.None);
                    await _currentScreen.Dispose(CancellationToken.None);
                    GameObject.Destroy(_currentScreen.GetGameObject());
                    _isTransitioning = false;
                });
            }
        }
        
        public async UniTask Move<T>(ITransitionAnimation closeAnimation,ITransitionAnimation openAnimation) where T : IScreen
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
            
            IScreenData screenData = null;
            
            // 現在の画面がある場合は破棄する
            if (_currentScreen != null)
            {
                var currentScreenObject = _currentScreen.GetGameObject();
                var currentScreenCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token,
                    currentScreenObject.GetCancellationTokenOnDestroy());
                await _currentScreen.Close(currentScreenCts.Token);
                if (closeAnimation != null)
                {
                    await closeAnimation.Play(currentScreenCts.Token);
                }
                screenData = await _currentScreen.Dispose(currentScreenCts.Token);
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
            await nextScreen.Initialize(screenData,nextScreenCts.Token);
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
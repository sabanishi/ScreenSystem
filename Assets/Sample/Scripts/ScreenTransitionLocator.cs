using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sabanishi.ScreenSystem;
using UnityEngine;

namespace Sabanishi.ScreenSystemSample
{
    /// <summary>
    /// ScreenTransitionをシングルトンとして公開するためのクラス
    /// </summary>
    public class ScreenTransitionLocator:MonoBehaviour
    {
        /**ScreenTypeとResourcesパスを対応させるDict*/
        private readonly Dictionary<Type, string> _screenPathDictionary = new()
        {
            {typeof(TitleScreen),"TitleScreen"},
            {typeof(SelectScreen),"SelectScreen"},
            {typeof(SampleScreenA),"SampleScreenA"},
            {typeof(SampleScreenB),"SampleScreenB"},
        };
        
        private static ScreenTransitionLocator _instance;
        public static ScreenTransitionLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("ScreenTransitionLocator is not setup.");
                }
                return _instance;
            }
        }

        private ScreenTransition _screenTransition;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _screenTransition = new ScreenTransition();
            _screenTransition.Initialize(LoadScreenPrefab());
        }

        private void OnDestroy()
        {
            _screenTransition.Dispose();
        }

        /// <summary>
        /// ScreenPathDictionaryを元にResourcesフォルダからScreenプレハブを取得する
        /// </summary>
        private Dictionary<Type, GameObject> LoadScreenPrefab()
        {
            var dict = new Dictionary<Type, GameObject>();
            foreach (var pair in _screenPathDictionary)
            {
                var prefab = Resources.Load<GameObject>(pair.Value);
                if (prefab == null)
                {
                    Debug.LogError($"{pair.Value} is not exits in Resources Folder");
                }
                dict.Add(pair.Key,prefab);
            }

            return dict;
        }

        public async UniTask Move<T>(ITransitionAnimation closeAnimation, ITransitionAnimation openAnimation) where T : IScreen
        {
            await _screenTransition.Move<T>(closeAnimation,openAnimation);
        }
    }
}
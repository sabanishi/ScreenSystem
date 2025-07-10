using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystem
{
    /// <summary>
    ///画面を定義するインターフェース<br/>
    ///
    /// Initialize<br/>
    /// --Activate<br/>
    /// ----Open<br/>
    /// ----Close<br/>
    /// --Deactivate<br/>
    /// Dispose<br/>
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// 初期化関数
        /// </summary>
        public UniTask Initialize(ScreenTransitioner parentTransitioner);
        
        /// <summary>
        /// 破棄関数
        /// </summary>
        public UniTask Dispose();
        
        /// <summary>
        /// Screenを開く直前に呼ばれる関数
        /// </summary>
        public UniTask Activate();

        /// <summary>
        /// bridge関数の後に呼ばれる関数(Openよりは前に実行される)
        /// </summary>
        public UniTask PostBridgeFunc();
        
        /// <summary>
        /// Screenを閉じた直後に呼ばれる関数
        /// </summary>
        /// 
        public UniTask Deactivate();
        
        /// <summary>
        /// Screenを開いた直後に呼ばれる関数
        /// </summary>
        public UniTask Open();
        /// <summary>
        /// Screenを閉じる直前に呼ばれる関数
        /// </summary>
        public UniTask Close();
        
        public GameObject GetGameObject();
        
        /// <summary>
        /// 自分が管理しているGameObjectを破棄する関数
        /// </summary>
        public void DestroyGameObject();
    }
}
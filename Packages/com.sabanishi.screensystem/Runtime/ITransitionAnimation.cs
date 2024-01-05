using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sabanishi.ScreenSystem
{
    /// <summary>
    /// Screen間の遷移アニメーション用インターフェース
    /// </summary>
    public interface ITransitionAnimation
    {
        public UniTask Play(CancellationToken token);
    }
}
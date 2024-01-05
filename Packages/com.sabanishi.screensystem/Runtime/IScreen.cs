using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sabanishi.ScreenSystem
{
    public interface IScreen
    {
        public UniTask Initialize(CancellationToken token);
        public UniTask Dispose(CancellationToken token);
        public UniTask Close(CancellationToken token);
        public UniTask Open(CancellationToken token);

        public GameObject GetGameObject();
    }
}
using Cysharp.Threading.Tasks;

namespace Sabanishi.ScreenSystem
{
    public class Hoge
    {
        public void Fga()
        {
            UniTask.Void(async () =>
            {
                await UniTask.Delay(1000);
            });
        }
    }
}
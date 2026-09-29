using Cysharp.Threading.Tasks;
using System.Threading;

namespace WhiteKNight
{
    public interface IOpenSceneTransitionPresenter
    {
        UniTask OnSceneStartAsync(CancellationToken token);
    }
}
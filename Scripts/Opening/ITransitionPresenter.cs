using Cysharp.Threading.Tasks;
using System.Threading;

namespace WhiteKNight
{
    public interface ITransitionPresenter
    {
        UniTask OnStartButtonClickedAsync(CancellationToken token);
    }
}
using Cysharp.Threading.Tasks;
using System.Threading;

namespace WhiteKNight
{
    public interface IStartGameTransitionPresenter
    {
        UniTask<StartGameResult> OnStartButtonClickedAsync(CancellationToken token);
    }
}
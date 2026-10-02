using Cysharp.Threading.Tasks;
using System.Threading;
using static WhiteKNight.LoadSceneUseCase;

namespace WhiteKNight
{
    public interface ITransitionView
    {
        SceneId SceneId { get; }

        UniTask PlayFadeOutAsync(float duration, CancellationToken token);

    }
}
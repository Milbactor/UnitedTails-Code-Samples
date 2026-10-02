using Cysharp.Threading.Tasks;
using System.Threading;
using static WhiteKNight.LoadSceneUseCase;

namespace WhiteKNight
{
    public interface ILoadSceneUseCase
    {
        UniTask LoadSceneAsync(SceneId sceneId, CancellationToken token);
    }
}
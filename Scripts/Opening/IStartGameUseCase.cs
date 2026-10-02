using System.Threading;
using static WhiteKNight.LoadSceneUseCase;

namespace WhiteKNight
{
    public interface IStartGameUseCase
    {
        Cysharp.Threading.Tasks.UniTask ExecuteAsync(CancellationToken ct, SceneId sceneId);

        bool IsCharactersReady();
    }
}
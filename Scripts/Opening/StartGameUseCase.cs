using Cysharp.Threading.Tasks;
using System.Threading;
using static WhiteKNight.LoadSceneUseCase;

namespace WhiteKNight
{
    public class StartGameUseCase : IStartGameUseCase
    {
        private readonly ICharacterSelectionModel model;
        private readonly ILoadSceneUseCase loadScene;

        public bool IsCharactersReady()
        {
            return !(model.SelectedPlayCharacterId == null || model.SelectedNpcCharacterId == null);
        }

        public StartGameUseCase(ICharacterSelectionModel model, ILoadSceneUseCase loadScene)
        {
            this.model = model;
            this.loadScene = loadScene;
        }

        public async UniTask ExecuteAsync(CancellationToken ct, SceneId sceneId)
        {
            await loadScene.LoadSceneAsync(sceneId, ct);
        }
    }
}


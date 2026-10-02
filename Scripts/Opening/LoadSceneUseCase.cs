using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

namespace WhiteKNight
{
    public class LoadSceneUseCase : ILoadSceneUseCase
    {
        public enum SceneId
        {
            Opening,
            CharacterSelection,
            darkterrain5_beauty,
            darkterrain5
        }

        public async UniTask LoadSceneAsync(SceneId sceneId, CancellationToken token)
        {
            //await SceneManager.LoadSceneAsync(sceneId.ToString());
            var handle = SceneManager.LoadSceneAsync(sceneId.ToString(), LoadSceneMode.Single);
           // await SceneManager.LoadSceneAsync(sceneId.ToString(), LoadSceneMode.Single)
           //       .ToUniTask(cancellationToken: token);

            while (!handle.isDone)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}


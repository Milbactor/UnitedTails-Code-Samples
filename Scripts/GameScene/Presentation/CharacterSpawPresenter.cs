using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class CharacterSpawnPresenter :
        ICharacterSpawnPresenter,
        IDisposable
    {
        [Inject]
        private ICharacterSpawnUseCase useCase;

        [Inject]
        private CharacterSpawnView spawnView;

        private CameraView cameraView;

        private AIResetPositionPresenter _aiResetPositionPresenter;

        public void SetCameraView(CameraView cameraView)
        {
            this.cameraView = cameraView;
        }

        public async UniTask Spawn()
        {
            try
            {
                var playerModel =
                    useCase.GetPlayCharacterModel();

                Transform playerTransform =
                    await spawnView.SpawnPlayer(
                        playerModel.PrefabReference);

                cameraView.SetFollowTarget(
                    playerTransform);

                var npcModel =
                    useCase.GetNpcCharacterModel();

                Transform npcTransform =
                    await spawnView.SpawnNpc(
                        npcModel.PrefabReference,
                        playerTransform);

                SetupAIResetPositionPresenter(
                    npcTransform);

                EnemyManager.Instance.Initialize(
                    npcTransform
                        .GetComponents<IEnemyDeathListener>()
                        .ToList());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void SetupAIResetPositionPresenter(Transform npcTransform)
        {
            _aiResetPositionPresenter?.Dispose();

            AIResetPositionView resetPositionView =
                npcTransform.GetComponent<
                    AIResetPositionView>();

            if (resetPositionView == null)
            {
                Debug.LogWarning(
                    $"{npcTransform.name} does not have " +
                    $"{nameof(AIResetPositionView)}.");

                return;
            }

            _aiResetPositionPresenter =
                new AIResetPositionPresenter(
                    resetPositionView,
                    spawnView);
        }

        public void Dispose()
        {
            _aiResetPositionPresenter?.Dispose();
            _aiResetPositionPresenter = null;
        }
    }
}
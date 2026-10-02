using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class GameMain : MonoBehaviour
    {
        [Inject] private ICharacterSpawnPresenter spawnPresenter;
        [Inject] private ICameraPresenter cameraPresenter;

        [SerializeField] private CameraSwitchInputController controller;
        [SerializeField] private CameraView cameraView;

        [SerializeField] private BGMId bGMId;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);

            SoundManager.Instance?.PlayBGM(bGMId);

            spawnPresenter.SetCameraView(cameraView);

            controller.Initialize(cameraPresenter);
            cameraPresenter.Initialize();

            _ = RunAsync();
        }

        private async UniTaskVoid RunAsync()
        {
            try
            {
                await spawnPresenter.Spawn();
            }
            catch (Exception e)
            {
                Debug.LogError($"Spawn error: {e.Message}");
            }
        }
    }
}
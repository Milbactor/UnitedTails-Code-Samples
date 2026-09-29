using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WhiteKNight
{
    public class OpenSceneTransitionView : MonoBehaviour, IOpenSceneTransitionView
    {
        [SerializeField] private GameObject ruleCanvas;
        [SerializeField] private Image fadeImage;
        [SerializeField] private Material fadeMaterial;

        private readonly Subject<Unit> _onSceneStart = new();
        public IObservable<Unit> OnSceneStart => _onSceneStart;

        void Awake()
        {
            ruleCanvas.SetActive(true);
            fadeMaterial = new Material(fadeImage.material);
            fadeImage.material = fadeMaterial;
        }

        private void Start()
        {
            _onSceneStart.OnNext(Unit.Default);
        }

        public async UniTask PlayFadeInAsync(float duration, CancellationToken token)
        {
            float time = 0f;
            while (time < duration)
            {
                float t = time / duration;
                fadeMaterial.SetFloat("_Alpha", 1f - t);
                await UniTask.Yield(PlayerLoopTiming.Update);
                time += Time.deltaTime;
            }
            fadeMaterial.SetFloat("_Alpha", 0f);
            ruleCanvas.SetActive(false);
        }
    }
}
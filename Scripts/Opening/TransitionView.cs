using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using static WhiteKNight.LoadSceneUseCase;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace WhiteKNight
{
    public class TransitionView : MonoBehaviour, ITransitionView
    {
        [SerializeField] private GameObject ruleCanvas;
        [SerializeField] private Button button; 
        [SerializeField] private Image fadeImage;
        [SerializeField] private Material fadeMaterial;
        [SerializeField] private SceneId sceneId;

        public IObservable<Unit> OnClickAsObservable => _onClickSubject;

        public SceneId SceneId { get => sceneId; }

        private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();

        void Awake()
        {
            fadeMaterial = new Material(fadeImage.material);  // 複製
            fadeImage.material = fadeMaterial;                // 差し替え
            button.OnClickAsObservable()
                .Subscribe(_ => _onClickSubject.OnNext(Unit.Default))
                .AddTo(this);
        }

        public async UniTask PlayFadeOutAsync(float duration, CancellationToken token)
        {
            ruleCanvas.SetActive(true);
            float time = 0f;
            while (time < duration)
            {
                // === 安全確認 ===
                if (fadeMaterial == null || this == null || gameObject == null)
                    return;

                float t = time / duration;
                fadeMaterial.SetFloat("_Alpha", t);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                time += Time.deltaTime;
            }

            fadeMaterial.SetFloat("_Alpha", 1f);
        }
    }
}


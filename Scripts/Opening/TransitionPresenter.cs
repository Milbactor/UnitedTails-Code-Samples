using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

namespace WhiteKNight
{
    public class TransitionPresenter : ITransitionPresenter
    {
        private readonly ITransitionView transitionView;
        private readonly ILoadSceneUseCase useCase;

        private CompositeDisposable disposables = new CompositeDisposable();

        private readonly CancellationTokenSource cts = new();

        public TransitionPresenter(TransitionView transitionView, ILoadSceneUseCase useCase)
        {
            this.transitionView = transitionView;
            this.useCase = useCase;
            transitionView.OnClickAsObservable
             .Subscribe(_ =>
             {
                 OnStartButtonClickedAsync(cts.Token).Forget();
             }).AddTo(disposables);
        }

        public async UniTask OnStartButtonClickedAsync(CancellationToken token)
        {
            await transitionView.PlayFadeOutAsync(1.0f, token);
            await useCase.LoadSceneAsync(transitionView.SceneId, token);
        }
    }


}


using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

namespace WhiteKNight
{
    public class StartGameTransitionPresenter : IStartGameTransitionPresenter
    {
        private readonly ITransitionView transitionView;
        private readonly IStartGameUseCase useCase;

        private CompositeDisposable disposables = new CompositeDisposable();

        private readonly CancellationTokenSource cts = new();

        public StartGameTransitionPresenter(TransitionView transitionView, IStartGameUseCase useCase)
        {            
            this.transitionView = transitionView;
            this.useCase = useCase;
            transitionView.OnClickAsObservable
             .Subscribe(_ => {
                 OnStartButtonClickedAsync(cts.Token).Forget();
             }).AddTo(disposables);
        }

        public async UniTask<StartGameResult> OnStartButtonClickedAsync(CancellationToken token)
        {
            if (useCase.IsCharactersReady() == false) return StartGameResult.NotReady;

            await transitionView.PlayFadeOutAsync(1.0f, token);
            await useCase.ExecuteAsync(token, transitionView.SceneId);

            return StartGameResult.Success;
        }
    }


}


using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

namespace WhiteKNight
{
    public class OpenSceneTransitionPresenter : IOpenSceneTransitionPresenter
    {
        private readonly IOpenSceneTransitionView transitionView;

        private CompositeDisposable disposables = new CompositeDisposable();

        private readonly CancellationTokenSource cts = new();

        public OpenSceneTransitionPresenter(OpenSceneTransitionView transitionView)
        {
            this.transitionView = transitionView;
            transitionView.OnSceneStart
             .Subscribe(_ =>
             {
                 OnSceneStartAsync(cts.Token).Forget();
             }).AddTo(disposables);
        }

        public async UniTask OnSceneStartAsync(CancellationToken token)
        {
            await transitionView.PlayFadeInAsync(1.0f, token);
        }

    }
}


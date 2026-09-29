using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using WhiteKNight;

namespace WhiteKnight
{
    public class FadeUseCase
    {
        public int CurrentIndex => _currentIndex;
        private int _currentIndex = 0;
        private readonly List<FadePresenter> _presenters = new List<FadePresenter>();

        private const float IDLE_TIME = 60f;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public FadeUseCase(List<FadePresenter> presenters)
        {
            if (presenters == null)
                throw new ArgumentNullException(nameof(presenters));

            if (presenters.Count < 2)
                throw new ArgumentException(
                    "FadeUseCase requires at least 2 presenters.",
                    nameof(presenters)
                );

            _presenters = presenters;
        }


        public async Task Begin()
        {
            var token = _cts.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(IDLE_TIME), token);
                    int nextIndex = (_currentIndex + 1) % _presenters.Count;
                    await CrossFade(_currentIndex, nextIndex);
                    _currentIndex = nextIndex;
                }
            }
            catch (TaskCanceledException)
            {
                // its normal, ignore
            }
        }

        public void Finish()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }

        public IObservable<Unit> CrossFade(int fromIndex, int toIndex)
        {
            return Observable.WhenAll(
                _presenters[fromIndex].FadeOut(),
                _presenters[toIndex].FadeIn()
            );
        }
    }
}

using System;
using System.Collections.Generic;
using UniRx;

namespace WhiteKNight
{
    public sealed class BoundaryPresenter : IDisposable
    {
        private readonly BoundaryDamageView _boundaryDamageView;
        private readonly BoundaryEffectView _boundaryEffectView;


        private readonly CompositeDisposable _disposables = new();

        private readonly Dictionary<IThirdPersonCharacterDamageView, int>
            _enteredAreaCounts = new();

        public BoundaryPresenter(
            IReadOnlyList<BoundaryAreaView> boundaryAreaViews,
            BoundaryDamageView boundaryDamageView,
            BoundaryEffectView boundaryEffectView)
        {
            _boundaryDamageView = boundaryDamageView;
            _boundaryEffectView = boundaryEffectView;

            foreach (BoundaryAreaView areaView in boundaryAreaViews)
            {
                areaView.OnBoundaryEntered
                    .Subscribe(HandleBoundaryEntered)
                    .AddTo(_disposables);

                areaView.OnBoundaryExited
                    .Subscribe(HandleBoundaryExited)
                    .AddTo(_disposables);
            }
        }

        private void HandleBoundaryEntered(IThirdPersonCharacterDamageView target)
        {
            if (_enteredAreaCounts.TryGetValue(
                    target,
                    out int areaCount))
            {
                _enteredAreaCounts[target] = areaCount + 1;
                return;
            }

            _enteredAreaCounts.Add(target, 1);
            _boundaryEffectView.StartSpore();
            _boundaryDamageView.StartDamage(target);
        }

        private void HandleBoundaryExited(ThirdPersonCharacterDamageView target)
        {
            if (!_enteredAreaCounts.TryGetValue(
                    target,
                    out int areaCount))
            {
                return;
            }

            areaCount--;

            if (areaCount > 0)
            {
                _enteredAreaCounts[target] = areaCount;
                return;
            }
            _boundaryEffectView?.StopSpore();
            _enteredAreaCounts.Remove(target);
            _boundaryDamageView.StopDamage(target);
        }

        public void Dispose()
        {
            _boundaryDamageView.Dispose();
            _disposables.Dispose();
        }
    }
}
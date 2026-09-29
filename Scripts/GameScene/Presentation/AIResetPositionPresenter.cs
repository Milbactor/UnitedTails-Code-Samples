using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public sealed class AIResetPositionPresenter : IDisposable
    {
        private readonly AIResetPositionView
            _resetPositionView;

        private readonly CharacterSpawnView
            _characterSpawnView;

        private readonly CompositeDisposable
            _disposables = new();

        private bool _isResetting;

        public AIResetPositionPresenter(
            AIResetPositionView resetPositionView,
            CharacterSpawnView characterSpawnView)
        {
            _resetPositionView = resetPositionView;
            _characterSpawnView = characterSpawnView;

            Observable.EveryUpdate()
                .Where(_ =>
                    !_isResetting &&
                    _resetPositionView
                        .IsBelowFallResetHeight)
                .Subscribe(_ => ResetToSafePosition())
                .AddTo(_disposables);
        }

        private void ResetToSafePosition()
        {
            if (_isResetting)
                return;

            _isResetting = true;

            try
            {
                Transform npcSpawnPoint =
                    _characterSpawnView.NpcSpawnPoint;

                if (npcSpawnPoint == null)
                    return;

                if (!_resetPositionView
                        .TryGetResetPosition(
                            npcSpawnPoint.position,
                            out Vector3 resetPosition))
                {
                    return;
                }

                _resetPositionView.ResetPosition(
                    resetPosition);
            }
            finally
            {
                _isResetting = false;
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
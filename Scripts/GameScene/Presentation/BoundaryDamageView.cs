using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class BoundaryDamageView : MonoBehaviour
    {
        private const float DamageIntervalSeconds = 1f;
        private const float DamagePerTick = 5f;

        private readonly SerialDisposable _damageTimer = new();

        private IThirdPersonCharacterDamageView _currentTarget;

        public void StartDamage(IThirdPersonCharacterDamageView target)
        {
            if (target == null)
                return;

            if (_currentTarget == target &&
                _damageTimer.Disposable != null)
            {
                return;
            }

            StopDamage();

            _currentTarget = target;

            _damageTimer.Disposable =
                Observable.Timer(
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(DamageIntervalSeconds))
                    .Subscribe(_ =>
                    {
                        if (_currentTarget == null)
                            return;

                        _currentTarget.TakeDamage(DamagePerTick);
                    });
        }

        public void StopDamage(IThirdPersonCharacterDamageView target)
        {
            if (_currentTarget != target)
                return;

            StopDamage();
        }

        private void StopDamage()
        {
            _damageTimer.Disposable = null;
            _currentTarget = null;
        }

        public void Dispose()
        {
            StopDamage();
            _damageTimer.Dispose();
        }
    }
}
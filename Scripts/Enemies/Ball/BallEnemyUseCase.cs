using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public sealed class BallEnemyUseCase : IDisposable
    {
        private readonly IBallEnemyModel _model;

        public bool IsDead => _model.IsDead;
        public ReactiveProperty<bool> IsAttacking => _model.IsAttacking;
        public ReactiveProperty<bool> IsDamaged => _model.IsDamaged;
        public ReactiveProperty<bool> IsJumping => _model.IsJumping;
        public ReactiveProperty<bool> IsIdle => _model.IsIdle;

        private readonly ReactiveProperty<float> _idleTimer = new();

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public BallEnemyUseCase(IBallEnemyModel model)
        {
            _model = model;

            IsIdle
                .DistinctUntilChanged()
                .Where(isIdle => isIdle)
                .Subscribe(_ =>
                {
                    _idleTimer.Value =
                        UnityEngine.Random.Range(0.1f, 0.5f);
                })
                .AddTo(_disposables);

            _idleTimer
                .Pairwise()
                .Where(timer =>
                    timer.Previous > 0f &&
                    timer.Current <= 0f)
                .Where(_ => IsIdle.Value)
                .Subscribe(_ =>
                {
                    OnIdleFinished();
                })
                .AddTo(_disposables);
        }

        public void UpdateIdleTimer(float deltaTime)
        {
            _idleTimer.Value =
                Mathf.Max(0f, _idleTimer.Value - deltaTime);
        }

        public float GetHPRate()
        {
            if (_model.MaxHp <= 0f)
                return 0f;

            return _model.CurrentHp.Value / _model.MaxHp;
        }

        public float GetCurrentHP()
        {
            return _model.CurrentHp.Value;
        }

        public void OnDamageStarted()
        {
            _model.IsDamaged.Value = true;
        }

        public void OnTakeDamage(float damage)
        {
            _model.DecreaseHP(damage);
        }

        public void OnDamageEnded()
        {
            _model.IsDamaged.Value = false;
        }

        private bool CanAttack()
        {
            if (IsDead || IsDamaged.Value || IsAttacking.Value)
                return false;

            return true;
        }

        private bool CanStartJump()
        {
            if (IsDead || IsDamaged.Value || IsAttacking.Value)
                return false;
            return true;
        }

        public bool TryStartAttack()
        {
            if (!CanAttack())
                return false;

            _model.IsAttacking.Value = true;
            return true;
        }

        public void EndAttack()
        {
            _model.IsAttacking.Value = false;
        }

        public bool TryStartJump()
        {
            if (!CanStartJump())
                return false;

            _model.IsJumping.Value = true;
            return true;
        }

        public void OnJumpFinished()
        {
            _model.IsJumping.Value = false;        
        }

        public void OnIdleStarted()
        {
            _model.IsIdle.Value = true;
        }

        public void OnIdleFinished()
        {
            _idleTimer.Value = 0f;
            _model.IsIdle.Value = false;
        }

        public BallJumpAction ResolveJumpAction()
        {
            if (IsDead || IsDamaged.Value || IsIdle.Value || IsAttacking.Value)
                return BallJumpAction.None;

            if (IsJumping.Value)
                return BallJumpAction.CheckLanding;

            return BallJumpAction.Jump;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _idleTimer.Dispose();
        }
    }
}
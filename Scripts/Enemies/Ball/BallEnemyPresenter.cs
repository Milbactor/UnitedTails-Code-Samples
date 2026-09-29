using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public sealed class BallEnemyPresenter : IEnemyPresenter, IDisposable
    {
        private readonly BallEnemyUseCase _useCase;
        private readonly BallEnemyView _view;
        private readonly IReadOnlyList<IEnemyHitBoxView> _hitboxViews;
        private readonly BallEnemySensorView _sensorAreaView;
        private readonly BallEnemyAttackAreaView _attackAreaView;
        private readonly BallResetPositionView _resetPositionView;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly OffScreenIndicatorView _offScreenIndicatorView;


        private bool _deathTriggered;

        public IReadOnlyList<IEnemyHitBoxView> HitBoxViews => _hitboxViews;

        public BallEnemyPresenter(
            BallEnemyUseCase useCase,
            BallEnemyView view,
            IReadOnlyList<IEnemyHitBoxView> hitboxViews,
            BallEnemySensorView sensorAreaView,
            BallEnemyAttackAreaView attackAreaView,
            BallResetPositionView resetPositionView,
            OffScreenIndicatorView offScreenIndicatorView
            )
        {
            _useCase = useCase;
            _view = view;
            _hitboxViews = hitboxViews;
            _sensorAreaView = sensorAreaView;
            _attackAreaView = attackAreaView;
            _resetPositionView = resetPositionView;
            _offScreenIndicatorView = offScreenIndicatorView;

            BindSensor();
            BindViewEvents();
            BindHitBoxes();
            BindUpdate();
            BindAttackArea();
            BindOffScreenIndicator();
        }

        private void BindSensor()
        {
            _sensorAreaView.OnTargetEntered
                .Subscribe(target =>
                {
                    _view.AddTarget(target);

                }) .AddTo(_disposables);

            _sensorAreaView.OnTargetExited
                .Subscribe(_view.RemoveTarget)
                .AddTo(_disposables);
        }

        private void BindAttackArea()
        {
            _view.OnUpdate.Subscribe(_ =>
            {
                if (_useCase.IsAttacking.Value)
                {
                    _view.SteerAttackTowardTarget();
                }
            })
            .AddTo(_disposables);

            _view.OnTargetEntered
            .Subscribe(_ =>
            {
                if (_useCase.IsIdle.Value)
                {
                    _useCase.OnIdleFinished();
                }

                if (_useCase.TryStartAttack())
                {
                    _view.StartAttack();
                }
            })
            .AddTo(_disposables);

            _attackAreaView.OnTargetEntered
              .Subscribe(target =>
              {
                  _view.OnAttackAreaEntered(target);

              }).AddTo(_disposables);

            _attackAreaView.OnTargetExited
             .Subscribe(target =>
             {
                 _view.OnAttackAreaExited(target);

             }).AddTo(_disposables);
        }

        private void BindViewEvents()
        {
            _view.OnJumpFinished
                .Subscribe(_ =>
                {
                    _useCase.OnJumpFinished();
                    _useCase.OnIdleStarted();
                })
                .AddTo(_disposables);

            _view.OnAttackFinished
                .Subscribe(_ => 
                { 
                    _useCase.EndAttack();
                    _useCase.OnIdleStarted();
                })
                .AddTo(_disposables);

            _view.OnDamageEnded
                .Subscribe(_ =>
                {
                    _useCase.OnDamageEnded();
                    _useCase.OnIdleStarted();
                })
                .AddTo(_disposables);

            _resetPositionView.OnPositionReset
                .Subscribe(_ =>
                {
                    float currentHP = _useCase.GetCurrentHP();
                    float reduceHP = currentHP - 1;
                    _useCase.OnTakeDamage(reduceHP);
                })
                .AddTo(_disposables);
        }

        private void BindHitBoxes()
        {
            foreach (IEnemyHitBoxView hitBoxView in _hitboxViews)
            {
                hitBoxView.OnDamageGiven
                    .Subscribe(damageInfo => ApplyDamageToTarget(damageInfo, hitBoxView))
                    .AddTo(_disposables);

                hitBoxView.OnDamageTaken
                    .Subscribe(damageGivenInfo =>
                    {
                        if (_useCase.IsDamaged.Value || _useCase.IsDead)
                            return;
                        if (_useCase.IsIdle.Value) { _useCase.OnIdleFinished(); }
                        if (_useCase.IsAttacking.Value) { _useCase.EndAttack(); }
                        if (_useCase.IsJumping.Value) { _useCase.OnJumpFinished(); }

                        _useCase.OnDamageStarted();
                        _useCase.OnTakeDamage(damageGivenInfo.Power);
                        damageGivenInfo.Target.GetComponentInParent<IThirdPersonCharacterAttackView>()?.Bounce();

                        if (!_deathTriggered && _useCase.IsDead)
                        {
                            _deathTriggered = true;
                            _view.Die();
                            _offScreenIndicatorView?.SetIndicatorEnabled(false);
                            _offScreenIndicatorView?.HideIndicator();
                            return;
                        }
                        _view.OnTakeDamageStarted(_useCase.GetHPRate());
                        _view.KnockBack(damageGivenInfo.Target.transform.position);

                    })
                .AddTo(_disposables);
            }
        }

        private void ApplyDamageToTarget(DamageGivenInfo damageInfo, IEnemyHitBoxView enemyHitBoxView)
        {
            if (!damageInfo.Target.CompareTag("Player"))
                return;

            var damageView =
                damageInfo.Target.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView == null)
               return;

            damageView.TakeDamage(damageInfo.Power);

            Vector3 bounceDirection =
                damageInfo.Target.transform.position - damageInfo.HitPoint;

            if (bounceDirection.sqrMagnitude < 0.0001f)
            {
                bounceDirection =
                    damageInfo.Target.transform.position - _view.transform.position;
            }
            damageView.Bounce(bounceDirection);

            var normal = (_view.transform.position - damageInfo.Target.transform.position).normalized;
            _view.ShowGiveDamageEffect(damageInfo.HitPoint, normal);
        }

        private void BindUpdate()
        {
            _view.OnUpdate
                .Where(_ => !_useCase.IsDead)
                .Subscribe(_ =>
                {
                    if (_useCase.IsIdle.Value)
                    {
                        _useCase.UpdateIdleTimer(Time.deltaTime);
                        return;
                    }
                 
                    var action = _useCase.ResolveJumpAction();
                    switch (action)
                    {
                        case BallJumpAction.CheckLanding:
                            _view.CheckLanding();
                            break;

                        case BallJumpAction.Jump:
                            if (_useCase.TryStartJump())
                            {
                                _view.StartRandomJump();
                            }
                            break;

                        case BallJumpAction.None:
                            break;
                    }
                })
                .AddTo(_disposables);
        }

        private void BindOffScreenIndicator()
        {
            if (_offScreenIndicatorView == null)
                return;
            _view.OnUpdate
                .Select(_ =>
                    !_useCase.IsDead &&
                    _view.CurrentTarget != null)
                .DistinctUntilChanged()
                .Subscribe(isEnabled =>
                {
                    _offScreenIndicatorView.SetIndicatorEnabled(isEnabled);
                })
                .AddTo(_disposables);
            _view.OnUpdate
                .Select(_ => _useCase.IsDead)
                .DistinctUntilChanged()
                .Where(isDead => isDead)
                .Subscribe(_ =>
                {
                    _offScreenIndicatorView.OnDead();
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }

    public enum BallJumpAction
    {
        None,
        CheckLanding,
        Jump
    }
}

using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class HumanoidEnemyPresenter : IEnemyPresenter
    {
        private readonly IHumanoidEnemyUseCase _useCase;
        private readonly HumanoidEnemyView _view;
        private readonly IReadOnlyList<IEnemyHitBoxView> _hitboxViews;
        private readonly HumanoidEnemySensorView _sensorAreaView;
        private readonly HumanoidEnemyAttackAreaView _attackAreaView;

        public IReadOnlyList<IEnemyHitBoxView> HitBoxViews =>
            _hitboxViews;

        private readonly CompositeDisposable _disposables =
            new CompositeDisposable();

        private bool _deathTriggered;

        public HumanoidEnemyPresenter(
            IHumanoidEnemyUseCase useCase,
            HumanoidEnemyView view,
            List<HumanoidEnemyHitBoxView> hitboxViews,
            HumanoidEnemySensorView sensorView,
            HumanoidEnemyAttackAreaView attackAreaView)
        {
            _view = view;
            _useCase = useCase;
            _hitboxViews = hitboxViews;
            _sensorAreaView = sensorView;
            _attackAreaView = attackAreaView;

            _sensorAreaView.OnTargetEntered
                .Subscribe(target =>
                {
                    _view.AddTarget(target);
                })
                .AddTo(_disposables);

            _sensorAreaView.OnTargetExited
                .Subscribe(target =>
                {
                    _view.RemoveTarget(target);
                })
                .AddTo(_disposables);

            foreach (IEnemyHitBoxView hitboxView in _hitboxViews)
            {
                hitboxView.OnDamageGiven
                    .Subscribe(ApplyDamageToTarget)
                    .AddTo(_disposables);
            }

            _view.OnDamageTaken
              .Subscribe(damageGivenInfo =>
              {
                  if (_useCase.IsDamaged.Value)
                      return;

                  _useCase.StartDamage(
                      _view.DamageCooldown
                  );

                  _useCase.OnTakeDamage(
                      damageGivenInfo.Power
                  );

                  float hpRate = _useCase.GetHPRate();

                  _view.OnTakeDamageStarted(hpRate);

                  damageGivenInfo.Target
                      .GetComponentInParent<
                          IThirdPersonCharacterAttackView>()
                      ?.Bounce();

                  if (!_deathTriggered &&
                      _useCase.IsDead)
                  {
                      _view.Die();
                      _deathTriggered = true;
                  }
              })
              .AddTo(_disposables);

              _view.OnAttackFinished
                    .Subscribe(_ =>
                    {
                        _useCase.EndAttack(
                            _view.AttackCooldown
                        );
                    })
                    .AddTo(_disposables);

            _useCase.CanMove
                .DistinctUntilChanged()
                .Subscribe(canMove =>
                {
                    if (canMove)
                        _view.OnWalkStarted();
                    else
                        _view.OnWalkEnded();
                })
                .AddTo(_disposables);

            _view.OnUpdate
             .Where(_ => !_useCase.IsDead)
             .Subscribe(_ =>
             {
                 // Cooldownは攻撃中でも毎フレーム進める
                 _useCase.Tick(
                     _view.MoveInterval,
                     Time.deltaTime
                 );

                 // 攻撃中は移動・次の攻撃判定だけ止める
                 if (_useCase.IsAttacking.Value)
                     return;

                 Transform currentTarget =
                     _view.CurrentTarget;
                 if (_attackAreaView.HasTargetInAttackArea &&
                     _useCase.CanAttack())
                 {
                     _view.StopMoving();
                     _view.FaceTarget(currentTarget);

                     _useCase.StartAttack();
                     _view.OnWalkEnded();
                     _view.PlayAttack();

                     return;
                 }

                 if (_useCase.CanMove.Value)
                 {
                     if (currentTarget != null)
                         _view.ChaseCurrentTarget();
                     else
                         _view.MoveToDestination();
                 }

                 if (_useCase
                     .ShouldDecideNewDestination.Value)
                 {
                     _view.DecideNewDestination();
                     _useCase.SetNewDestination(false);
                 }
             })
             .AddTo(_disposables);
                }

        private void ApplyDamageToTarget(
            DamageGivenInfo damageInfo)
        {
            if (!damageInfo.Target.CompareTag("Player"))
                return;

            var damageView =
                damageInfo.Target.GetComponentInParent<
                    IThirdPersonCharacterDamageView>();

            if (damageView == null)
                return;

            _view.FaceTarget(
                damageInfo.Target.transform
            );

            Vector3 forward =
                damageInfo.Target.transform.position -
                damageInfo.HitPoint;

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward =
                    damageInfo.Target.transform.position -
                    _view.transform.position;
            }

            damageView.TakeDamage(damageInfo.Power);
            damageView.Bounce(forward);

            Vector3 normal =
                (_view.transform.position -
                 damageInfo.Target.transform.position)
                .normalized;

            _view.ShowGiveDamageEffect(
                damageInfo.HitPoint,
                normal
            );
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class SpiderEnemyPresenter : IEnemyPresenter
    {
        private readonly ISpiderEnemyUseCase _useCase = null;
        private readonly SpiderEnemyView _view = null;
        private readonly IReadOnlyList<IEnemyHitBoxView> _hitboxViews = new List<SpiderEnemyHitBoxView>();
        private readonly SpiderEnemySensorView _sensorAreaView;
        private readonly SpiderEnemyAttackAreaView _attackAreaView;

        public IReadOnlyList<IEnemyHitBoxView> HitBoxViews => _hitboxViews;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _deathTriggered = false;
        public SpiderEnemyPresenter(
            ISpiderEnemyUseCase useCase, 
            SpiderEnemyView view, 
            List<SpiderEnemyHitBoxView> hitboxViews,
            SpiderEnemySensorView sensorAreaView,
            SpiderEnemyAttackAreaView attackAreaView
            )
        {
            _view = view;
            _useCase = useCase;
            _hitboxViews = hitboxViews;
            _sensorAreaView = sensorAreaView;
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

            foreach (var hitboxView in _hitboxViews) 
            {
                hitboxView.OnDamageGiven
                    .Subscribe(damageInfo =>
                    {
                        if (_useCase.IsDead) return;
                        ApplyDamageToTarget(damageInfo);
                    })
                    .AddTo(_disposables);
            }

            _view.OnDamageTaken
                .Subscribe(damageGivenInfo =>
                {
                    if (_useCase.IsDamaged.Value) return;

                    _useCase.OnTakeDamage(damageGivenInfo.Power);
                    var hpRate = _useCase.GetHPRate();
                    _view.OnTakeDamageStarted(hpRate);
                    damageGivenInfo.Target.GetComponentInParent<IThirdPersonCharacterAttackView>()?.Bounce();
       
                    if (!_deathTriggered && _useCase.IsDead == true)
                    {
                        _view.Die();
                        _deathTriggered = true;
                    }
                }).AddTo(_disposables);

            _view.OnAttackFinished
                .Subscribe(_ => _useCase.EndAttack()).AddTo(_disposables);

            _useCase.CanWalk
                .DistinctUntilChanged()
                .Subscribe(canMove => {
                    if (canMove) { _view.OnWalkStarted(); }
                    else { _view.OnWalkEnded(); }
                })
                .AddTo(_disposables);

            _view.OnUpdate
             .Where(_ => !_useCase.IsDead)
             .Where(_ => !_useCase.IsAttacking.Value)
             .Subscribe(_ =>
             {
                 _useCase.Tick(
                     _view.MoveInterval,
                     Time.deltaTime
                 );

                 if (_attackAreaView.HasTargetInAttackArea &&
                     _useCase.CanAttack())
                 {
                     _useCase.StartAttack();
                     _view.OnWalkEnded();
                     _view.PlayAttack();
                     return;
                 }

                 if (_useCase.CanWalk.Value)
                 {
                     if (_view.CurrentTarget != null)
                     {
                         _view.ChaseCurrentTarget();
                     }
                     else
                     {
                         _view.MoveToDestination();
                     }
                 }

                 if (_useCase.ShouldDecideNewDestination.Value)
                 {
                     _view.DecideNewDestination();
                     _useCase.SetNewDestination(false);
                 }
             })
             .AddTo(_disposables);
        }

        private void ApplyDamageToTarget(DamageGivenInfo damageInfo)
        {
            if (damageInfo.Target.tag != "Player") return;
            var damageView = damageInfo.Target.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView == null) return;

            Vector3 forward =
                damageInfo.Target.transform.position - damageInfo.HitPoint;

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = damageInfo.Target.transform.position - _view.transform.position;
            }

            damageView.TakeDamage(damageInfo.Power);
            damageView.Bounce(forward);

            var normal = (_view.gameObject.transform.position - damageInfo.Target.transform.position).normalized;
            _view.ShowGiveDamageEffect(damageInfo.HitPoint, normal);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
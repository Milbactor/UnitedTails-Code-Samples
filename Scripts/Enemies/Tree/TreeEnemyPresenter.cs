using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemyPresenter : IDisposable, IEnemyPresenter
    {
        private readonly TreeEnemyView _treeEnemyView;
        private readonly TreeSporeView _treeSporeView;
        private readonly TreeEnemyAttackAreaView _attackAreaView;
        private readonly TreeEnemySensorView _sensorView;
        private readonly TreeEnemyUseCase _useCase;

        private IDisposable _sporeCooldownDisposable;
        private const float SporeCooldownSeconds = 3f;

        private readonly CompositeDisposable _disposables = new();

        public IReadOnlyList<IEnemyHitBoxView> HitBoxViews => throw new NotImplementedException();

        public TreeEnemyPresenter(
            TreeEnemyView treeEnemyView,
            TreeSporeView treeSporeView,
            TreeEnemyAttackAreaView attackAreaView,
            TreeEnemySensorView treeEnemySensorView,
            TreeEnemyUseCase useCase
            )
        {
            _treeEnemyView = treeEnemyView;
            _treeSporeView = treeSporeView;
            _attackAreaView = attackAreaView;
            _sensorView = treeEnemySensorView;
            _useCase = useCase;

            Bind();
        }

        private void Bind()
        {
            _sensorView.CurrentTarget
               .DistinctUntilChanged()
                .Subscribe(target =>
                {
                    _treeEnemyView.LookTarget = target;
                })
                .AddTo(_disposables);

            _treeEnemyView.OnDamageTaken
                .Subscribe(damageInfo =>
                {
                    _useCase.TakeDamage(damageInfo.Power);
                    damageInfo.Target.GetComponentInParent<IThirdPersonCharacterAttackView>().Bounce();
                    _treeEnemyView.OnTakeDamageStarted( _useCase.CurrentHp.Value);
                })
                .AddTo(_disposables);

            _treeEnemyView.OnDamageTakenEnded
                .Subscribe(_ =>
                {
                    _useCase.FinishDamage();
                })
                .AddTo(_disposables);

            _treeEnemyView.OnDamageGiven
                .Subscribe(info =>
                {
                    ApplyDamageToTarget(info);
                })
                .AddTo(_disposables);

            _attackAreaView.OnTargetEntered
                .Subscribe(target =>
                {
                    _useCase.AddAttackTarget(target);
                })
                .AddTo(_disposables);

            _attackAreaView.OnTargetExited
                .Subscribe(target =>
                {
                    _useCase.RemoveAttackTarget(target);

                })
                .AddTo(_disposables);
            
            _useCase.HasAttackTargets
                .CombineLatest(
                    _useCase.IsSporeCoolingDown,
                        (hasTargets, isCoolingDown) =>
                            hasTargets && !isCoolingDown)
                    .DistinctUntilChanged()
                    .Subscribe(canAttack =>
                    {
                        if (canAttack)
                        {
                            _treeSporeView.PlayAttack();
                        }
                        else
                        {
                            _treeSporeView.StopEmission();
                        }
                    })
                    .AddTo(_disposables);

            foreach (TreeSporeHitBoxView hitView in _treeSporeView.HitViews)
            {
                hitView.OnDamageGiven
                    .Subscribe(info =>
                    {
                        StartSporeCooldown();
                        ApplyDamageToTarget(info);
                    })
                    .AddTo(_disposables);
            }

            foreach (TreeEnemyHitBoxView hitView in _treeEnemyView.HitViews)
            {
                hitView.OnDamageGiven
                    .Subscribe(info =>
                    {
                        ApplyDamageToTarget(info);
                    })
                    .AddTo(_disposables);

                hitView.OnDamageTaken
                    .Subscribe(info =>
                    {
                        _treeEnemyView.TakeDamage(info);
                        info.Target.GetComponentInParent<IThirdPersonCharacterAttackView>().Bounce();
                        _useCase.TakeDamage(info.Power);
                        var hpRate = _useCase.CurrentHp.Value;
                        _treeEnemyView.OnTakeDamageStarted(hpRate);
                    })
                    .AddTo(_disposables);
            }
            _useCase.OnDied
                .Take(1)
                .Subscribe(_ =>
                {
                    _treeSporeView.DisableAll();
                    _treeEnemyView.Die();
                })
                .AddTo(_disposables);
        }

        private void ApplyDamageToTarget(DamageGivenInfo damageInfo)
        {
            var damageView =
                         damageInfo.Target.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView == null) return;

            Vector3 forward =
            damageInfo.Target.transform.position - damageInfo.HitPoint;

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = damageInfo.Target.transform.position - _treeEnemyView.transform.position;
            }

            damageView.TakeDamage(damageInfo.Power);
            damageView.Bounce(forward);
            var normal = (_treeEnemyView.transform.position - damageInfo.Target.transform.position).normalized;
            _treeEnemyView.ShowGiveDamageEffect(damageInfo.HitPoint, normal);
        }

        private void StartSporeCooldown()
        {
            // 同時に複数の胞子が当たってもタイマーを乱立させない
            if (_useCase.IsSporeCoolingDown.Value)
                return;

            _useCase.SetSporeCoolingDown(true);
            _sporeCooldownDisposable?.Dispose();

            _sporeCooldownDisposable =
                Observable.Timer(
                        TimeSpan.FromSeconds(SporeCooldownSeconds))
                    .Subscribe(_ =>
                    {
                        _useCase.SetSporeCoolingDown(false);
                        _sporeCooldownDisposable = null;
                    });
        }

        public void Dispose()
        {
            _sporeCooldownDisposable?.Dispose();
            _disposables.Dispose();
        }
    }
}
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class AIBehaviourInput : CombatSettingViewBase, IAIBehaviourInput, IEnemyDeathListener, ICharacterLifeCycle
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float partnerSideOffset = 2f;

        private ActorStateProvider _actorStateProvider;
        private Transform _partner;
        private Transform _target;
        private Collider _targetCollider;

        public Transform Partner => _partner;
        public Transform Target => _target;
        public Transform SelfTransform => this.transform;
        public bool HasTarget => _target != null;

        public float DistanceToTarget
        {
            get
            {
                if (_target == null)
                    return float.MaxValue;

                var closestPoint = _targetCollider.ClosestPoint(transform.position);

                return Vector3.Distance(
                    transform.position,
                    closestPoint);
            }
        }

        public bool TryGetClosestTargetPoint(out Vector3 point)
        {
            point = default;

            if (_target == null)
                return false;

            point = _targetCollider != null
                ? _targetCollider.ClosestPoint(transform.position)
                : _target.position;

            return true;
        }

        public bool IsPartnerSpinJumping => 
            _actorStateProvider != null ? _actorStateProvider.IsSpinJumping.Value : false;
        public bool IsPartnerJumping =>
            _actorStateProvider != null &&
            (
                _actorStateProvider.IsJumping.Value ||
                _actorStateProvider.IsSpinJumping.Value ||
                !_actorStateProvider.IsGround.Value
            );

        private IEnemyView CurrentEnemyView
        {
            get
            {
                if (_target == null) return null;

                var enemyView = _target
                    .GetComponentsInParent<MonoBehaviour>()
                    .OfType<IEnemyView>()
                    .FirstOrDefault();

                return enemyView != null && !enemyView.IsDead
                    ? enemyView
                    : null;
            }
        }
        
        public float StopDistance
        {
            get
            {
                var enemyView = CurrentEnemyView;

                if (enemyView == null || enemyView.IsDead)
                    return CombatSetting.StopDistance;

                return ResolveDistance(
                    CombatSetting.StopDistance,
                     DistanceToTarget);
            }
        }

        public float AttackRange
        {
            get
            {
                var enemyView = CurrentEnemyView;

                if (enemyView == null || enemyView.IsDead)
                    return CombatSetting.AttackRange;

                return ResolveDistance(
                    CombatSetting.AttackRange,
                    DistanceToTarget);
            }
        }

        public float SpinJumpRange
        {
            get
            {
                var enemyView = CurrentEnemyView;

                if (enemyView == null || enemyView.IsDead)
                    return CombatSetting.SpinJumpRange;

                return ResolveDistance(
                    CombatSetting.SpinJumpRange,
                      DistanceToTarget);
            }
        }

        public float SpinAttackRange => CombatSetting.SpinAttackStartDistance;

        public bool PreferAerialAttack()
        {
            var enemyView = CurrentEnemyView;

            if (enemyView == null || enemyView.IsDead)
                return false;

            if (!TryGetClosestTargetPoint(out var targetPoint))
                return false;

            float targetHeight =
                targetPoint.y - transform.position.y;

            return targetHeight >=
                   CombatSetting.AerialAttackHeightThreshold;
        }

        private float ResolveDistance(float defaultValue, float currentDistance)
        {
            return Mathf.Max(defaultValue, currentDistance);
        }

        [Inject]
        public void Construct(CharacterDeathNotifier characterDeathNotifier)
        {
            characterDeathNotifier.Register(this);
        }

        private void Start()
        {
            Observable.Timer(
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(0.2f))
                .Where(_ => !_dead)
                .Subscribe(_ => DetectEnemy())
                .AddTo(this);
        }

        public void SetPartner(Transform partner)
        {
            _partner = partner;
            _actorStateProvider = _partner.GetComponent<ActorStateProvider>();
        }

        public void GiveUpEnemy()
        {
            _target = null;
        }

        public Vector3 GetMoveTargetPosition(AIMoveState currentMoveState)
        {
            if (!HasTarget)
                return GetPartnerFollowPosition();

            switch (currentMoveState)
            {
                case AIMoveState.ChasingEnemy:
                    return Target.position;

                case AIMoveState.Attacking:
                    return Target.position;

                default:
                    return GetPartnerFollowPosition();
            }
        }

        private Vector3 GetPartnerFollowPosition()
        {
            if (_partner == null)
                return transform.position;

            return _partner.position
                 + _partner.right * partnerSideOffset;
        }

        private void DetectEnemy()
        {
            if (_partner == null)
                return;

            float distanceToPartner =
                Vector3.Distance(transform.position, _partner.position);

            if (distanceToPartner > CombatSetting.PartnerLeaveDistance)
            {
                GiveUpEnemy();
                return;
            }

            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                CombatSetting.EquipBuffer,
                enemyLayer,
                QueryTriggerInteraction.Collide
            );

            IEnemyView nearestEnemy = null;
            Collider nearestCollider = null;
            float nearestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit == null)
                    continue;

                var enemyView = hit
                    .GetComponentsInParent<MonoBehaviour>(true)
                    .OfType<IEnemyView>()
                    .FirstOrDefault();

                if (enemyView == null || enemyView.IsDead)
                    continue;
                
                if (hit == null)
                    continue;

                Collider combatCollider = enemyView.CombatCollider;

                if (combatCollider == null)
                    continue;

                Vector3 closestPoint =
                    combatCollider.ClosestPoint(transform.position);

                float distance = Vector3.Distance(
                    transform.position,
                    closestPoint);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemyView;
                    nearestCollider = combatCollider;
                }
            }

            if (nearestEnemy != null)
            {
                _target = ((MonoBehaviour)nearestEnemy).transform;
                _targetCollider = nearestCollider;
                return;
            }

            var currentEnemy = CurrentEnemyView;

            if (currentEnemy != null)
            {
                Collider currentCollider = currentEnemy.CombatCollider;

                if (currentCollider != null)
                {
                    Vector3 closestPoint =
                        currentCollider.ClosestPoint(transform.position);

                    float currentDistance = Vector3.Distance(
                        transform.position,
                        closestPoint);

                    if (currentDistance <= CombatSetting.EquipBuffer)
                        return;
                }
            }
            _target = null;
            _targetCollider = null;
        }

        public void OnEnemyDied(IEnemyView enemy)
        {
            if (_target != null)
            {
                if (_target.GetComponentInParent<IEnemyView>() == enemy)
                {
                    GiveUpEnemy();
                    DetectEnemy();
                }
            }
            else { DetectEnemy(); }
        }

        public void OnDead()
        {    
            _dead = true;
            GiveUpEnemy();
        }

        public float HeightDistanceToTarget => _target != null
            ? _target.position.y - transform.position.y
            : 0f;

        private bool _dead = false;
        public bool IsDead => _dead;
    }
}
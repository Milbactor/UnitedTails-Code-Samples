using UniRx;


namespace WhiteKNight
{
    public class SpiderEnemyModel : IEnemyModel
    {
        public bool IsDead => _hp.Value <= 0f;

        public float MaxHp { get; } = 100f;
        public ReactiveProperty<float> CurrentHp => _hp;
        private readonly ReactiveProperty<float> _hp = new ReactiveProperty<float>(0);

        public ReactiveProperty<bool> IsAttacking => _isAttacking;
        private readonly ReactiveProperty<bool> _isAttacking = new ReactiveProperty<bool>(false);

        public ReactiveProperty<bool> IsIdle => _isIdle;
        private readonly ReactiveProperty<bool> _isIdle = new ReactiveProperty<bool>(false);

        public ReactiveProperty<bool> IsDamaged => _isDamaged;
        private readonly ReactiveProperty<bool> _isDamaged = new ReactiveProperty<bool>(false);

        public ReactiveProperty<bool> IsWalking => _isWalking;
        private readonly ReactiveProperty<bool> _isWalking = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanWalk => _canWalk;
        private readonly ReactiveProperty<bool> _canWalk = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> ShouldDecideNewDestination => _shouldDecideNewDestination;
        private readonly ReactiveProperty<bool> _shouldDecideNewDestination = new ReactiveProperty<bool>(false);

        public SpiderEnemyModel()
        {
            _hp.Value = MaxHp;
        }

        public void DecreaseHP(float hp)
        {
            _hp.Value -= hp;
        }

        public void OnAttackStarted()
        {
            _isAttacking.Value = true;
        }
        
        public void OnAttackFinished()
        {
            _isAttacking.Value = false;
        }

        public void SetCanMoveState()
        {
            _canWalk.Value = !_isIdle.Value && !_isAttacking.Value && !_isDamaged.Value;
        }
    }
}



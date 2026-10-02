using UniRx;

namespace WhiteKNight
{
    public interface IBallEnemyModel : IEnemyModel
    {
        ReactiveProperty<bool> IsJumping { get; }
        ReactiveProperty<bool> IsWaitingForNextJump { get; }
    }

    public class BallEnemyModel : IBallEnemyModel
    {
        private readonly ReactiveProperty<bool> _isJumping = new(false);
        public ReactiveProperty<bool> IsJumping => _isJumping;

        private readonly ReactiveProperty<bool> _isWaitingForNextJump = new(false);
        public ReactiveProperty<bool> IsWaitingForNextJump => _isWaitingForNextJump;

        public bool IsDead => _currentHp.Value <= 0f;

        private float _maxHp = 100f;
        public float MaxHp => _maxHp;

        private readonly ReactiveProperty<float> _currentHp = new(100f);
        public ReactiveProperty<float> CurrentHp => _currentHp;

        private readonly ReactiveProperty<bool> _isAttacking = new(false);
        public ReactiveProperty<bool> IsAttacking => _isAttacking;

        private readonly ReactiveProperty<bool> _isIdle = new(true);
        public ReactiveProperty<bool> IsIdle => _isIdle;

        private readonly ReactiveProperty<bool> _isDamaged = new(false);
        public ReactiveProperty<bool> IsDamaged => _isDamaged;

        private readonly ReactiveProperty<bool> _isWalking = new(false);
        public ReactiveProperty<bool> IsWalking => _isWalking;

        private readonly ReactiveProperty<bool> _canWalk = new(true);
        public ReactiveProperty<bool> CanWalk => _canWalk;

        private readonly ReactiveProperty<bool> _shouldDecideNewDestination = new(false);
        public ReactiveProperty<bool> ShouldDecideNewDestination => _shouldDecideNewDestination;

        public BallEnemyModel(float maxHP)
        {
            _maxHp = _currentHp.Value = maxHP;
        }

        public void DecreaseHP(float hp)
        {
            _currentHp.Value -= hp;
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
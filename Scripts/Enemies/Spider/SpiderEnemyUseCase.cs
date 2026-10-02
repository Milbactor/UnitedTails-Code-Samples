using UniRx;

namespace WhiteKNight
{
    public class SpiderEnemyUseCase : ISpiderEnemyUseCase
    {
        private IEnemyModel _spiderEnemyModel = null;
        public bool IsDead => _spiderEnemyModel.IsDead;
        public ReactiveProperty<bool> IsAttacking => _spiderEnemyModel.IsAttacking;
        public ReactiveProperty<bool> ShouldDecideNewDestination => _spiderEnemyModel.ShouldDecideNewDestination;
        public ReactiveProperty<bool> IsIdle => _spiderEnemyModel.IsIdle;
        public ReactiveProperty<bool> IsDamaged => _spiderEnemyModel.IsDamaged;
        public ReactiveProperty<bool> CanWalk => _spiderEnemyModel.CanWalk;

        private float _moveTimer = 0f;

        public SpiderEnemyUseCase(IEnemyModel model)
        {
            _spiderEnemyModel = model;
        }

        public float GetHPRate()
        {
            if(_spiderEnemyModel.MaxHp != 0)
            {
                return _spiderEnemyModel.CurrentHp.Value / _spiderEnemyModel.MaxHp;
            }
            return 0;
        }

        public void OnTakeDamage(float damage)
        {
            _spiderEnemyModel.DecreaseHP(damage);
        }

        private void ResetMoveTimer()
        {
            _moveTimer = 0f;
            _spiderEnemyModel.ShouldDecideNewDestination.Value = true;
        }

        public void SetNewDestination(bool shouldSet)
        {
            _spiderEnemyModel.ShouldDecideNewDestination.Value = shouldSet;
        }

        public void Tick(float moveInterval, float deltaTime)
        {
            _spiderEnemyModel.SetCanMoveState();

            _moveTimer += deltaTime;
            if (_moveTimer >= moveInterval)
            {
                ResetMoveTimer();
                SetNewDestination(true);
            }
        }

        public bool CanAttack()
        {
            if (IsDead)
                return false;

            if (IsDamaged.Value)
                return false;

            return true;
        }

        public void StartAttack()
        {
            _spiderEnemyModel.IsAttacking.Value = true;
            _spiderEnemyModel.CanWalk.Value = false;
        }

        public void EndAttack()
        {
            _spiderEnemyModel.IsAttacking.Value = false;
            _spiderEnemyModel.CanWalk.Value = true;
        }
    }
}



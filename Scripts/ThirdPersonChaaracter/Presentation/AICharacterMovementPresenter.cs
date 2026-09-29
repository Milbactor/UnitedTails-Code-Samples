using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterMovementPresenter :  IThirdPersonCharacterMovementPresenter, ICharacterLifeCycle
    {
        private ICharacterMovementUseCase _movementUseCase;
        private readonly IAIMoveCommandUseCase _moveCommandUseCase;
        private readonly IAIJumpDecisionUseCase _jumpDecisionUseCase;
        private AIBehaviourInput _input;
        private AICharacterView _characterView;
        private AICharacterAttackView _characterAttackView;

        private bool shouldApplyRootMotion;
        private AIMoveCommand _command;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private Vector3 _targetPosition;

        public IReadOnlyReactiveProperty<float> GroundDistance => _jumpDecisionUseCase.GroundDistance;

        private bool _isDead = false;
        public bool IsDead => _isDead;

        public AICharacterMovementPresenter(
            ICharacterMovementUseCase movementUseCase,
            IAIAttackUseCase attackUseCase,
            IAIApproachUseCase approachUseCase,
            IAIMoveCommandUseCase moveCommandUseCase,
            AIJumpDecisionUseCase jumpDecisionUseCase,
            AICharacterView characterView,
            AICharacterAttackView characterAttackView,
            AIBehaviourInput aiBehaviorInput,
            CharacterDeathNotifier characterDeathNotifier
            )
        {
            _movementUseCase = movementUseCase;
            _moveCommandUseCase = moveCommandUseCase;
            _input = aiBehaviorInput;
            _characterView = characterView;
            _characterAttackView = characterAttackView;
            _jumpDecisionUseCase = jumpDecisionUseCase;

            characterDeathNotifier.Register(this);

            _characterView.OnUpdate
                .Where(_ => !_isDead)
                .Subscribe(_ =>
                {
                    _targetPosition = _input.GetMoveTargetPosition(_jumpDecisionUseCase.moveState);
                    _jumpDecisionUseCase.Tick(_targetPosition);
                    _moveCommandUseCase.Tick(
                        _input.SelfTransform.position,
                        _targetPosition);

                    _command = _moveCommandUseCase.GetMoveCommand();

                    Tick();
                })
                .AddTo(_disposables);

            characterView.OnJumpFinished
                .Subscribe(_ =>
                {
                    _movementUseCase.OnJumpFinished();
                })
                .AddTo(_disposables);

            characterView.OnSpinJumpFinished
                .Subscribe(_ =>
                {
                    _movementUseCase.OnSpinJumpFinished();
                })
                .AddTo(_disposables);

            _jumpDecisionUseCase.ShouldJump
                .DistinctUntilChanged()
                .Where(shouldJump => shouldJump)
                .Subscribe(_ => HandleJump())
                .AddTo(_disposables);

            _jumpDecisionUseCase.ShouldSpinJump
                .DistinctUntilChanged()
                .Where(shouldSpinJump => shouldSpinJump)
                .Subscribe(_ => HandleSpinJump())
                .AddTo(_disposables);

            _movementUseCase.IsOnGround
                .DistinctUntilChanged()
                .Where(isOnGround => isOnGround)
                .Subscribe(_ =>
                {
                    _movementUseCase.OnJumpFinished();
                    _movementUseCase.OnSpinJumpFinished();
                    //AI landed: Jump states and triggers reset
                    _characterAttackView.ResetAttackTriggers();
                })
                .AddTo(_disposables);
        }

        void Tick()
        {
            _movementUseCase.Tick();
            shouldApplyRootMotion = _movementUseCase.ShouldApplyRootMotion.Value;

            AIMovementMode movementMode = _command.movementMode;
           _characterView.SetMode(movementMode);
            // 接地状態はPhysicsのときだけ反映
            if (movementMode == AIMovementMode.Physics) 
            { 
                _characterView.SetGroundState(_movementUseCase.IsOnGround.Value);
            }

            HandleMove();
            HandleAnimation();
        }

        private void HandleMove()
        {
            var groundNormal = _movementUseCase.GroundNormal.Value;

            var mode = _command.movementMode;
            Vector3 worldDir;

            if (mode == AIMovementMode.NavMesh)
            {
                _characterView.SetDestination(_targetPosition);
                worldDir = _characterView.Velocity;
                worldDir.y = 0f;
            }
            else { worldDir = _command.Direction; }

            _characterView.Move(worldDir, groundNormal, mode);

            if (mode == AIMovementMode.Physics)
            {
                _characterView.SetApplyRootMotion(shouldApplyRootMotion);
                if (!_movementUseCase.IsOnGround.Value) { _characterView.HandleAirborneMovement(); }
            }
        }

        private void HandleAnimation()
        {
            _characterView.UpdateAnimator(_characterView.Velocity, _movementUseCase.IsOnGround.Value);
        }

        private void HandleJump()
        {
            _characterView.HandleGroundedMovement(_command.Direction, _jumpDecisionUseCase.ShouldJump.Value);
            _characterView.TriggerJump();
            _movementUseCase.OnJumpStarted();            
        }

        private void HandleSpinJump()
        {
            _jumpDecisionUseCase.OnSpinStarted();
            _movementUseCase.OnSpinJumpStarted();

            _characterView.HandleSpinJumpMovement(
                _command.Direction,
                _jumpDecisionUseCase.ShouldSpinJump.Value);
        }

        public void OnDead()
        {
            if(_isDead) return;
            _isDead = true;

            _disposables.Dispose();
        }
    }
}
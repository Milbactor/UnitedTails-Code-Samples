using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterMovementPresenter : IThirdPersonCharacterMovementPresenter
    {
        private readonly ThirdPersonCharacterMovementUseCase _useCase;
        private readonly IThirdPersonCharacterInput _input;
        private readonly ThirdPersonCharacterView _characterView;
        private readonly ActorStateProvider _actorStateProvider;
        private readonly ICameraView _cameraView;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public IReadOnlyReactiveProperty<float> GroundDistance => _useCase.GroundDistance;

        private float _ungroundedTime;
        private const float FallAnimationDelay = 0.08f;

        public ThirdPersonCharacterMovementPresenter(
            ThirdPersonCharacterMovementUseCase useCase,
            IThirdPersonCharacterInput input,
            ThirdPersonCharacterView characterView,
            ActorStateProvider actorStateProvider,
            ICameraView cameraView
        )
        {
            _useCase = useCase;
            _input = input;
            _characterView = characterView;
            _actorStateProvider = actorStateProvider;
            _cameraView = cameraView;

            useCase.IsOnGround
                .SkipLatestValueOnSubscribe()
                .DistinctUntilChanged()
                .Subscribe(characterView.SetGroundState)
                .AddTo(_disposables);

            useCase.IsJumping
                .Skip(1)
                .Subscribe(isJumping =>
                {
                    actorStateProvider.IsJumping.Value = isJumping;
                })
                .AddTo(_disposables);

            useCase.IsSpinJumping
                .Skip(1)
                .Subscribe(isSpinning =>
                {
                    actorStateProvider.IsSpinJumping.Value = isSpinning;
                })
                .AddTo(_disposables);

            useCase.IsOnGround
                .Skip(1)
                .Subscribe(onGround =>
                {
                    actorStateProvider.IsGround.Value = onGround;
                })
                .AddTo(_disposables);

            characterView.OnUpdate
                .Subscribe(_ => Tick())
                .AddTo(_disposables);

            characterView.OnJumpFinished
                .Subscribe(_ => useCase.OnJumpFinished())
                .AddTo(_disposables);

            characterView.OnSpinJumpFinished
                .Subscribe(_ => useCase.OnSpinJumpFinished())
                .AddTo(_disposables);

            characterView.OnFixedUpdate
                .Subscribe(_ =>
                {
                    if (!_useCase.IsOnGround.Value)
                    {
                        _characterView.HandleAirborneMovement();
                    }
                })
                .AddTo(_disposables);
        }

        private void Tick()
        {
            _useCase.Tick();

            var jump = _input.JumpPressed;
            var isGround = _useCase.IsOnGround.Value;

            HandleJump(jump);

            var moveInput = _input.MoveInput;
            var direction = BuildMoveDirection(moveInput, isGround);

            HandleMove(direction, isGround);
            HandleAnimation(moveInput);
        }

        private Vector3 BuildMoveDirection(Vector2 moveInput, bool isGround)
        {
            if (isGround)
            {
                float forward = moveInput.y;

                if (Mathf.Abs(moveInput.x) > 0.01f &&
                    Mathf.Abs(moveInput.y) < 0.01f)
                {
                    forward = 0.5f;
                }

                var localDirection = new Vector3(
                    moveInput.x,
                    0f,
                    forward
                );

                var direction =
                    _characterView.TransformDirection(localDirection);

                if (direction.sqrMagnitude > 1f)
                {
                    direction.Normalize();
                }

                return direction;
            }

            return BuildAirMoveDirection(moveInput);
        }

        private Vector3 BuildAirMoveDirection(Vector2 moveInput)
        {
            if (_cameraView == null)
            {
                return Vector3.zero;
            }

            float forward = Mathf.Max(0f, moveInput.y);

            float turn = Mathf.Clamp(
                _input.MouseX + moveInput.x,
                -1f,
                1f
            );

            var localDirection = new Vector3(turn, 0f, forward);
            var direction = _characterView.TransformDirection(localDirection);

            return direction.sqrMagnitude > 1f
                ? direction.normalized
                : direction;
        }

        private void HandleMove(Vector3 direction, bool isGround)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector3.zero;
            }

            var groundNormal = _useCase.GroundNormal.Value;
            var shouldApplyRootMotion =
                _useCase.ShouldApplyRootMotion.Value;

            _characterView.Move(direction, groundNormal);

            _characterView.SetApplyRootMotion(
                shouldApplyRootMotion
            );

        }

        private void HandleJump(bool inputJumpPressed)
        {
            if (!inputJumpPressed)
            {
                return;
            }

            if (_useCase.CanStartJump())
            {
                _characterView.HandleGroundedMovement();
                _characterView.StartJump();
                _useCase.OnJumpStarted();
            }
            else if (_useCase.CanStartSpinJump())
            {
                _characterView.HandleSpinJumpMovement();
                _characterView.StartSpinJump();
                _useCase.OnSpinJumpStarted();
            }
        }

        private void HandleAnimation(Vector2 input)
        {
            bool isGrounded = _useCase.IsOnGround.Value;

            if (isGrounded)
            {
                _ungroundedTime = 0f;
            }
            else
            {
                _ungroundedTime += Time.deltaTime;
            }

            bool groundedForAnimation =
                isGrounded ||
                _ungroundedTime < FallAnimationDelay;

            _characterView.UpdateAnimator(
                input,
                groundedForAnimation
            );
        }

        public IActorStateProvider GetActorStateProvider()
        {
            return _actorStateProvider;
        }
    }
}
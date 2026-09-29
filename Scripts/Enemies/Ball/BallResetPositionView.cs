using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class BallResetPositionView : MonoBehaviour
    {
        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private float maxDistanceFromInitialPosition = 30f;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private bool _isResetting;

        private readonly Subject<Unit> _onPositionReset = new();
        public IObservable<Unit> OnPositionReset => _onPositionReset;

        private void Awake()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;

            if (_targetRenderer == null)
            {
                _targetRenderer = GetComponentInChildren<Renderer>();
            }
        }

        private void Start()
        {
            float maxDistanceSqr =
                maxDistanceFromInitialPosition * maxDistanceFromInitialPosition;

            Observable.EveryFixedUpdate()
                .Where(_ => !_isResetting)
                .Where(_ =>
                    (transform.position - _initialPosition).sqrMagnitude
                    >= maxDistanceSqr)
                .Subscribe(_ =>
                {
                    RequestResetAfterLeavingBattleArea();
                    _onPositionReset.OnNext(Unit.Default);
                })
                .AddTo(this);

        }

        public void RequestResetAfterLeavingBattleArea()
        {
            if (_isResetting)
            {
                return;
            }

            _isResetting = true;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;

            Observable.EveryUpdate()
                .Where(_ => !IsVisibleFromMainCamera())
                .Take(1)
                .Subscribe(_ => ResetToInitialPosition())
                .AddTo(this);
        }

        private bool IsVisibleFromMainCamera()
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null || _targetRenderer == null)
            {
                return false;
            }

            Plane[] cameraPlanes =
                GeometryUtility.CalculateFrustumPlanes(mainCamera);

            return GeometryUtility.TestPlanesAABB(
                cameraPlanes,
                _targetRenderer.bounds);
        }

        private void ResetToInitialPosition()
        {
            transform.SetPositionAndRotation(
                _initialPosition,
                _initialRotation);

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = false;

            _isResetting = false;
        }
    }
}
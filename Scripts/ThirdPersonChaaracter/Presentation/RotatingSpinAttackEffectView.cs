using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class RotatingSpinAttackEffectView : MonoBehaviour
    {
        [Header("Effect")]
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Transform _rotationRoot;

        [Header("Timing")]
        [SerializeField] private float _duration = 1.2f;
        [SerializeField] private float _fadeDuration = 0.3f;

        [Header("Rotation")]
        [SerializeField] private Vector3 _rotationAxis = Vector3.right;
        [SerializeField] private float _rotationSpeed = 360f;

        private static readonly int AlphaId =
            Shader.PropertyToID("_Alpha");

        private MaterialPropertyBlock _propertyBlock;
        private IDisposable _playDisposable;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            if (_rotationRoot == null)
                _rotationRoot = transform;

            SetAlpha(0f);
            gameObject.SetActive(false);
        }

        public void Play()
        {
            _playDisposable?.Dispose();

            gameObject.SetActive(true);
            SetAlpha(0f);

            _playDisposable = Observable.EveryUpdate()
                .Select(_ => Time.deltaTime)
                .Scan(0f, (elapsed, deltaTime) => elapsed + deltaTime)
                .TakeWhile(elapsed => elapsed < _duration)
                .DoOnCompleted(FinishEffect)
                .Subscribe(elapsed =>
                {
                    RotateEffect();
                    SetAlpha(CalculateAlpha(elapsed));
                });
        }

        private void FinishEffect()
        {
            SetAlpha(0f);
            _playDisposable = null;
            gameObject.SetActive(false);
        }

        private void RotateEffect()
        {
            _rotationRoot.Rotate(
                _rotationAxis,
                _rotationSpeed * Time.deltaTime,
                Space.Self);
        }

        private float CalculateAlpha(float elapsedTime)
        {
            // 最初の0.3秒
            if (elapsedTime < _fadeDuration)
            {
                float rate = elapsedTime / _fadeDuration;
                return Mathf.SmoothStep(0f, 1f, rate);
            }

            // 最後の0.3秒
            float fadeOutStartTime = _duration - _fadeDuration;

            if (elapsedTime > fadeOutStartTime)
            {
                float rate =
                    (elapsedTime - fadeOutStartTime) / _fadeDuration;

                return Mathf.SmoothStep(1f, 0f, rate);
            }

            return 1f;
        }

        private void SetAlpha(float alpha)
        {
            foreach (Renderer targetRenderer in _renderers)
            {
                if (targetRenderer == null)
                    continue;

                targetRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(AlphaId, alpha);
                targetRenderer.SetPropertyBlock(_propertyBlock);
            }
        }
        private void OnDestroy()
        {
            _playDisposable?.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WhiteKNight
{
    public class HPView : MonoBehaviour, IHPView
    {
        [SerializeField] private Slider _mainHPSlider;
        [SerializeField] private Slider _damageSlider;
        [SerializeField] private List<Image> _tipImages = new List<Image>();
        [SerializeField] private List<Image> _rightTipBackgroundImages;

        [SerializeField] private float _delaySeconds = 0.25f;
        [SerializeField] private float _followSpeed = 0.6f;

        private readonly SerialDisposable _damageDisposable = new();

        private Color _color;

        public void Initialize(Color color)
        {
            _color = color;
            foreach (Image image in _tipImages) { image.color = _color; }
            foreach (Image image in _rightTipBackgroundImages) { image.color = _color; }
            _mainHPSlider.value = 1f;
        }

        public void SetHPRate(float rate)
        {
            rate = Mathf.Clamp01(rate);
            _mainHPSlider.value = rate;

            if (rate < 1f)
            {
                foreach (Image image in _rightTipBackgroundImages) { image.enabled = false; }
            }

            if (rate <= 0f)
            {
                foreach (Image image in _tipImages) { image.enabled = false; }
            }

            if (rate == 1)
            {
                foreach (Image image in _rightTipBackgroundImages) { image.enabled = true; }
            }

            _damageDisposable.Disposable = Observable
                .Timer(TimeSpan.FromSeconds(_delaySeconds))
                .SelectMany(_ => Observable.EveryUpdate())
                .TakeWhile(_ => !Mathf.Approximately(_damageSlider.value, rate))
                .Subscribe(
                    _ =>
                    {
                        _damageSlider.value = Mathf.MoveTowards(
                            _damageSlider.value,
                            rate,
                            Time.deltaTime * _followSpeed
                        );
                    },
                    () =>
                    {
                        _damageSlider.value = rate;
                    }
                );
        }

        private void OnDestroy()
        {
            _damageDisposable.Dispose();
        }
    }
}


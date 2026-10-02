using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class CharacterDepartureView : MonoBehaviour, ICharacterDepartureView
    {
        [SerializeField] private DepartureDirection _departureDirection;
        [SerializeField] private float _duration = 60f;
        [SerializeField] private float _speed = 5f;

        private IDisposable _departureDisposable;
       
        public void StartDeparture()
        {
            _departureDisposable?.Dispose();

            Vector3 direction = _departureDirection == DepartureDirection.Up
                ? Vector3.up
                : Vector3.down;

            _departureDisposable = Observable.EveryUpdate()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    transform.position += direction * _speed * Time.deltaTime;
                });

            Observable.Timer(TimeSpan.FromSeconds(_duration))
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    Destroy(gameObject);
                })
                .AddTo(this);
        }

        public enum DepartureDirection
        {
            Up,
            Down
        }
    }
}
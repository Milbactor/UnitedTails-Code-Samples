using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class PuritusView : MonoBehaviour
    {
        private readonly Subject<float> _onPuritusObtained = new Subject<float>();
        public IObservable<float> OnPuritusObtained => _onPuritusObtained;

        [SerializeField] private float _recovery = 10f;

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) { return; }

            _onPuritusObtained.OnNext(_recovery);
            SoundManager.Instance?.PlaySE(SoundEffectId.Recovery);
            this.gameObject.SetActive(false);
        }
    }
}
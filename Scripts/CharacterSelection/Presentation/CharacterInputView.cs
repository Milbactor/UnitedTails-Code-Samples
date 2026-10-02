using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class CharacterInputView : MonoBehaviour, ICharacterInputView
    {
        [SerializeField] private string characterId;
        [SerializeField] private SoundEffectId _hoverID;
        [SerializeField] private SoundEffectId _clickID;

        private readonly Subject<string> _onHoverEnter = new();
        private readonly Subject<string> _onHoverExit = new();
        private readonly Subject<string> _onClicked = new();

        public IObservable<string> OnHoverEnter => _onHoverEnter;
        public IObservable<string> OnHoverExit => _onHoverExit;
        public IObservable<string> OnClicked => _onClicked;
        public string CharacterId { get => characterId; }

        private void Start()
        {
            OnHoverEnter.Subscribe(_ => { SoundManager.Instance?.PlaySE(_hoverID); }).AddTo(this);
            OnClicked.Subscribe(_ => { SoundManager.Instance?.PlaySE(_clickID); }).AddTo(this);
        }

        public void OnPointerEnter()
        {
            _onHoverEnter.OnNext(CharacterId);
        }

        public void OnPointerExit()
        {
            _onHoverExit.OnNext(CharacterId);
        }

        public void OnPointerDown()
        {
            _onClicked.OnNext(CharacterId);
        }

        private void OnDestroy()
        {
            _onHoverEnter.Dispose();
            _onHoverExit.Dispose();
            _onClicked.Dispose();
        }
    }
}
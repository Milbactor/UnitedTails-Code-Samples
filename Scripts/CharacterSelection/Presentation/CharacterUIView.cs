using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

namespace WhiteKNight
{
    public class CharacterUIView : MonoBehaviour, ICharacterUIView
    {
        [SerializeField] private GameObject profilePanel;

        [SerializeField] private Button selectButton;
        [SerializeField] private Button cancelButton;

        [SerializeField] private SoundEffectId _okClickID;
        [SerializeField] private SoundEffectId _cancelClickID;


        private readonly Subject<Unit> _onSelectClicked = new();
        private readonly Subject<Unit> _onCancelClicked = new();

        public IObservable<Unit> OnSelectClicked => _onSelectClicked;
        public IObservable<Unit> OnCancelClicked => _onCancelClicked;

        [SerializeField] public string CharacterId;


        private void Awake()
        {
            selectButton.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    _onSelectClicked.OnNext(Unit.Default);
                    SoundManager.Instance?.PlaySE(_okClickID);
            
                }).AddTo(this);

            cancelButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _onCancelClicked.OnNext(Unit.Default);
                    SoundManager.Instance?.PlaySE(_cancelClickID);
                }).AddTo(this);
        }

        public void Hide()
        {
            profilePanel.SetActive(false);
        }

        public void Show()
        {
            profilePanel.SetActive(true);
        }
    }
}
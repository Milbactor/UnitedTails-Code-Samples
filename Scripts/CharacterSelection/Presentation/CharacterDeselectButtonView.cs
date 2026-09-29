using System;
using UniRx;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace WhiteKNight
{
    public class CharacterDeselectButtonView : MonoBehaviour, ICharacterDeselectButtonView
    {
        [SerializeField] private string characterId;
        [SerializeField] private Button deselectCharacterButton;

        private readonly Subject<string> _onButtonClicked = new();
        public IObservable<string> OnButtonClicked => _onButtonClicked;

        public string CharacterId { get => characterId; }


        void Awake()
        {
            deselectCharacterButton.OnClickAsObservable()
                .Subscribe(_ => _onButtonClicked.OnNext(characterId))
                .AddTo(this);
        }
    }
}
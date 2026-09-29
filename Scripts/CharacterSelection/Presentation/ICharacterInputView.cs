using System;

namespace WhiteKNight
{
    public interface ICharacterInputView
    {
        public string CharacterId { get; }
        IObservable<string> OnHoverEnter { get; }
        IObservable<string> OnHoverExit { get; }
        IObservable<string> OnClicked { get; }

        void OnPointerEnter();

        void OnPointerDown();

        void OnPointerExit();
    }
}
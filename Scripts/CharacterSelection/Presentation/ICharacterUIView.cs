using System;
using UniRx;

namespace WhiteKNight
{
    public interface ICharacterUIView
    {
        IObservable<Unit> OnSelectClicked { get; }
        IObservable<Unit> OnCancelClicked { get; }


        void Hide();
        void Show();
    }
}
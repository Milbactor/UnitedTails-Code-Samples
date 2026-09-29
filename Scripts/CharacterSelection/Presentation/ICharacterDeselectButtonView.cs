using System;
using UniRx;

namespace WhiteKNight
{
    public interface ICharacterDeselectButtonView
    {
        IObservable<string> OnButtonClicked { get; }
    }
}
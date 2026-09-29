using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;

namespace WhiteKNight
{
    public interface IOpenSceneTransitionView
    {
        IObservable<Unit> OnSceneStart { get; }

        UniTask PlayFadeInAsync(float duration, CancellationToken token);
    }
}
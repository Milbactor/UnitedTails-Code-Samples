using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace WhiteKNight
{
    public class FadeView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public IObservable<Unit> FadeTo(float targetAlpha, float duration)
        {
            float startAlpha = _image.color.a;

            return Observable.EveryUpdate()
                .Select(_ => Time.deltaTime)
                .Scan(0f, (time, delta) => time + delta)
                .TakeWhile(time => time < duration)
                .Do(time =>
                {
                    var color = _image.color;
                    color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                    _image.color = color;
                })
                .AsUnitObservable()
                .Finally(() =>
                {
                    // final correction
                    var color = _image.color;
                    color.a = targetAlpha;
                    _image.color = color;
                });
        }
    }
}

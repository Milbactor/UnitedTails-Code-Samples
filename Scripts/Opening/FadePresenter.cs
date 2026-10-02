using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class FadePresenter
    {
        private FadeView _view;

        private const float FADE_TIME = 1.5f;

  
        public FadePresenter(FadeView fadeView)
        {
            _view = fadeView;
        }

        public IObservable<Unit> FadeOut()
        {
            return _view.FadeTo(0f, FADE_TIME);
        }

        public IObservable<Unit> FadeIn()
        {
            return _view.FadeTo(1f, FADE_TIME);
        }
    }
}

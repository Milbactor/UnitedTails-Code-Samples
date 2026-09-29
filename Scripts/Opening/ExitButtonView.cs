using System;
using UniRx;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteKnight
{
    public class ExitButtonView : MonoBehaviour
    {
        public IObservable<Unit> OnClickAsObservable => _onClickSubject;
        private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();

        void Awake()
        {
            this.GetComponent<UnityEngine.UI.Button>()
                .OnClickAsObservable()
                .Subscribe(_ => _onClickSubject.OnNext(Unit.Default))
                .AddTo(this);
        }

        void OnDestroy()
        {
            _onClickSubject?.Dispose();
        }
    }

}
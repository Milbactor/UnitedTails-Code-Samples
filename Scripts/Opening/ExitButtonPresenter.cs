using UniRx;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace WhiteKnight
{
    public class ExitButtonPresenter : MonoBehaviour
    {
        [SerializeField] private ExitButtonView _view;
        private CompositeDisposable _disposables = new CompositeDisposable();

        void Awake()
        {
            if (_view == null)
                _view = GetComponent<ExitButtonView>();

            _view.OnClickAsObservable
                 .Subscribe(_ =>
                 {
                     Debug.Log("Exit button clicked. End game...");
#if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
                 })
                 .AddTo(_disposables);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }

    }

}
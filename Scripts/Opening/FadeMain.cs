using System;
using UnityEngine;
using Zenject;

namespace WhiteKnight
{
    [DefaultExecutionOrder(999)]
    public class FadeMain : MonoBehaviour
    {
        private FadeUseCase _fadeUseCase;

        [Inject]
        public void Construct(FadeUseCase fadeUseCase)
        {
            _fadeUseCase = fadeUseCase;
        }

        private async void Awake()
        {
            try
            {
                await _fadeUseCase.Begin();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnDestroy()
        {
            _fadeUseCase?.Finish();
        }
    }
}

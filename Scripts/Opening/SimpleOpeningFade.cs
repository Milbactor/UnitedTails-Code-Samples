using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WhiteKnight
{
    public class SimpleOpeningFade : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _duration = 1f;

        private void Awake()
        {
            // Sceneの最初のフレームから白にしておく
            _fadeImage.gameObject.SetActive(true);
            _fadeImage.color = Color.white;
        }

        private IEnumerator Start()
        {
            float time = 0f;

            while (time < _duration)
            {
                time += Time.deltaTime;

                float alpha = 1f - Mathf.Clamp01(time / _duration);

                Color color = _fadeImage.color;
                color.a = alpha;
                _fadeImage.color = color;

                yield return null;
            }

            Color finalColor = _fadeImage.color;
            finalColor.a = 0f;
            _fadeImage.color = finalColor;

            _fadeImage.gameObject.SetActive(false);
        }
    }
}
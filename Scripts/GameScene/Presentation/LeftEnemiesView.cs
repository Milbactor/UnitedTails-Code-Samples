using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WhiteKNight
{
    public class LeftEnemiesView : MonoBehaviour, ILeftEnemiesView
    {
        [SerializeField] private Image _whiteRateImage;
        [SerializeField] private List<Image> _digitImages = new List<Image>();
        [SerializeField] private List<Sprite> _digitSprites = new List<Sprite>();


        private void Start()
        {
            _whiteRateImage.fillAmount = 0;
        }

        public void SetEnemyRate(float rate)
        {
            var amount = 1 - rate;
            if (amount < 0) { amount = 0; }
            _whiteRateImage.fillAmount = amount;
        }

        public void InitializeDigits(int total)
        {
            SetTotalDigits(total);
            SetDefeatedDigits(0);
        }

        public void SetTotalDigits(int total)
        {
            total = Mathf.Clamp(total, 0, 99);
            SetTwoDigits(2, total); // Total：digitImages[2], [3]
        }

        public void SetDefeatedDigits(int defeated)
        {
            defeated = Mathf.Clamp(defeated, 0, 99);
            SetTwoDigits(0, defeated); // Remaining：digitImages[0], [1]
        }

        private void SetTwoDigits(int startIndex, int value)
        {
            if (_digitImages == null || _digitImages.Count <= startIndex + 1) return;
            if (_digitSprites == null || _digitSprites.Count < 10) return;

            value = Mathf.Clamp(value, 0, 99);

            int tens = value / 10;
            int ones = value % 10;

            // 十の位
            if (tens == 0)
            {
                _digitImages[startIndex+1].enabled = false;
            }
            else
            {
                _digitImages[startIndex+1].enabled = true;
                _digitImages[startIndex+1].sprite = _digitSprites[tens];
            }

            // 一の位
            _digitImages[startIndex].enabled = true;
            _digitImages[startIndex].sprite = _digitSprites[ones];
        }
    }
}
using System.Collections;
using UnityEngine;

namespace WhiteKNight
{
    public class ResultUIView : MonoBehaviour
    {
        [SerializeField] private GameObject WinUI;
        [SerializeField] private GameObject LostUI;

        private bool _isUIShowing = false;

        public void OnWin()
        {
            this.gameObject.SetActive(true);
            WinUI.SetActive(true);
            if (_isUIShowing) return;
            StartCoroutine(HideUI(WinUI));
        }

        public void OnLost()
        {
            this.gameObject.SetActive(true);
            LostUI.SetActive(true);
            if (_isUIShowing) return;
            StartCoroutine(HideUI(LostUI));
        }

        IEnumerator HideUI(GameObject ui)
        {
            _isUIShowing = true;
            yield return new WaitForSeconds(5f);
            ui.SetActive(false);
            _isUIShowing = false;
        }
    }
}
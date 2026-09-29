using UnityEngine;

namespace WhiteKNight
{
    public class WeaponSwapView : MonoBehaviour, IWeaponSwapView
    {
        [SerializeField] private GameObject swordOnBack;
        [SerializeField] private GameObject swordInHand;

        public void SetSwordOnBack()
        {
            swordOnBack.SetActive(true);
            swordInHand.SetActive(false);
        }

        public void SetSwordInHand()
        {
            swordOnBack.SetActive(false);
            swordInHand.SetActive(true);
        }
    }
}
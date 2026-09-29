using UnityEngine;
using UnityEngine.UI;

namespace WhiteKNight
{
    public class CharacterNameView : MonoBehaviour
    {
        [SerializeField] private Image _nameImage;

        public void SetNameImage(Sprite characterNameSpirte)
        {
            _nameImage.sprite = characterNameSpirte;
        }
    }
}
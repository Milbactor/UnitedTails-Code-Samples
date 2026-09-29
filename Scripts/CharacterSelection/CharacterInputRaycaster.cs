using UnityEngine;

namespace WhiteKNight
{
    public class CharacterInputRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private ICharacterInputView lastHover;

        void Update()
        {
            bool isClicked = false;
            if (Input.GetMouseButtonDown(0))
            {
                isClicked = true;
            }

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var view = hit.collider.GetComponent<ICharacterInputView>();
      
                if (view != lastHover)
                {
                    lastHover?.OnPointerExit();
                    view?.OnPointerEnter();
                    lastHover = view;
                }
                if (isClicked == true)
                {
                    view?.OnPointerDown();
                }
            }
            else if (lastHover != null)
            {
                lastHover.OnPointerExit();
                lastHover = null;
            }
        }
    }
}
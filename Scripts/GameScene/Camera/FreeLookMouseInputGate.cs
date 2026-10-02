using UnityEngine;
using Cinemachine;

namespace WhiteKNight
{
    public class FreeLookMouseInputGate : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook freeLook;
       // [SerializeField] private int rotateMouseButton = 1; // 1 = 右クリック, 2 = 中クリック

        [SerializeField] private float xSensitivity = 3f;
        [SerializeField] private float ySensitivity = 0.02f;

        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minRadius = 2f;
        [SerializeField] private float maxRadius = 8f;

        private float currentRadius = 5f;

        void Update()
        {
            bool rotating =
                Input.GetMouseButton(1) || Input.GetMouseButton(2);

            if (rotating)
            {
                freeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X") * xSensitivity;
                freeLook.m_YAxis.Value += Input.GetAxis("Mouse Y") * ySensitivity;
                freeLook.m_YAxis.Value = Mathf.Clamp01(freeLook.m_YAxis.Value);
            }
            else
            {
                freeLook.m_XAxis.m_InputAxisValue = 0f;
                freeLook.m_YAxis.m_InputAxisValue = 0f;
            }

            float scroll = Input.mouseScrollDelta.y;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentRadius -= scroll * zoomSpeed;
                currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);

                for (int i = 0; i < 3; i++)
                {
                    var orbit = freeLook.m_Orbits[i];
                    orbit.m_Radius = currentRadius;
                    freeLook.m_Orbits[i] = orbit;
                }
            }
        }
    }
}
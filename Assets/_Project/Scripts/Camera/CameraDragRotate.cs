using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Camera
{
    public class CameraDragRotate : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook freeLookCamera;
        [SerializeField] private float sensitivityX = 3f;
        [SerializeField] private float sensitivityY = 2f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 10f;

        private bool _isRotating;

        private void Update()
        {
            // HandleRotation();
            // HandleZoom();
        }

        private void HandleRotation()
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                _isRotating = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Mouse.current.middleButton.wasReleasedThisFrame)
            {
                _isRotating = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (_isRotating)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                freeLookCamera.m_XAxis.m_InputAxisValue = delta.x * sensitivityX;
                freeLookCamera.m_YAxis.m_InputAxisValue = delta.y * sensitivityY;
            }
            else
            {
                freeLookCamera.m_XAxis.m_InputAxisValue = 0;
                freeLookCamera.m_YAxis.m_InputAxisValue = 0;
            }
        }

        private void HandleZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                float zoomDelta = scroll * zoomSpeed * Time.deltaTime;

                // Меняем радиусы всех трёх орбит
                for (int i = 0; i < 3; i++)
                {
                    var orbit = freeLookCamera.m_Orbits[i];
                    orbit.m_Radius = Mathf.Clamp(orbit.m_Radius - zoomDelta, minZoom, maxZoom);
                    freeLookCamera.m_Orbits[i] = orbit;
                }
            }
        }
    }
}

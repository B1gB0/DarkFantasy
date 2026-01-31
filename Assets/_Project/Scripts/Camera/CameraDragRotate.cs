using UnityEngine;
using Cinemachine;

public class CameraDragRotate : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private float sensitivity = 0.2f;

    private bool isDragging = false;
    private Vector2 lastPos;

    private void Update()
    {
        // ПК — ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Если ЛКМ не зажата — не вращаем
        if (!Input.GetMouseButton(0))
        {
            isDragging = false;
        }

        // Мобилки — тач
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastPos = t.position;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        if (isDragging)
        {
            Vector2 currentPos = Input.touchCount > 0 
                ? (Vector2)Input.GetTouch(0).position 
                : (Vector2)Input.mousePosition;

            Vector2 delta = currentPos - lastPos;

            // Вращаем только по горизонтали
            freeLookCamera.m_XAxis.Value += delta.x * sensitivity;

            // Фиксируем вертикальную ось, чтобы камера не приближалась
            freeLookCamera.m_YAxis.Value = 0.5f;

            lastPos = currentPos;
        }
    }
}



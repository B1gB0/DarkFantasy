using UnityEngine;

namespace _Project.Scripts.UI
{
    public class InventoryRig : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _previewCamera;
        [SerializeField] private Transform _modelAnchor;

        public UnityEngine.Camera Camera => _previewCamera;
        public Transform ModelAnchor => _modelAnchor;

        public void SetActive(bool active)
        {
            if (_previewCamera != null)
                _previewCamera.enabled = active;

            gameObject.SetActive(active);
        }
    }
}
using UnityEngine;

namespace _Project.Scripts.Props
{
    public class Lanterns : MonoBehaviour
    {
        [SerializeField] private Light lightSource;
        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.4f;
        [SerializeField] private float speed = 10f;

        private float _targetIntensity;

        private void Start()
        {
            if (lightSource == null)
                lightSource = GetComponent<Light>();

            _targetIntensity = lightSource.intensity;
        }

        private void Update()
        {
            if (Mathf.Abs(lightSource.intensity - _targetIntensity) < 0.05f)
            {
                _targetIntensity = Random.Range(minIntensity, maxIntensity);
            }
            
            lightSource.intensity = Mathf.Lerp(
                lightSource.intensity,
                _targetIntensity,
                Time.deltaTime * speed
            );
        }
    }
}

using UnityEngine;

namespace _Project.Scripts.Props
{
    public class Lanterns : MonoBehaviour
    {
        [SerializeField] private Light lightSource;
        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.4f;
        [SerializeField] private float speed = 10f;

        private float targetIntensity;

        private void Start()
        {
            if (lightSource == null)
                lightSource = GetComponent<Light>();

            targetIntensity = lightSource.intensity;
        }

        private void Update()
        {
            // Когда достигли цели — выбираем новую
            if (Mathf.Abs(lightSource.intensity - targetIntensity) < 0.05f)
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            }

            // Плавное движение к цели
            lightSource.intensity = Mathf.Lerp(
                lightSource.intensity,
                targetIntensity,
                Time.deltaTime * speed
            );
        }
    }
}

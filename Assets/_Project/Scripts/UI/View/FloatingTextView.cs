using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.View
{
    public class FloatingTextView : View
    {
        private const float Delay = 4f;
        private const float ZOffset = 0.2f;
        private const float YOffset = 0.5f;

        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private SpriteRenderer _icon;

        private void OnEnable()
        {
            StartCoroutine(LifeRoutine());
        }

        private void OnDisable()
        {
            StopCoroutine(LifeRoutine());
        }

        public void SetFloatingText(string value, Transform target, FloatingTextViewType viewType, Color color)
        {
            _text.text = viewType == FloatingTextViewType.Healing ? $"+{value}" : $"{value}";
            _text.color = color;

            SetIcon(viewType);
            
            transform.position = new Vector3(
                target.position.x,
                target.position.y + YOffset,
                target.position.z - ZOffset);
        }

        private void SetIcon(FloatingTextViewType viewType)
        {
            _icon.sprite = viewType switch
            {
                FloatingTextViewType.Gold => _sprites[0],
                _ => null
            };
        }

        private IEnumerator LifeRoutine()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(Delay);

            yield return waitForSeconds;

            Deactivate();
        }
    }
}
using _Project.Scripts.UI.View;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public class FloatingTextService : IFloatingTextService
    {
        private const string ObjectPoolDamageText = "PoolDamageText";
        private const int Count = 4;
        private const bool IsAutoExpand = true;

        private ObjectPool<FloatingTextView> _poolDamageText;
        private IPlayerService _playerService;

        [Inject] private void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public void Init(FloatingTextView textView)
        {
            _poolDamageText =
                new ObjectPool<FloatingTextView>(textView, Count, new GameObject(ObjectPoolDamageText).transform)
                {
                    AutoExpand = IsAutoExpand,
                };
        }

        public void OnSpawnFloatingText(
            string value,
            Transform target,
            FloatingTextViewType floatingTextViewType,
            Color color)
        {
            ChangeText(value, target, floatingTextViewType, color);
        }

        private void ChangeText(string value, Transform target, FloatingTextViewType floatingTextViewType, Color color)
        {
            FloatingTextView textView = _poolDamageText.GetFreeElement();
            textView.SetFloatingText(value, target, floatingTextViewType, color);
            textView.Activate();

            Vector3 directionToCamera = _playerService.FreeLookCamera.transform.position - textView.transform.position;
            textView.transform.rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);
        }
    }
}
using _Project.Scripts.UI.View;
using UnityEngine;

namespace _Project.Scripts.Services
{
    public interface IFloatingTextService
    {
        public void OnSpawnFloatingText(
            string value,
            Transform target,
            FloatingTextViewType floatingTextViewType,
            Color color);

        public void Init(FloatingTextView textView);
    }
}
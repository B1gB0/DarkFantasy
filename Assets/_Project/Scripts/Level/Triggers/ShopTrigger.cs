using System;
using UnityEngine;

namespace _Project.Scripts.Level.Triggers
{
    public class ShopTrigger : Trigger
    {
        public event Action OnOpenShop;

        public bool IsShopOpen { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Player.Player _))
                return;

            if (IsShopOpen)
                return;

            IsShopOpen = true;
            OnOpenShop?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player.Player _))
                IsShopOpen = false;
        }
    }
}
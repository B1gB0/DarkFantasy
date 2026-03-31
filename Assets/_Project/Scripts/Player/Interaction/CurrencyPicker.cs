using UnityEngine;

namespace _Project.Scripts.Player.Interaction
{
    public class CurrencyPicker : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Currency.Currency currency))
            {
                Storage.Storage.AddValue();
                Destroy(currency.gameObject);
                Debug.Log("Взял монетку");
            }
        }
    }
}
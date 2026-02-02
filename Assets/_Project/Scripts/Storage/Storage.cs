using UnityEngine;

namespace _Project.Scripts.Storage
{
    public class Storage : MonoBehaviour
    {
        public static int Currency {get; private set;}

        public static void AddValue(int value = 1)
        {
            Currency += value;
        }
    }
}
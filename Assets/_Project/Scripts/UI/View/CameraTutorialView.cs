using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Project.Scripts.UI.View
{
    public class CameraTutorialView : MonoBehaviour
    {
        [SerializeField] private Image _mouseIcon;

        private void Start()
        {
            _mouseIcon.gameObject.SetActive(YG2.envir.isDesktop);
        }
    }
}
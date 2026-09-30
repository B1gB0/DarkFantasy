using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI.View
{
    public abstract class View : MonoBehaviour
    {
        public virtual void Show() { }
        public virtual void Hide() { }
        
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            EventSystem.current?.SetSelectedGameObject(null);
            gameObject.SetActive(false);
        }
    }
}

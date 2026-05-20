using DG.Tweening;

namespace _Project.Scripts.UI.View
{
    public class JoystickView : View
    {
        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
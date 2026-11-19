using UnityEngine;

namespace Project.Sources.UI.Synty.InterfaceFantasyWarriorHUD.Samples.Scripts
{
    /// <summary>
    ///     Opens a URL in the default browser.
    /// </summary>
    public class SampleURL : MonoBehaviour
    {
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
    }
}

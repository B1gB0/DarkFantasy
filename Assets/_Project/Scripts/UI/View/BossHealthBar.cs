using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.View
{
    public class BossHealthBar : HealthBar
    {
        [SerializeField] private TMP_Text _name;
        
        public void SetName(string bossName)
        {
            _name.text = bossName;
        }
    }
}
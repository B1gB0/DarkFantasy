using UnityEditor;
using UnityEngine;

namespace Project.Sources.Characters_and_Envitoimnet.POLYGON_SYNTY.Synty.SyntyPackageHelper.Editor
{
    [CustomEditor(typeof(SyntyPackageHelperConfig))]
    public class ExamplePackConfigLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SyntyPackageHelperConfig config = (SyntyPackageHelperConfig)target;
            if (GUILayout.Button("Install Packages"))
            {
                SyntyPackageHelper.ProcessConfigs( new SyntyPackageHelperConfig[] { config }, true);
            }
        }
    }

    [CreateAssetMenu(fileName = "SyntyPackageHelperConfig", menuName = "Scriptable Objects/SyntyPackageHelperConfig")]
    public class SyntyPackageHelperConfig : ScriptableObject
    {
        public string assetPackDisplayName;
        public string[] packageIds;
        public bool hasPromptedUser;
    }
}
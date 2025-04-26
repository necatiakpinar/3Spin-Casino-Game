using Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SpinCasinoEditor : EditorWindow
    {
        private int _enteredLevelIndex;

        [MenuItem("Tools/Spin Casino")]
        public static void ShowWindow()
        {
            GetWindow<SpinCasinoEditor>("SpinCasinoEditor");
            Player.LoadSaveDataFromDisk();
        }

        private void OnGUI()
        {
            GUILayout.Label("MatchPairs Tools", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Clear All Data"))
                Player.ClearAllData();

        }
    }
}
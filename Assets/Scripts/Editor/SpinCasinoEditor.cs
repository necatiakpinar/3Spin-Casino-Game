using Controllers;
using Data;
using Helpers;
using Loggers;
using Miscs;
using UnityEditor;
using UnityEngine;
using ILogger = Interfaces.ILogger;

namespace Editor
{
    public class SpinCasinoEditor : EditorWindow
    {
        private int _enteredLevelIndex;
        
        private static CryptoHelper _cryptoHelper;
        private static JsonHelper _jsonHelper;
        private static SaveSystemController<GameplayData> _gameplaySaveSystemController;
        private static ILogger _logger;
        private static PersistentDataController _persistentDataController;
        
        [MenuItem("Tools/Spin Casino")]
        public static void ShowWindow()
        {
            GetWindow<SpinCasinoEditor>("SpinCasinoEditor");
            
            _logger = new UnityLogger();
            _cryptoHelper = new CryptoHelper();
            _jsonHelper = new JsonHelper();
            _gameplaySaveSystemController = new SaveSystemController<GameplayData>(Constants.GameplayDataPath, _jsonHelper, _cryptoHelper, _logger);
            _gameplaySaveSystemController.LoadSaveDataFromDisk();
            _persistentDataController = new PersistentDataController((GameplayData)_gameplaySaveSystemController.PersistentData);
        }

        private void OnGUI()
        {
            GUILayout.Label("MatchPairs Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear All Data"))
            {
                _gameplaySaveSystemController.ClearAllData();
            }

        }
    }
}
using System;
using Core.Logger;
using Helpers;
using Miscs;
using UnityEngine;

namespace Data
{
    public static class Player
    {
        private static GameplayData _gameplayData;

        public static GameplayData GameplayData => _gameplayData;

        private static string _filePath =
            $"{Application.persistentDataPath}/{Constants.PlayerDataFileName}.{Constants.SaveFileExtension}";

        public static void SaveDataToDisk()
        {
            var data = JsonUtility.ToJson(_gameplayData, true);
            if (FileHelper.WriteToFile(_filePath, data))
            {
                //LoggerUtil.Log("GameplayData successfully saved.");
            }
            else
            {
                LoggerUtil.Log("GameplayData could not be saved.");
            }
        }

        public static void LoadSaveDataFromDisk()
        {
            GameplayData playerData;

            if (FileHelper.LoadFromFile(_filePath, out string diskData))
            {
                try
                {
                    playerData = JsonUtility.FromJson<GameplayData>(diskData);
                }
                catch (Exception e)
                {
                    LoggerUtil.LogError(e);
                    playerData = new GameplayData();
                }
            }
            else
            {
                LoggerUtil.Log("There isn't a disk data.");
                playerData = new GameplayData();
            }

            _gameplayData = playerData;
        }
    }
}
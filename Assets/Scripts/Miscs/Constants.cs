using UnityEngine;

namespace Miscs
{
    public static class Constants
    {
        public static readonly string PlayerDataFileName = "GameData";
        public static readonly string SaveFileExtensionName = "NA";
        public static readonly string GameplayDataPath =
            $"{Application.persistentDataPath}/{PlayerDataFileName}.{SaveFileExtensionName}";
    }
}
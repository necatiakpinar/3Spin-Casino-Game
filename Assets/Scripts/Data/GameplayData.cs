using System.Collections.Generic;

namespace Data
{
    public static class GameplayData
    {
        public static int TotalSpinRatio = 100;
        public static int CurrentSpinIndex = 0;
        public static List<ResultData> Results = new List<ResultData>();
        public static Dictionary<int, ResultData> ResultDictionary = new Dictionary<int, ResultData>();
        
    }
}
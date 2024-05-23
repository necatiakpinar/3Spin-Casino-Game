using System;
using System.Collections.Generic;
using Abstractions;
using Data.Currencies;
using Enums;
using Miscs;

namespace Data
{
    [Serializable]
    public class GameplayData
    {
        public int TotalSpinRatio = 100;
        public int CurrentSpinIndex = 0;
        public List<ResultData> Results = new List<ResultData>();
        public SerializableDictionary<int, List<SlotObjectType>> ResultDictionary = new SerializableDictionary<int, List<SlotObjectType>>();
        public List<BaseCurrency> Currencies = new List<BaseCurrency>();

        public GameplayData()
        {
            Currencies.Add(new CoinCurrency(0, CurrencyType.Coin));
        }
    }
}
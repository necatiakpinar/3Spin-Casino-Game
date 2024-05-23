using System;
using Abstractions;
using Enums;

namespace Data.Currencies
{
    [Serializable]
    public class CoinCurrency : BaseCurrency
    {
        public CoinCurrency(int amount, CurrencyType currencyType) : base(amount, currencyType)
        {
            
        }
    }
}
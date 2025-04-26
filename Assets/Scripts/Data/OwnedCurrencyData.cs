using System;
using Enums;

namespace Data
{
    [Serializable]
    public class OwnedCurrencyData
    {
        public CurrencyType Type;
        public int Amount;
    }
}
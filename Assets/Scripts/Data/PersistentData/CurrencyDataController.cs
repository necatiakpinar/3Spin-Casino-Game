using System;
using System.Collections.Generic;
using Enums;
using EventBus;
using EventBus.Events;

namespace Data.PersistentData
{
    [Serializable]
    public class CurrencyDataController
    {
        public Dictionary<CurrencyType, OwnedCurrencyData> OwnedCurrencies = new()
        {
            { CurrencyType.Coin, new OwnedCurrencyData { Type = CurrencyType.Coin, Amount = 0 } },
        };

        public CurrencyDataController()
        {

        }

        public void IncreaseCurrency(CurrencyType type, int amount)
        {
            var isExist = OwnedCurrencies.TryGetValue(type, out var currency);
            if (isExist)
            {
                currency.Amount += amount;
                EventBusManager.Raise(new CurrencyUpdatedEvent(type, amount));
            }
        }

        public void DecreaseCurrency(CurrencyType type, int amount)
        {
            var isExist = OwnedCurrencies.TryGetValue(type, out var currency);
            if (isExist)
            {
                currency.Amount -= amount;
                if (currency.Amount <= 0)
                    currency.Amount = 0;

                EventBusManager.Raise(new CurrencyUpdatedEvent(type, amount));
            }
        }

        public bool HasEnoughCurrency(CurrencyType type, int amount)
        {
            var isExist = OwnedCurrencies.TryGetValue(type, out var currency);
            if (isExist)
                return currency.Amount >= amount;

            return false;
        }

        public bool TryToDecreaseCurrency(CurrencyType type, int amount)
        {
            if (HasEnoughCurrency(type, amount))
            {
                DecreaseCurrency(type, amount);
                return true;
            }

            return false;
        }

        public OwnedCurrencyData GetOwnedCurrency(CurrencyType type)
        {
            OwnedCurrencies.TryGetValue(type, out var currency);
            return currency;
        }
    }
}
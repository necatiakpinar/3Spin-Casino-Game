using System;
using Enums;
using UnityEngine;

namespace Abstractions
{
    [Serializable]
    public class BaseCurrency
    {
        [SerializeField] private int _amount;
        [SerializeField] private CurrencyType _currencyType;

        public int Amount => _amount;
        public CurrencyType CurrencyType => _currencyType;

        public BaseCurrency(int amount, CurrencyType currencyType)
        {
            _amount = amount;
            _currencyType = currencyType;
        }

        public void AddAmount(int amount)
        {
            _amount += amount;
        }

        public void SubtractAmount(int amount)
        {
            _amount -= amount;
        }
    }
}
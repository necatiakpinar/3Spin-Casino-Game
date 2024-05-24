using System;
using Abstractions;
using Data;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.Widgets
{
    public class CurrencyWidget : BaseWidget
    {
        [SerializeField] private CurrencyType _currencyType;
        [SerializeField] private Image _currencyImage;
        [SerializeField] private TMP_Text _currencyAmountLabel;

        private BaseCurrency _currency;

        private readonly Vector3 _punchScale = new Vector3(1.25f, 1.25f, 1.25f);
        private readonly float _punchDuration = .3f;
        private readonly int _punchVibrato = 2;
        private readonly float _punchElasticity = .5f;

        private void OnEnable()
        {
            Action<object[]> onCurrencyUpdate = (parameters) => UpdateCurrencyLabel((CurrencyType)parameters[0]);
            EventManager.Subscribe(ActionType.OnCurrencyUpdated, onCurrencyUpdate);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(ActionType.OnCurrencyUpdated);
        }

        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            _currency = Player.GameplayData.Currencies.Find(cur => cur.CurrencyType == _currencyType);
            _currencyAmountLabel.text = _currency.Amount.ToString();
        }

        private void UpdateCurrencyLabel(CurrencyType updatedCurrencyType)
        {
            if (_currencyType != updatedCurrencyType)
                return;

            _currencyImage.transform.DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity);
            _currencyAmountLabel.text = _currency.Amount.ToString();
        }
    }
}
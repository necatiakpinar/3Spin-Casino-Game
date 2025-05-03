using System;
using Abstractions;
using Data;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EventBus;
using EventBus.Events;
using Managers;
using Vector3 = UnityEngine.Vector3;

namespace UI.Widgets
{
    public class CurrencyWidget : BaseWidget
    {
        [SerializeField] private CurrencyType _currencyType;
        [SerializeField] private Image _currencyImage;
        [SerializeField] private TMP_Text _currencyAmountLabel;

        private OwnedCurrencyData _currencyData;

        private readonly Vector3 _punchScale = new Vector3(1.25f, 1.25f, 1.25f);
        private readonly float _punchDuration = .3f;
        private readonly int _punchVibrato = 2;
        private readonly float _punchElasticity = .5f;

        private void OnEnable()
        {
            EventBusManager.Subscribe<CurrencyUpdatedEvent>(UpdateCurrencyLabel);
            
        }

        private void OnDisable()
        {
            EventBusManager.Unsubscribe<CurrencyUpdatedEvent>(UpdateCurrencyLabel);
        }

        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            var persistentData = EventBusManager.RaiseWithResult<GetPersistentDataEvent, GameplayData>(new GetPersistentDataEvent());

            _currencyData = persistentData.CurrencyDataController.GetOwnedCurrency(_currencyType);
            var currencyAmountText = _currencyData.Amount.ToString();
            _currencyAmountLabel.text = currencyAmountText;
        }

        private void UpdateCurrencyLabel(CurrencyUpdatedEvent @event)
        {
            if (_currencyType != @event.Type)
                return;

            _currencyImage.transform.DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity);
            var newAmount = _currencyData.Amount.ToString();
            _currencyAmountLabel.text = newAmount;
        }
    }
}
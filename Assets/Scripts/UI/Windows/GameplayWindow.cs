using System;
using Abstractions;
using Enums;
using TMPro;
using UI.Buttons;
using UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameplayWindow : BaseWindow
    {
        [SerializeField] private CurrencyWidget _currencyWidget;
        [SerializeField] private RegularButton _spinButton;

        private void Start()
        {
            Init(); //For now...
        }

        public override void Init(BaseWindowParameters windowParameters = null)
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _spinButton.AddAction(OnSpinButtonClicked);
        }

        public void OnSpinButtonClicked()
        {
            var isSpinning = EventManager.NotifyWithReturn<bool>(FunctionType.CheckIsSpinning);
            if (isSpinning)
            {
                Debug.Log("Can't spin while spinning!");
                return;
            }

            EventManager.Notify(ActionType.OnSpinPressed);
        }
    }
}
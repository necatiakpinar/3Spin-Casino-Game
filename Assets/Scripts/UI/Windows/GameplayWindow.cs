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
            EventManager.Notify(ActionType.OnSpinPressed);
            Debug.Log("Spin button clicked!");
        }
    }
}
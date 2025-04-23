using Abstractions;
using Core.Logger;
using Enums;
using UI.Widgets;
using UnityEngine;

namespace UI.Windows
{
    public class GameplayWindow : BaseWindow
    {
        [SerializeField] private CurrencyWidget _currencyWidget;
        [SerializeField] private BaseButton _spinButton;

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
                LoggerUtil.Log("Can't spin while spinning!");
                return;
            }

            EventManager.Notify(ActionType.OnSpinPressed);
        }
    }
}
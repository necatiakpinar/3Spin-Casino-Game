using Abstractions;
using Enums;
using Helpers;
using Managers;
using UI.Widgets;
using UnityEngine;

namespace UI.Windows
{
    public class GameplayWindow : BaseWindow
    {
        [SerializeField] private CurrencyWidget _currencyWidget;
        [SerializeField] private CustomButton _spinButton;

        private void Start()
        {
            Init(); //For now...(UI Manager required in full project)
        }

        public override void Init(BaseWindowParameters windowParameters = null)
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _spinButton.AddAction(OnSpinButtonClicked);
        }

        private void OnSpinButtonClicked()
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
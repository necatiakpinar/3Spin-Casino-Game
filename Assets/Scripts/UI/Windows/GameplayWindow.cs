using Abstractions;
using Cysharp.Threading.Tasks;
using EventBus;
using EventBus.Events;
using Helpers;
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
            Init(); //For now...(UI Manager required in full project, after creating grid then UI must loaded with state .)
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
            var isSpinning = false;

            var isSpinningEventResult = EventBus<GetSpinningStatusEvent, bool>.Raise(new GetSpinningStatusEvent());
            if (isSpinningEventResult != null)
                isSpinning = isSpinningEventResult[0];

            if (isSpinning)
            {
                LoggerUtil.Log("Can't spin while spinning!");
                return;
            }

            //EventManager.Notify(ActionType.OnSpinPressed);
            EventBus<SpinPressedEvent, UniTask>.Raise(new SpinPressedEvent());
        }
    }
}
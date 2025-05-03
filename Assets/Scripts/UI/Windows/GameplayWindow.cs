using Abstractions;
using Cysharp.Threading.Tasks;
using EventBus;
using EventBus.Events;
using Helpers;
using StateMachines.States;
using UI.Widgets;
using UnityEngine;

namespace UI.Windows
{
    public class GameplayWindow : BaseWindow
    {
        [SerializeField] private CurrencyWidget _currencyWidget;
        [SerializeField] private CustomButton _spinButton;

        private void OnEnable()
        {
            EventBusManager.Subscribe<StateMachineStateChangedEvent>(OnStateMachineStateChanged);
        }

        private void OnDisable()
        {
            EventBusManager.Unsubscribe<StateMachineStateChangedEvent>(OnStateMachineStateChanged);
        }

        private void OnStateMachineStateChanged(StateMachineStateChangedEvent @event)
        {
            if (@event.StateName == nameof(GameplayState))
                Init();
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

            var isSpinningEventResult = EventBusManager.RaiseWithResult<GetSpinningStatusEvent, bool>(new GetSpinningStatusEvent());
            isSpinning = isSpinningEventResult;

            if (isSpinning)
            {
                LoggerUtil.Log("Can't spin while spinning!");
                return;
            }
            
            EventBusManager.RaiseWithResult<SpinPressedEvent, UniTask>(new SpinPressedEvent());
        }
    }
}
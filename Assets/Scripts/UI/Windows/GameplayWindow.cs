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

        private EventBinding<StateMachineStateChangedEvent> _stateChangedEventBinding;

        private void OnEnable()
        {
            _stateChangedEventBinding = new EventBinding<StateMachineStateChangedEvent>(OnStateMachineStateChanged);
            EventBus<StateMachineStateChangedEvent>.Register(_stateChangedEventBinding);
        }

        private void OnDisable()
        {
            EventBus<StateMachineStateChangedEvent>.Deregister(_stateChangedEventBinding);
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

            var isSpinningEventResult = EventBus<GetSpinningStatusEvent, bool>.Raise(new GetSpinningStatusEvent());
            if (isSpinningEventResult != null)
                isSpinning = isSpinningEventResult[0];

            if (isSpinning)
            {
                LoggerUtil.Log("Can't spin while spinning!");
                return;
            }

            EventBus<SpinPressedEvent, UniTask>.Raise(new SpinPressedEvent());
        }
    }
}
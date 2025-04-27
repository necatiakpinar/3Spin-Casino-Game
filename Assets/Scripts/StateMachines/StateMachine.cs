using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBus;
using EventBus.Events;
using Interfaces;

namespace StateMachines
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private IState _currentState;
        private readonly ILogger _logger;
        
        public StateMachine(ILogger logger)
        {
            _logger = logger;
        }
        
        public void AddState<T>(T state) where T : IState
        {
            var type = typeof(T);
            if (_states.ContainsKey(type))
            {
                _logger.LogWarning($"{type.Name} already added to StateMachine.");
                return;
            }

            state.SetChangeStateAction(ChangeStateInternal);
            _states.Add(type, state);
        }

        public async UniTask ChangeState<T>() where T : IState
        {
            await ChangeStateInternal(typeof(T));
        }

        private async UniTask ChangeStateInternal(Type targetType)
        {
            if (!_states.TryGetValue(targetType, out var newState))
                _logger.LogError($"State {targetType.Name} is not added to StateMachine.");

            if (_currentState != null)
                await _currentState.Exit();

            _currentState = newState;

            if (_currentState != null)
                await _currentState.Enter();
            
            EventBus<StateMachineStateChangedEvent>.Raise(new StateMachineStateChangedEvent(targetType.Name));
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
}
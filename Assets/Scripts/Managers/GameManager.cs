using Addressables;
using Data.ScriptableObjects.Properties;
using Factories;
using Interfaces;
using Loggers;
using StateMachines.States;
using UnityEngine;
using UnityObjects;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform _gridParent;
        private GridPropertiesDataSo _gridProperties;
        private StateMachines.StateMachine _stateMachine;

        private IState _startState;
        private IState _gameplayState;
        private readonly UnityLogger _logger = new UnityLogger();

        private async void Start()
        {
            _gridProperties = await AddressableLoader.LoadAssetAsync<GridPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_GridPropertiesData));
            var objectFactory = new UnityObjectFactory(_logger);
            var transformProvider = new UnityTransform(_gridParent);

            _stateMachine = new StateMachines.StateMachine(_logger);
            _startState = new StartingState(objectFactory, transformProvider, _gridProperties, _logger);
            _gameplayState = new GameplayState(_logger);
            _stateMachine.AddState(_startState);
            _stateMachine.AddState(_gameplayState);

            await _stateMachine.ChangeState<StartingState>();
        }
    }
}
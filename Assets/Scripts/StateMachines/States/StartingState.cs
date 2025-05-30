﻿using Controllers;
using Cysharp.Threading.Tasks;
using Interfaces;
using System;

namespace StateMachines.States
{
    public class StartingState : IState
    {
        private GridController _gridController;
        public Func<Type, IStateParameters, UniTask> ChangeState { get; set; }

        private readonly IObjectFactory _objectFactory;
        private readonly ITransform _gridTransform;
        private readonly IGridProperties _gridProperties;
        private readonly ILogger _logger;
        private readonly int _initialWaitDuration = 500;

        public StartingState(IObjectFactory objectFactory, ITransform transformProvider, IGridProperties gridProperties, ILogger logger)
        {
            _objectFactory = objectFactory;
            _gridTransform = transformProvider;
            _gridProperties = gridProperties;
            _logger = logger;
        }

        public async UniTask Enter(IStateParameters parameters = null)
        {
            _gridController = new GridController(_gridProperties, _objectFactory, _gridTransform);
            _logger.Log("StartingState.Enter");
            await UniTask.Delay(_initialWaitDuration);
            await ChangeState.Invoke(typeof(GameplayState), null);
        }

        public void AddEventBindings()
        {

        }

        public void RemoveEventBindings()
        {
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using Interfaces;
using ILogger = Interfaces.ILogger;

namespace StateMachines.States
{
    public class GameplayState : IState
    {
        private Func<Type, UniTask> _changeState;
        private readonly ILogger _logger;

        public GameplayState(ILogger logger)
        {
            _logger = logger;
        }
        
        public async UniTask Enter()
        {
            _logger.Log("GameplayState Entered");
            await UniTask.CompletedTask;
        }
        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
        public void Update()
        {
        }

        public void SetChangeStateAction(Func<Type, UniTask> changeStateAction)
        {
            _changeState = changeStateAction;
        }
    }
}
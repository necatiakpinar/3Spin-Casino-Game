using System;
using Cysharp.Threading.Tasks;
using Interfaces;
using ILogger = Interfaces.ILogger;

namespace StateMachines.States
{
    public class GameplayState : IState
    {
        public Func<Type, IStateParameters, UniTask> ChangeState { get; set; }

        private readonly ILogger _logger;

        public GameplayState(ILogger logger)
        {
            _logger = logger;
        }

        public async UniTask Enter(IStateParameters parameters = null)
        {
            _logger.Log("GameplayState Entered");
            await UniTask.CompletedTask;
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
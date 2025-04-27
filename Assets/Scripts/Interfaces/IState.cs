using System;
using Cysharp.Threading.Tasks;

namespace Interfaces
{
    public interface IState
    {
        UniTask Enter();
        UniTask Exit();
        void Update();
        void SetChangeStateAction(Func<Type, UniTask> changeStateAction);
    }
}
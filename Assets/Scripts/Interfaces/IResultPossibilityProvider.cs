using System.Collections.Generic;
using Data.ScriptableObjects;

namespace Interfaces
{
    public interface IResultPossibilityProvider
    {
        List<ResultPossibility> GetResultPossibilities();
    }
}
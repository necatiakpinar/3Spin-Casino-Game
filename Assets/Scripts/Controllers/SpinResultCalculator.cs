using System.Collections.Generic;
using Data;
using Data.ScriptableObjects;
using Enums;
using Miscs;

namespace Controllers
{
    public class SpinResultCalculator
    {
        private List<SpinResultHolder> _spinResultHolders = new List<SpinResultHolder>();
        private readonly ResultPossibilitiesDataSo _resultPossibilitiesData;

        public SpinResultCalculator(ResultPossibilitiesDataSo resultPossibilitiesData)
        {
            _resultPossibilitiesData = resultPossibilitiesData;
        }

        public SerializableDictionary<int, List<SlotObjectType>> Calculate()
        {
            SerializableDictionary<int, List<SlotObjectType>> resultDictionary = new SerializableDictionary<int, List<SlotObjectType>>();

            ClearResults();
            CalculateIntervalsForEachResult();

            var resultIntervals = PrepareResultIntervals();
            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var selectedIntervalIndex = resultIntervals[result.ResultObjects][j];
                    var resultInterval = result.Intervals[j];
                    resultInterval.SetSelected(selectedIntervalIndex);
                    var isExist = resultDictionary.ContainsKey(selectedIntervalIndex);
                    if (!isExist)
                        resultDictionary.Add(selectedIntervalIndex, result.ResultObjects);
                }
            }

            return resultDictionary;
        }
        private static void ClearResults()
        {

            Player.GameplayData.Results.Clear();
            Player.GameplayData.ResultDictionary.Clear();
            Player.SaveDataToDisk();
        }

        private void CalculateIntervalsForEachResult()
        {
            SortResultPossibilities(_resultPossibilitiesData.ResultPossibilities);

            for (int i = 0; i < _resultPossibilitiesData.ResultPossibilities.Count; i++)
            {
                var targetPossibilityData = _resultPossibilitiesData.ResultPossibilities[i];
                var slotIntervals = CalculateAndGetIntervals(Player.GameplayData.TotalSpinRatio, targetPossibilityData.Possibility);
                var resultData = new ResultData(targetPossibilityData.TargetTypes, slotIntervals, targetPossibilityData.Possibility);
                Player.GameplayData.Results.Add(resultData);
            }
        }

        public void SortResultPossibilities(List<ResultPossibility> _resultPossibilities)
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i < _resultPossibilities.Count - 1; i++)
                {
                    if (_resultPossibilities[i].Possibility < _resultPossibilities[i + 1].Possibility)
                    {
                        var temp = _resultPossibilities[i];
                        _resultPossibilities[i] = _resultPossibilities[i + 1];
                        _resultPossibilities[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);
        }

        public List<ResultInterval> CalculateAndGetIntervals(int totalSpins, int numberOfIntervals)
        {
            var slotObjectIntervals = new List<ResultInterval>();
            int intervalSize = totalSpins / numberOfIntervals;
            int remainingSpins = totalSpins % numberOfIntervals;

            int start = 0;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                int intervalSizeWithExtra = intervalSize;
                if (i < remainingSpins)
                    intervalSizeWithExtra += 1;

                int end = start + intervalSizeWithExtra - 1;
                if (i == numberOfIntervals - 1)
                    end = totalSpins - 1;

                var interval = new ResultInterval(start, end);
                slotObjectIntervals.Add(interval);
                start = end + 1;
            }

            return slotObjectIntervals;
        }

        private Dictionary<List<SlotObjectType>, List<int>> PrepareResultIntervals()
        {
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = new Dictionary<List<SlotObjectType>, List<int>>();

            _spinResultHolders = new List<SpinResultHolder>();
            for (int i = 0; i < Player.GameplayData.TotalSpinRatio; i++)
            {
                var resultData = GetMostAvailableResultData(i);
                var spinResultHolder = new SpinResultHolder(i, resultData);
                _spinResultHolders.Add(spinResultHolder);
            }

            RandomizeResultsInIntervals();

            for (int i = 0; i < _spinResultHolders.Count; i++)
            {
                var spinResultHolder = _spinResultHolders[i];
                if (spinResultHolder.Result != null)
                {
                    var result = spinResultHolder.Result;
                    if (resultIntervals.ContainsKey(result.ResultObjects))
                        resultIntervals[result.ResultObjects].Add(spinResultHolder.SpinIndex);
                    else
                        resultIntervals.Add(result.ResultObjects, new List<int> { spinResultHolder.SpinIndex });
                }
            }
            
            return resultIntervals;
        }

        private ResultData GetMostAvailableResultData(int resultHolderIndex)
        {
            var maxIntervalResultData = TryGetMaxIntervalResultData(resultHolderIndex);
            if (maxIntervalResultData != null)
                return maxIntervalResultData;

            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    if (interval.MinIndex <= resultHolderIndex && interval.MaxIndex >= resultHolderIndex && !interval.IsAddedIntoResultHolder)
                    {
                        interval.IsAddedIntoResultHolder = true;
                        return result;
                    }
                }
            }

            return null;
        }

        private ResultData TryGetMaxIntervalResultData(int resultHolderIndex)
        {
            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    if (interval.MaxIndex == resultHolderIndex && !interval.IsAddedIntoResultHolder)
                    {
                        interval.IsAddedIntoResultHolder = true;
                        return result;
                    }
                }
            }

            return null;
        }

        private void RandomizeResultsInIntervals()
        {
            for (int i = 0; i < _spinResultHolders.Count; i++)
            {
                var result = _spinResultHolders[i].Result;
                var resultIntervals = result.GetInterval(i);

                var targetSpinResultHolder = GetRandomSpinResultHolderInInterval(i, result, resultIntervals);
                if (targetSpinResultHolder != null)
                {
                    var tempResult = _spinResultHolders[i].Result;
                    _spinResultHolders[i].Result = targetSpinResultHolder.Result;
                    targetSpinResultHolder.Result = tempResult;
                }
            }
        }

        private SpinResultHolder GetRandomSpinResultHolderInInterval(int resultIndex, ResultData result, ResultInterval resultInterval)
        {
            var targetResultIndex = UnityEngine.Random.Range(resultInterval.MinIndex, resultInterval.MaxIndex + 1);
            var targetResult = _spinResultHolders[targetResultIndex].Result;
            while (targetResult == result)
            {
                targetResultIndex = UnityEngine.Random.Range(resultInterval.MinIndex, resultInterval.MaxIndex + 1);
                targetResult = _spinResultHolders[targetResultIndex].Result;
            }

            bool canSwap = CanResultsSwap(resultIndex, result, targetResultIndex, targetResult);

            if (canSwap)
                return _spinResultHolders[targetResultIndex];

            return null;
        }

        private bool CanResultsSwap(int resultIndex, ResultData result, int targetResultIndex, ResultData targetResult)
        {
            var targetResultIntervalInResult = targetResult.GetInterval(resultIndex);
            var resultIntervalInTargetResult = result.GetInterval(targetResultIndex);

            var targetResultCounter = 0;
            var targetResultCanSwap = false;

            for (int i = targetResultIntervalInResult.MinIndex; i <= targetResultIntervalInResult.MaxIndex; i++)
            {
                var spinResult = _spinResultHolders[i].Result;
                if (spinResult.ResultObjects == targetResult.ResultObjects && i == targetResultIndex)
                {
                    targetResultCounter++;
                    targetResultCanSwap = true;
                }
            }

            var resultCounter = 0;
            var resultCanSwap = false;

            for (int i = resultIntervalInTargetResult.MinIndex; i <= resultIntervalInTargetResult.MaxIndex; i++)
            {
                var result_t = _spinResultHolders[i].Result;
                if (result_t.ResultObjects == result.ResultObjects && i == resultIndex)
                {
                    resultCounter++;
                    resultCanSwap = true;
                }
            }

            if (targetResultCounter > 1 || resultCounter > 1)
                return false;

            return targetResultCanSwap && resultCanSwap;
        }

    }
}
using System.Collections.Generic;
using Data;
using Data.ScriptableObjects;
using Enums;
using Interfaces;
using Miscs;

namespace Controllers
{
    public class SpinResultCalculator
    {
        private List<SpinResultHolder> _spinResultHolders;
        
        private readonly List<ResultData> _resultDataList;
        private readonly int _totalSpinRatio;
        private readonly IResultPossibilityProvider _resultPossibilityProvider;

        public SpinResultCalculator(IResultPossibilityProvider resultPossibilityProvider, int totalSpinRatio = 100)
        {
            _spinResultHolders = new List<SpinResultHolder>();
            _resultDataList = new List<ResultData>();
            _totalSpinRatio = totalSpinRatio;
            _resultPossibilityProvider = resultPossibilityProvider;
        }

        public SerializableDictionary<int, List<SlotObjectType>> Calculate(out List<ResultData> resultDataList)
        {
            var resultDictionary = new SerializableDictionary<int, List<SlotObjectType>>();
            CalculateIntervalsForEachResult();
            resultDataList = _resultDataList;

            var resultIntervals = PrepareResultIntervals();
            for (int i = 0; i < _resultDataList.Count; i++)
            {
                var result = _resultDataList[i];
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

        private void CalculateIntervalsForEachResult()
        {
            SortResultPossibilities(_resultPossibilityProvider.GetResultPossibilities());

            for (int i = 0; i < _resultPossibilityProvider.GetResultPossibilities().Count; i++)
            {
                var targetPossibilityData = _resultPossibilityProvider.GetResultPossibilities()[i];
                var slotIntervals = CalculateAndGetIntervals(_totalSpinRatio, targetPossibilityData.Possibility);
                var resultData = new ResultData(targetPossibilityData.TargetTypes, slotIntervals, targetPossibilityData.Possibility);
                _resultDataList.Add(resultData);
            }
        }

        private void SortResultPossibilities(List<ResultPossibility> resultPossibilities)
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i < resultPossibilities.Count - 1; i++)
                {
                    if (resultPossibilities[i].Possibility < resultPossibilities[i + 1].Possibility)
                    {
                        (resultPossibilities[i], resultPossibilities[i + 1]) = (resultPossibilities[i + 1], resultPossibilities[i]);
                        swapped = true;
                    }
                }
            } while (swapped);
        }

        private List<ResultInterval> CalculateAndGetIntervals(int totalSpins, int numberOfIntervals)
        {
            var slotObjectIntervals = new List<ResultInterval>();
            var intervalSize = totalSpins / numberOfIntervals;
            var remainingSpins = totalSpins % numberOfIntervals;

            var start = 0;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                var intervalSizeWithExtra = intervalSize;
                if (i < remainingSpins)
                    intervalSizeWithExtra += 1;

                var end = start + intervalSizeWithExtra - 1;
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
            var resultIntervals = new Dictionary<List<SlotObjectType>, List<int>>();

            _spinResultHolders = new List<SpinResultHolder>();
            for (int i = 0; i < _totalSpinRatio; i++)
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

            for (int i = 0; i < _resultDataList.Count; i++)
            {
                var result = _resultDataList[i];
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
            for (int i = 0; i < _resultDataList.Count; i++)
            {
                var result = _resultDataList[i];
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
                    (_spinResultHolders[i].Result, targetSpinResultHolder.Result) = (targetSpinResultHolder.Result, _spinResultHolders[i].Result);
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

            var canSwap = CanResultsSwap(resultIndex, result, targetResultIndex, targetResult);

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
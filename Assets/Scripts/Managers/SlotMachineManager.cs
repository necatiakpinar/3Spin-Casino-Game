using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addressables;
using Controllers;
using Data;
using Data.ScriptableObjects;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Managers
{
    public class SlotMachineManager : MonoBehaviour
    {
        private Dictionary<int, List<TileMono>> _gridDictionary;
        private ResultPossibilitiesDataSo _resultPossibilitiesData;
        private List<SlotColumnController> _slotColumnControllers = new List<SlotColumnController>();
        private List<SpinResultHolder> _spinResultHolders = new List<SpinResultHolder>();

        private bool _isSpinning = false;

        public void OnEnable()
        {
            Action<object[]> onTilesCreated = (parameters) => OnTilesCreated((Dictionary<int, List<TileMono>>)parameters[0]);
            EventManager.Subscribe(ActionType.OnTilesCreated, onTilesCreated);
        }

        public void OnDestroy()
        {
            EventManager.Unsubscribe(ActionType.OnTilesCreated);
        }

        private void Awake()
        {
            Player.LoadSaveDataFromDisk();
        }

        public async void OnTilesCreated(Dictionary<int, List<TileMono>> gridDictionary)
        {
            _gridDictionary = gridDictionary;
            for (int i = 0; i < _gridDictionary.Count; i++)
            {
                var slotColumnController = new SlotColumnController(_gridDictionary[i]);
                _slotColumnControllers.Add(slotColumnController);
            }

            _resultPossibilitiesData = await AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));

            //if (Player.GameplayData.Results.Count == 0 && Player.GameplayData.CurrentSpinIndex < Player.GameplayData.TotalSpinRatio)
            CalculateSpinResults();
        }

        private void CalculateSpinResults()
        {
            // Clear previous results
            Player.GameplayData.Results.Clear();
            Player.GameplayData.ResultDictionary.Clear();
            Player.SaveDataToDisk();

            CalculateIntervalsForEachResult();

            // Prepare result intervals
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = PrepareResultIntervals();
            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var selectedIntervalIndex = resultIntervals[result.ResultObjects][j];
                    var resultInterval = result.Intervals[j];
                    resultInterval.SetSelected(selectedIntervalIndex);
                    var isExist = Player.GameplayData.ResultDictionary.ContainsKey(selectedIntervalIndex);
                    if (!isExist)
                        Player.GameplayData.ResultDictionary.Add(selectedIntervalIndex, result.ResultObjects);
                    else
                        Debug.LogError("Interval is already exist");
                }
            }

            Player.SaveDataToDisk();

            // //show result intervals
            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    Debug.Log($"Result {i}: {result.Name} {interval.MinIndex} {interval.MaxIndex} Selected: {interval.SelectedIntervalIndex}");
                }
            }
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
                else
                    Debug.LogError("No target result found");
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
                var result_t = _spinResultHolders[i].Result;
                if (result_t.ResultObjects == targetResult.ResultObjects && i == targetResultIndex)
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

        private Dictionary<List<SlotObjectType>, List<int>> PrepareResultIntervals()
        {
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = new Dictionary<List<SlotObjectType>, List<int>>();

            _spinResultHolders = new List<SpinResultHolder>();
            for (int i = 0; i < Player.GameplayData.TotalSpinRatio; i++)
            {
                var resultData = GetMostAvailableResultData(i);
                var spinResultHolder = new SpinResultHolder(i, resultData);
                _spinResultHolders.Add(spinResultHolder);
                //  Debug.LogError(resultIndexHolder.Index + " " + resultIndexHolder.Result.Name);
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

            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                List<int> resultIntervalIndexes = _spinResultHolders.Where(x => x.Result == result).Select(x => x.SpinIndex).ToList();
                Debug.LogError(resultIntervalIndexes.Count);
            }

            return resultIntervals;
        }

        private ResultData GetMostAvailableResultData(int resultHolderIndex)
        {
            var maxIntervalResultData = TryGetMaxIntervalResultData(resultHolderIndex);
            if (maxIntervalResultData != null)
                return maxIntervalResultData;

            Dictionary<ResultData, ResultInterval> eligibleResults = new Dictionary<ResultData, ResultInterval>();

            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    if (interval.MinIndex <= resultHolderIndex && interval.MaxIndex >= resultHolderIndex && !interval.IsAddedIntoResultHolder)
                    {
                        //eligibleResults.Add(result,interval);
                        interval.IsAddedIntoResultHolder = true;
                        return result;
                    }
                }
            }

            // if (eligibleResults.Count > 0)
            // {
            //     var eligibleResultsKeys = eligibleResults.Keys.ToList();
            //     eligibleResultsKeys.Sort((a, b) => a.Possibility.CompareTo(b.Possibility));
            //     System.Random rnd = new System.Random();
            //     var randomIndex = rnd.Next(0, eligibleResultsKeys.Count);
            //     eligibleResults[eligibleResultsKeys[randomIndex]].IsAddedIntoResultHolder = true;
            //     return eligibleResultsKeys[randomIndex];
            // }


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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_isSpinning)
                Spin();
        }

        /// <summary>
        /// Spin the slot machine
        /// </summary>
        private async void Spin()
        {
            var isExist = Player.GameplayData.ResultDictionary.ContainsKey(Player.GameplayData.CurrentSpinIndex);
            if (isExist)
            {
                var spinResult = Player.GameplayData.ResultDictionary[Player.GameplayData.CurrentSpinIndex];
                Debug.LogError($"{Player.GameplayData.CurrentSpinIndex} {spinResult[0]} {spinResult[1]} {spinResult[2]}");
                _isSpinning = true;
                for (int i = 0; i < _slotColumnControllers.Count; i++)
                {
                    if (i == _slotColumnControllers.Count - 1) // Last column
                        await _slotColumnControllers[i].Spin(spinResult[i]);
                    else
                    {
                        _slotColumnControllers[i].Spin(spinResult[i]);
                        await Task.Delay(50);
                    }
                }
            }

            _isSpinning = false;

            if (Player.GameplayData.CurrentSpinIndex < Player.GameplayData.TotalSpinRatio - 1) // Continue to next spin
                Player.GameplayData.CurrentSpinIndex++;
            else // Recalculate results if all spins are done
            {
                Player.GameplayData.CurrentSpinIndex = 0;
                CalculateSpinResults();
            }

            Player.SaveDataToDisk();
        }
    }
}
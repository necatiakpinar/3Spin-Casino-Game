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

        private bool _isSpinning = false;
        private int _maxCounter = 2000;

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

        /// <summary>
        /// Sorts the result possibilities by possibility in descending order
        /// </summary>
        /// <param name="_resultPossibilities"></param>
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

        /// <summary>
        ///  Calculate and set intervals for each result possibility
        /// </summary>
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
//                Debug.Log($"Result {i}: {result.ResultObjects[0]} {result.ResultObjects[1]} {result.ResultObjects[2]}");
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var intervalIndex = resultIntervals[result.ResultObjects][j];
                    var resultInterval = result.Intervals[j];
                    resultInterval.SetSelected(intervalIndex);
                    var isExist = Player.GameplayData.ResultDictionary.ContainsKey(intervalIndex);
                    if (!isExist)
                        Player.GameplayData.ResultDictionary.Add(intervalIndex, result.ResultObjects);
                    else
                        Debug.LogError("Interval is already exist");

                    //  Debug.Log($"Interval: {intervalIndex}");
                }
            }

            Player.SaveDataToDisk();

            // //show result intervals
            // for (int i = 0; i < GameplayData.Results.Count; i++)
            // {
            //     var result = GameplayData.Results[i];
            //     for (int j = 0; j < result.Intervals.Count; j++)
            //     {
            //         var interval = result.Intervals[j];
            //         Debug.Log($"Result {i}: {result.ResultObjects[0]} {result.ResultObjects[1]} {result.ResultObjects[2]} {interval.MinIndex} {interval.MaxIndex} Selected: {interval.SelectedIntervalIndex}");
            //     }
            // }
        }

        private void CalculateIntervalsForEachResult()
        {
            // Sort result possibilities by possibility
            SortResultPossibilities(_resultPossibilitiesData.ResultPossibilities);

            for (int i = 0; i < _resultPossibilitiesData.ResultPossibilities.Count; i++)
            {
                var targetPossibilityData = _resultPossibilitiesData.ResultPossibilities[i];
                var slotIntervals = CalculateAndGetIntervals(Player.GameplayData.TotalSpinRatio, targetPossibilityData.Possibility);
                var resultData = new ResultData(targetPossibilityData.TargetTypes, slotIntervals);
                Player.GameplayData.Results.Add(resultData);
            }
        }
        
        private List<ResultIndexHolder> _resultIndexHolders = new List<ResultIndexHolder>();
        private bool IsResultExistInInterval(string resultName, ResultInterval resultInterval, out int index)
        {
            for (int i = resultInterval.MinIndex; i < resultInterval.MaxIndex + 1; i++)
            {
                if (_resultIndexHolders[i].Result == resultName && !_resultIndexHolders[i].IsLocked)
                {
                    index = i;
                    return true;
                }
            }
            
            var randomAvailableIndex = UnityEngine.Random.Range(resultInterval.MinIndex, resultInterval.MaxIndex + 1);
            var currentCounter = 0;
            while (_resultIndexHolders[randomAvailableIndex].IsLocked && currentCounter < _maxCounter)
            {
                randomAvailableIndex = UnityEngine.Random.Range(resultInterval.MinIndex, resultInterval.MaxIndex + 1);
                currentCounter++;
            }
            
            if (currentCounter >= _maxCounter)
            {
                Debug.LogError("Counter is exceeded");
            }
            index = randomAvailableIndex;
            return false;
        }
        
        private Dictionary<List<SlotObjectType>, List<int>> PrepareResultIntervals()
        {
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = new Dictionary<List<SlotObjectType>, List<int>>();
            
            _resultIndexHolders = new List<ResultIndexHolder>();
            var currentIndex = 0;
            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var resultIndexHolder = new ResultIndexHolder(currentIndex, result.Name);
                    _resultIndexHolders.Add(resultIndexHolder);
                    currentIndex++;
                }
            }

            // for (int i = 0; i < _resultIndexHolders.Count; i++)
            // {
            //     Debug.LogError($"{_resultIndexHolders[i].Index} {_resultIndexHolders[i].Result}");
            // }
            
            // Check Interval Indexes
            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    var isResultExist = IsResultExistInInterval(result.Name, interval, out var index);
                    if (isResultExist)
                    {
                        _resultIndexHolders[index].SetResult(result.Name);
                    }
                    else
                    {
                        _resultIndexHolders[index].SetResult(result.Name);
                    }
                }
            }
            
            // // // // show result index holders
            // for (int i = 0; i < _resultIndexHolders.Count; i++)
            // {
            //     Debug.LogError($"{_resultIndexHolders[i].Index} {_resultIndexHolders[i].Result}");
            // }
            
            // for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            // {
            //     var result = Player.GameplayData.Results[i];
            //     List<int> resultIntervalIndexes = _resultIndexHolders.Where(x => x.Result == result.Name).Select(x => x.Index ).ToList();
            //     Debug.LogError(resultIntervalIndexes.Count);
            // }
            //
            for (int i = 0; i < _resultIndexHolders.Count; i++)
            {
                var resultIndexHolder = _resultIndexHolders[i];
                if (!resultIndexHolder.IsLocked)
                    Debug.LogError($"{resultIndexHolder.Index} {resultIndexHolder.Result}");
            }

            return resultIntervals;
        }
        
        /// <summary>
        /// Calculate and get intervals
        /// </summary>
        /// <param name="totalSpins"></param>
        /// <param name="numberOfIntervals"></param>
        /// <returns></returns>
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
                {
                    intervalSizeWithExtra += 1;
                }

                int end = start + intervalSizeWithExtra - 1;
                if (i == numberOfIntervals - 1)
                {
                    end = totalSpins - 1;
                }

                var interval = new ResultInterval(start, end);
                slotObjectIntervals.Add(interval);
                start = end + 1;
            }

            return slotObjectIntervals;
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
                //Debug.LogError($"{spinResult[0]} {spinResult[1]} {spinResult[2]}");
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
            
            if (Player.GameplayData.CurrentSpinIndex  < Player.GameplayData.TotalSpinRatio - 1) // Continue to next spin
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
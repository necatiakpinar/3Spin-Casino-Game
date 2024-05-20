using System;
using System.Collections.Generic;
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
        private int _maxCounter = 150;

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

            if (Player.GameplayData.Results.Count == 0 && Player.GameplayData.CurrentSpinIndex < Player.GameplayData.TotalSpinRatio)
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

        /// <summary>
        /// Prepare result intervals
        /// </summary>
        /// <returns></returns>
        private Dictionary<List<SlotObjectType>, List<int>> PrepareResultIntervals()
        {
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = new Dictionary<List<SlotObjectType>, List<int>>();
            List<int> assignedIntervalIndexes = new List<int>();
            List<int> allIndexes = new List<int>();

            for (int i = 0; i < Player.GameplayData.TotalSpinRatio; i++)
            {
                allIndexes.Add(i);
            }

            for (int i = 0; i < Player.GameplayData.Results.Count; i++)
            {
                var result = Player.GameplayData.Results[i];
                List<int> intervalQueue = new List<int>();
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    var randomIndex = UnityEngine.Random.Range(interval.MinIndex, interval.MaxIndex + 1);
                    var currentCounter = 0;
                    while (assignedIntervalIndexes.Contains(randomIndex) && currentCounter < _maxCounter)
                    {
                        randomIndex = UnityEngine.Random.Range(interval.MinIndex, interval.MaxIndex + 1);
                        currentCounter++;
                    }

                    if (currentCounter == _maxCounter)
                    {
//                        Debug.LogError($"GOTCHA! {result.ResultObjects[0]} {result.ResultObjects[1]} {result.ResultObjects[2]} {interval.MinIndex} {interval.MaxIndex}");
                        var randomIndexIndex = UnityEngine.Random.Range(0, allIndexes.Count);
                        randomIndex = allIndexes[randomIndexIndex];
                        //                      Debug.LogError($"Random Index: {randomIndex}");
                    }

                    intervalQueue.Add(randomIndex);
                    assignedIntervalIndexes.Add(randomIndex);
                    allIndexes.Remove(randomIndex);
                }

                resultIntervals.Add(result.ResultObjects, intervalQueue);
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
            int intervalSize = totalSpins / numberOfIntervals; // Her interval için temel boyut
            int remainingSpins = totalSpins % numberOfIntervals; // Dağıtılamayan kalan spin sayısı

            int start = 0;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                int intervalSizeWithExtra = intervalSize;
                if (i < remainingSpins)
                {
                    intervalSizeWithExtra += 1; // Ekstra spinleri eşit olarak ilk 'remainingSpins' sayıda aralığa dağıt
                }

                int end = start + intervalSizeWithExtra - 1;
                if (i == numberOfIntervals - 1)
                {
                    // Son aralık için kalan tüm spinleri ekle
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
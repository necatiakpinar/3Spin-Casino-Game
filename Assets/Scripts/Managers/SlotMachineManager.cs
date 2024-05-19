using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addressables;
using Data;
using Data.ScriptableObjects;
using DefaultNamespace;
using DefaultNamespace.Controllers;
using Enums;
using UnityEngine;
using UnityEngine.U2D;

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

        public async void OnTilesCreated(Dictionary<int, List<TileMono>> gridDictionary)
        {
            _gridDictionary = gridDictionary;
            for (int i = 0; i < _gridDictionary.Count; i++)
            {
                var slotColumnController = new SlotColumnController(_gridDictionary[i]);
                _slotColumnControllers.Add(slotColumnController);
            }

            _resultPossibilitiesData = await AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));
            CalculateSpinResults();
        }

        private void CalculateSpinResults()
        {
            // Calculate and set intervals for each result possibility
            for (int i = 0; i < _resultPossibilitiesData.ResultPossibilities.Count; i++)
            {
                var targetPossibilityData = _resultPossibilitiesData.ResultPossibilities[i];
                var slotIntervals = CalculateAndGetIntervals(GameplayData.TotalSpinRatio, targetPossibilityData.Possibility);
                var resultData = new ResultData(targetPossibilityData.TargetTypes, slotIntervals);
                GameplayData.Results.Add(resultData);
            }

            // Prepare result intervals
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = PrepareResultIntervals();
            for (int i = 0; i < GameplayData.Results.Count; i++)
            {
                var result = GameplayData.Results[i];
//                Debug.Log($"Result {i}: {result.ResultObjects[0]} {result.ResultObjects[1]} {result.ResultObjects[2]}");
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var intervalIndex = resultIntervals[result.ResultObjects][j];
                    var resultInterval = result.Intervals[j];
                    resultInterval.SetSelected(intervalIndex);
                    var isExist = GameplayData.ResultDictionary.ContainsKey(intervalIndex);
                    if (!isExist)
                        GameplayData.ResultDictionary.Add(intervalIndex, result);
                    else
                        Debug.LogError("Interval is already exist");

                      //  Debug.Log($"Interval: {intervalIndex}");
                }
            }
            
            //show result intervals
            for (int i = 0; i < GameplayData.Results.Count; i++)
            {
                var result = GameplayData.Results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    Debug.Log($"Result {i}: {result.ResultObjects[0]} {result.ResultObjects[1]} {result.ResultObjects[2]} {interval.MinIndex} {interval.MaxIndex} Selected: {interval.SelectedIntervalIndex}");
                }
            }
            
        }
        
        private Dictionary<List<SlotObjectType>, List<int>> PrepareResultIntervals()
        {
            Dictionary<List<SlotObjectType>, List<int>> resultIntervals = new Dictionary<List<SlotObjectType>, List<int>>();
            List<int> assignedIntervalIndexes = new List<int>();
            List<int> allIndexes = new List<int>();
            
            for (int i = 0; i < GameplayData.TotalSpinRatio; i++)
            {
                allIndexes.Add(i);
            }

            for (int i = 0; i < GameplayData.Results.Count; i++)
            {
                var result = GameplayData.Results[i];
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

        public List<ResultInterval> CalculateAndGetIntervals(int totalSpins, int numberOfIntervals)
        {
            var slotObjectIntervals = new List<ResultInterval>();
            int intervalSize = totalSpins / numberOfIntervals;  // Her interval için temel boyut
            int remainingSpins = totalSpins % numberOfIntervals;  // Dağıtılamayan kalan spin sayısı

            int start = 0;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                int intervalSizeWithExtra = intervalSize;
                if (i < remainingSpins) {
                    intervalSizeWithExtra += 1;  // Ekstra spinleri eşit olarak ilk 'remainingSpins' sayıda aralığa dağıt
                }

                int end = start + intervalSizeWithExtra - 1;
                if (i == numberOfIntervals - 1) {  // Son aralık için kalan tüm spinleri ekle
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
        
        private async void Spin()
        {
            var isExist = GameplayData.ResultDictionary.ContainsKey(GameplayData.CurrentSpinIndex);
            if (isExist)
            {
                var spinResult = GameplayData.ResultDictionary[GameplayData.CurrentSpinIndex].ResultObjects;
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
            GameplayData.CurrentSpinIndex++;
        }
    }
} 
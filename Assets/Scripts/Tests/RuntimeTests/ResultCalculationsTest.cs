using System.Collections;
using System.Collections.Generic;
using Addressables;
using Controllers;
using Data;
using Data.ScriptableObjects;
using Enums;
using Miscs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.RuntimeTests
{
    public class ResultCalculationsTest
    {
        [UnityTest]
        public IEnumerator CheckResultsHaveTotalSpinRatio()
        {
            Player.LoadSaveDataFromDisk();
            var loadTask =
                AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(
                    AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));
            
            while (!loadTask.IsCompleted)
                yield return null;

            Assert.IsNotNull(loadTask.Result);

            var results = loadTask.Result;
            int totalSpinRatio = 0;

            foreach (var resultPossibility in results.ResultPossibilities)
            {
                totalSpinRatio += resultPossibility.Possibility;
                Debug.Log($" {resultPossibility.Name} {resultPossibility.Possibility}");
            }

            Assert.AreEqual(Player.GameplayData.TotalSpinRatio, totalSpinRatio,
                "The total spin ratio should be equal to the sum of all the spin ratios in the result dictionary.");
        }
        
        [UnityTest]
        public IEnumerator CheckSpinResults()
        {
            Player.LoadSaveDataFromDisk();
            var loadTask =
                AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(
                    AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));
            while (!loadTask.IsCompleted)
                yield return null;

            Assert.IsNotNull(loadTask.Result);

            var spinResultCalculator = new SpinResultCalculator(loadTask.Result);
            spinResultCalculator.Calculate();
            var resultDictionary = Player.GameplayData.ResultDictionary;
            Assert.AreEqual(100, resultDictionary.pairs.Count,
                "The count of pairs should be exactly 100 for the test to pass.");
        }

        
    }
}
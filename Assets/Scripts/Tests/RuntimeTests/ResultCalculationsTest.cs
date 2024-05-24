using System.Collections;
using Addressables;
using Controllers;
using Data;
using Data.ScriptableObjects;
using Managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.RuntimeTests
{
    public class ResultCalculationsTest
    {
        [Test]
        public void ResultCalculationsTestSimplePasses()
        {
        }
        
        [UnityTest]
        public IEnumerator CheckSpinResults()
        {
            Player.LoadSaveDataFromDisk();
            var loadTask = AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));
            while (!loadTask.IsCompleted)
                yield return null;

            Assert.IsNotNull(loadTask.Result);

            var spinResultCalculator = new SpinResultCalculator(loadTask.Result);
            spinResultCalculator.Calculate();
            var results = Player.GameplayData.ResultDictionary;
            Assert.AreEqual(100, results.pairs.Count, "The count of pairs should be exactly 100 for the test to pass.");
        }
    }
}
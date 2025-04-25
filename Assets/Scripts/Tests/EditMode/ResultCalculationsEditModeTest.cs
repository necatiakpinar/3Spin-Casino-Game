using System.Collections.Generic;
using Controllers;
using Data;
using Data.ScriptableObjects;
using NUnit.Framework;
using UnityEditor;

namespace Tests.EditMode
{
    [TestFixture]
    [Category("Logic Calculations")]
    public class ResultCalculationsEditModeTest
    {
        private ResultPossibilitiesDataSo _resultData;

        private const string _resultDataPath =
            "Assets/ScriptableObjects/Possibilities/SO_ResultPossibilitiesData.asset";

        [OneTimeSetUp]
        public void Setup()
        {
            _resultData = AssetDatabase.LoadAssetAtPath<ResultPossibilitiesDataSo>(_resultDataPath);
            Player.LoadSaveDataFromDisk();
        }

        [Test]
        public void TotalSpinRatio_ShouldMatchSumOfAllPossibilities()
        {
            Assert.IsNotNull(_resultData, "Result data should not be null");

            int totalSpinRatio = 0;
            foreach (var resultPossibility in _resultData.ResultPossibilities)
            {
                totalSpinRatio += resultPossibility.Possibility;
                TestContext.WriteLine($" {resultPossibility.Name} {resultPossibility.Possibility}");
            }

            Assert.AreEqual(Player.GameplayData.TotalSpinRatio,
                totalSpinRatio,
                "The total spin ratio should be equal to the sum of all the spin ratios in the result dictionary.");
        }

        [Test]
        public void SpinResults_ShouldHaveExactly100Pairs()
        {
            Assert.IsNotNull(_resultData, "Result data should not be null");

            var spinResultCalculator = new SpinResultCalculator(_resultData, Player.GameplayData.TotalSpinRatio);
            var resultDictionary = spinResultCalculator.Calculate(out var results);

            Assert.AreEqual(Player.GameplayData.TotalSpinRatio,
                resultDictionary.pairs.Count,
                $"The count of pairs should be exactly {Player.GameplayData.TotalSpinRatio} for the test to pass.");
        }

        [Test]
        public void SlotResults_ShouldHaveCorrectDistributionOnIntervals()
        {
            Assert.IsNotNull(_resultData, "Result data should not be null");

            var spinResultCalculator = new SpinResultCalculator(_resultData);
            var resultDictionary = spinResultCalculator.Calculate(out var results);

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];
                    TestContext.WriteLine($"Result {i}: {result.Name} {interval.MinIndex} {interval.MaxIndex} Selected: {interval.SelectedIntervalIndex}");
                }
            }

            Assert.AreEqual(Player.GameplayData.TotalSpinRatio,
                resultDictionary.pairs.Count,
                $"The count of pairs should be exactly {Player.GameplayData.TotalSpinRatio} for the test to pass.");
        }

        [Test]
        public void SlotResults_ShouldHaveCorrectTotalCreatedAmount()
        {
            Assert.IsNotNull(_resultData, "Result data should not be null");

            var spinResultCalculator = new SpinResultCalculator(_resultData);
            spinResultCalculator.Calculate(out var results);

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                TestContext.WriteLine($"Result {i}: {result.Name} Created: {result.Intervals.Count} Expected Amount {result.Possibility}");
                Assert.AreEqual(result.Intervals.Count,
                    result.Possibility,
                    "The created intervals should be equal to the possibility.");
            }
        }

        [Test]
        public void SlotResults_ShouldHaveUniqueIntervalIndexes()
        {
            Assert.IsNotNull(_resultData, "Result data should not be null");

            var spinResultCalculator = new SpinResultCalculator(_resultData);
            spinResultCalculator.Calculate(out var results);

            var usedIntervalIndexes = new Dictionary<int, string>();

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                for (int j = 0; j < result.Intervals.Count; j++)
                {
                    var interval = result.Intervals[j];

                    if (usedIntervalIndexes.TryGetValue(interval.SelectedIntervalIndex, out var index))
                    {
                        Assert.Fail(
                            $"Interval index {interval.SelectedIntervalIndex} is used by both '{index}' and '{result.Name}'. Each interval index should be unique.");
                    }

                    usedIntervalIndexes[interval.SelectedIntervalIndex] = result.Name;
                }
            }

            for (int i = 0; i < Player.GameplayData.TotalSpinRatio; i++)
            {
                Assert.IsTrue(usedIntervalIndexes.ContainsKey(i),
                    $"Interval index {i} is not used by any result. All indexes from 0 to {Player.GameplayData.TotalSpinRatio} should be used.");
            }

            Assert.AreEqual(Player.GameplayData.TotalSpinRatio,
                usedIntervalIndexes.Count,
                $"There should be exactly {Player.GameplayData.TotalSpinRatio} unique interval indexes (0-{Player.GameplayData.TotalSpinRatio}).");
        }
    }
}
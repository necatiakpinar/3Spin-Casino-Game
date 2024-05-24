 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Addressables;
using Controllers;
using Data;
using Data.ScriptableObjects;
using Data.ScriptableObjects.Properties;
using Enums;
using UnityEngine;
using Vfx;

namespace Managers
{
    public class SlotMachineManager : MonoBehaviour
    {
        private Dictionary<int, List<TileMono>> _gridDictionary;
        private ResultPossibilitiesDataSo _resultPossibilitiesData;
        private List<SlotColumnController> _slotColumnControllers = new List<SlotColumnController>();
        private SpinResultCalculator _spinResultCalculator;
        private bool _isSpinning = false;

        private SlotMachinePropertiesDataSo _properties;
        private SlotObjectCurrenciesDataSo _slotObjectCurrenciesDataSo;
        private SlotColumnPropertiesDataSo _slotColumnPropertiesDataSo;
        
        
        public void OnEnable()
        {
            Action<object[]> onTilesCreated = (parameters) => OnTilesCreated((Dictionary<int, List<TileMono>>)parameters[0]);
            EventManager.Subscribe(ActionType.OnTilesCreated, onTilesCreated);

            Action<object[]> onSpinPressed = (_) => Spin();
            EventManager.Subscribe(ActionType.OnSpinPressed, onSpinPressed);

            EventManager.Subscribe<bool>(FunctionType.CheckIsSpinning, (_) => _isSpinning);
        }

        public void OnDestroy()
        {
            EventManager.Unsubscribe(ActionType.OnTilesCreated);
            EventManager.Unsubscribe(ActionType.OnSpinPressed);

            EventManager.Unsubscribe(FunctionType.CheckIsSpinning);
        }

        private void Awake()
        {
            Player.LoadSaveDataFromDisk();
        }

        private async Task FetchData()
        {
            _properties = await AddressableLoader.LoadAssetAsync<SlotMachinePropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotMachinePropertiesData));
            _slotObjectCurrenciesDataSo = await AddressableLoader.LoadAssetAsync<SlotObjectCurrenciesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotObjectCurrenciesData));
            _slotColumnPropertiesDataSo = await AddressableLoader.LoadAssetAsync<SlotColumnPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotColumnPropertiesData));
        }

        public async void OnTilesCreated(Dictionary<int, List<TileMono>> gridDictionary)
        {
            await FetchData();

            _gridDictionary = gridDictionary;
            for (int i = 0; i < _gridDictionary.Count; i++)
            {
                var slotColumnController = new SlotColumnController(_gridDictionary[i], _slotColumnPropertiesDataSo);
                _slotColumnControllers.Add(slotColumnController);
            }

            _resultPossibilitiesData = await AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));

            _spinResultCalculator = new SpinResultCalculator(_resultPossibilitiesData);
            if (Player.GameplayData.Results.Count == 0 && Player.GameplayData.CurrentSpinIndex < Player.GameplayData.TotalSpinRatio)
                _spinResultCalculator.Calculate();
        }

        private async void Spin()
        {
            var isExist = Player.GameplayData.ResultDictionary.ContainsKey(Player.GameplayData.CurrentSpinIndex);

            if (isExist)
            {
                var spinResult = Player.GameplayData.ResultDictionary[Player.GameplayData.CurrentSpinIndex];
                Debug.LogError($"{Player.GameplayData.CurrentSpinIndex} {spinResult[0]} {spinResult[1]} {spinResult[2]}");
                var isFirstTwoSame = spinResult[0] == spinResult[1];
                _isSpinning = true;
                for (int i = 0; i < _slotColumnControllers.Count; i++)
                {
                    if (i == _slotColumnControllers.Count - 1) // Last column
                    {
                        var randomStopType = UnityEngine.Random.Range(0, 2) == 0 ? SlotColumnStopType.Slow : SlotColumnStopType.Regular;
                        var stopType = isFirstTwoSame ? randomStopType : SlotColumnStopType.Fast;
                        _slotColumnControllers[i].Spin(spinResult[i], stopType);
                    }
                    else
                    {
                        _slotColumnControllers[i].Spin(spinResult[i], _properties.FirstTwoStopType);
                        await Task.Delay(_properties.SpinStartingDelay); 
                    }
                }

                await Task.Delay(_properties.SpinDuration);

                for (int i = 0; i < _slotColumnControllers.Count; i++)
                    await _slotColumnControllers[i].SlowDown();

                await CheckForWin(spinResult);
            }

            _isSpinning = false;
            UpdateData();
        }

        private async Task CheckForWin(List<SlotObjectType> spinResult)
        {
            var firstSlotObjectType = spinResult[0];
            var isWin = spinResult.All(x => x == firstSlotObjectType);
            if (isWin)
            {
                var vfx = VFXPoolManager.Instance.SpawnFromPool<ParticleVFX>(VFXType.Coins, Vector3.zero, Quaternion.identity, false);
                if (vfx)
                {
                    await vfx.Play();
                    var currencyMultiplier = _slotObjectCurrenciesDataSo.GetSlotObjectCurrencyMultipliers(CurrencyType.Coin, firstSlotObjectType);
                    var currency = Player.GameplayData.Currencies.FirstOrDefault(currency => currency.CurrencyType == CurrencyType.Coin);
                    if (currency != null)
                    {
                        currency.AddAmount(currencyMultiplier.Amount);
                        EventManager.Notify(ActionType.OnCurrencyUpdated, CurrencyType.Coin);
                    }

                    Player.SaveDataToDisk();
                }
            }
        }

        private void UpdateData()
        {
            if (Player.GameplayData.CurrentSpinIndex < Player.GameplayData.TotalSpinRatio - 1) // Continue to next spin
                Player.GameplayData.CurrentSpinIndex++;
            else // Recalculate results if all spins are done
            {
                Player.GameplayData.CurrentSpinIndex = 0;
                _spinResultCalculator.Calculate();
            }

            Player.SaveDataToDisk();
        }
    }
}
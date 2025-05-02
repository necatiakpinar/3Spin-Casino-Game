using System.Collections.Generic;
using Abstractions;
using Addressables;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using Data.ScriptableObjects;
using Data.ScriptableObjects.Properties;
using Enums;
using EventBus;
using EventBus.Events;
using Helpers;
using Interfaces;
using Loggers;
using Miscs;
using UnityEngine;
using ILogger = Interfaces.ILogger;
using Vector3 = UnityEngine.Vector3;

namespace Managers
{
    public class SlotMachineManager : MonoBehaviour
    {
        private Dictionary<int, List<ITile>> _gridDictionary;
        private ResultPossibilitiesDataSo _resultPossibilitiesData;
        private SpinResultCalculator _spinResultCalculator;
        private SlotMachinePropertiesDataSo _properties;
        private SlotObjectCurrenciesDataSo _slotObjectCurrenciesDataSo;
        private SlotColumnPropertiesDataSo _slotColumnPropertiesDataSo;
        private bool _isSpinning = false;

        private EventBinding<TilesCreatedEvent> _tilesCreatedEventBinding;
        private EventBinding<SpinPressedEvent, UniTask> _spinPressedEventBinding;
        private EventBinding<GetSpinningStatusEvent, bool> _getSpinningStatusEventBinding;

        private readonly List<SlotColumnController> _slotColumnControllers = new List<SlotColumnController>();

        private CryptoHelper _cryptoHelper;
        private JsonHelper _jsonHelper;
        private SaveSystemController<GameplayData> _gameplaySaveSystemController;
        private ILogger _logger;
        private PersistentDataController _persistentDataController;

        public void OnEnable()
        {
            _tilesCreatedEventBinding = new EventBinding<TilesCreatedEvent>(OnTilesCreated);
            EventBus<TilesCreatedEvent>.Register(_tilesCreatedEventBinding);

            _spinPressedEventBinding = new EventBinding<SpinPressedEvent, UniTask>(Spin);
            EventBus<SpinPressedEvent, UniTask>.Register(_spinPressedEventBinding);

            _getSpinningStatusEventBinding = new EventBinding<GetSpinningStatusEvent, bool>(GetSpinningStatus);
            EventBus<GetSpinningStatusEvent, bool>.Register(_getSpinningStatusEventBinding);
        }

        public void OnDestroy()
        {
            EventBus<TilesCreatedEvent>.Deregister(_tilesCreatedEventBinding);
            EventBus<SpinPressedEvent, UniTask>.Deregister(_spinPressedEventBinding);
            EventBus<GetSpinningStatusEvent, bool>.Deregister(_getSpinningStatusEventBinding);
        }

        private void Awake()
        {
            _logger = new UnityLogger();
            _cryptoHelper = new CryptoHelper();
            _jsonHelper = new JsonHelper();
            _gameplaySaveSystemController = new SaveSystemController<GameplayData>(Constants.GameplayDataPath, _jsonHelper, _cryptoHelper, _logger);
            _gameplaySaveSystemController.LoadSaveDataFromDisk();
            _persistentDataController = new PersistentDataController((GameplayData)_gameplaySaveSystemController.PersistentData);
        }

        private async UniTask FetchData()
        {
            _properties = await AddressableLoader.LoadAssetAsync<SlotMachinePropertiesDataSo>(
                AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotMachinePropertiesData));
            _slotObjectCurrenciesDataSo =
                await AddressableLoader.LoadAssetAsync<SlotObjectCurrenciesDataSo>(
                    AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotObjectCurrenciesData));
            _slotColumnPropertiesDataSo =
                await AddressableLoader.LoadAssetAsync<SlotColumnPropertiesDataSo>(
                    AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotColumnPropertiesData));
        }

        private async void OnTilesCreated(TilesCreatedEvent @event)
        {
            await FetchData();

            _gridDictionary = @event.GridDictionary;
            for (int i = 0; i < _gridDictionary.Count; i++)
            {
                var slotColumnController = new SlotColumnController(_gridDictionary[i], _slotColumnPropertiesDataSo);
                _slotColumnControllers.Add(slotColumnController);
            }

            _resultPossibilitiesData =
                await AddressableLoader.LoadAssetAsync<ResultPossibilitiesDataSo>(
                    AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_ResultPossibilitiesData));

            _spinResultCalculator = new SpinResultCalculator(_resultPossibilitiesData, _persistentDataController.GameplayData.TotalSpinRatio);
            if (_persistentDataController.GameplayData.Results.Count == 0 &&
                _persistentDataController.GameplayData.CurrentSpinIndex < _persistentDataController.GameplayData.TotalSpinRatio)
            {
                ClearResults();
                _persistentDataController.GameplayData.CurrentSpinIndex = 0;
                _persistentDataController.GameplayData.ResultDictionary =
                    _spinResultCalculator.Calculate(out _persistentDataController.GameplayData.Results);
                _gameplaySaveSystemController.SaveDataToDisk(null);
            }
        }

        private async UniTask Spin(SpinPressedEvent @event)
        {
            var isExist = _persistentDataController.GameplayData.ResultDictionary.ContainsKey(_persistentDataController.GameplayData.CurrentSpinIndex);

            if (isExist)
            {
                var spinResult = _persistentDataController.GameplayData.ResultDictionary[_persistentDataController.GameplayData.CurrentSpinIndex];
                LoggerUtil.Log($"{_persistentDataController.GameplayData.CurrentSpinIndex} {spinResult[0]} {spinResult[1]} {spinResult[2]}");
                var isFirstTwoSame = spinResult[0] == spinResult[1];
                _isSpinning = true;
                for (int i = 0; i < _slotColumnControllers.Count; i++)
                {
                    if (i == _slotColumnControllers.Count - 1)
                    {
                        var randomStopType = UnityEngine.Random.Range(0, 2) == 0 ? SlotColumnStopType.Slow : SlotColumnStopType.Regular;
                        var stopType = isFirstTwoSame ? randomStopType : SlotColumnStopType.Fast;
                        _slotColumnControllers[i].Spin(spinResult[i], stopType);
                    }
                    else
                    {
                        _slotColumnControllers[i].Spin(spinResult[i], _properties.FirstTwoStopType);
                        await UniTask.Delay(_properties.SpinStartingDelay);
                    }
                }

                await UniTask.Delay(_properties.SpinDuration);

                for (int i = 0; i < _slotColumnControllers.Count; i++)
                    await _slotColumnControllers[i].SlowDown();

                await CheckForWin(spinResult);
            }

            _isSpinning = false;
            UpdateData();
        }

        public bool GetSpinningStatus(GetSpinningStatusEvent @event)
        {
            return _isSpinning;
        }

        private async UniTask CheckForWin(List<SlotObjectType> spinResult)
        {
            var firstSlotObjectType = spinResult[0];
            var isWin = true;
            for (int i = 0; i < spinResult.Count; i++)
            {
                if (spinResult[i] != firstSlotObjectType)
                {
                    isWin = false;
                    break;
                }
            }

            if (isWin)
            {
                var spawnedVfxEvent =
                    EventBus<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>>.Raise(new SpawnFromObjectPoolEvent<VFXType>(VFXType.Coins,
                        Vector3.zero,
                        Quaternion.identity,
                        updatePositionAndRotation: false))[0];
                var spawnedVfx = await spawnedVfxEvent;
                if (spawnedVfx)
                {
                    await spawnedVfx.Play();
                    var currencyMultiplier = _slotObjectCurrenciesDataSo.GetSlotObjectCurrencyMultipliers(CurrencyType.Coin, firstSlotObjectType);

                    _persistentDataController.GameplayData.CurrencyDataController.IncreaseCurrency(CurrencyType.Coin, currencyMultiplier.Amount);
                    _gameplaySaveSystemController.SaveDataToDisk(null);
                }
            }
        }

        private void UpdateData()
        {
            if (_persistentDataController.GameplayData.CurrentSpinIndex < _persistentDataController.GameplayData.TotalSpinRatio - 1)
                _persistentDataController.GameplayData.CurrentSpinIndex++;
            else
            {
                _persistentDataController.GameplayData.CurrentSpinIndex = 0;
                ClearResults();
                _persistentDataController.GameplayData.ResultDictionary =
                    _spinResultCalculator.Calculate(out _persistentDataController.GameplayData.Results);
                _gameplaySaveSystemController.SaveDataToDisk(null);
            }

            _gameplaySaveSystemController.SaveDataToDisk(null);

        }

        private void ClearResults()
        {
            _persistentDataController.GameplayData.Results.Clear();
            _persistentDataController.GameplayData.ResultDictionary.Clear();
            _gameplaySaveSystemController.SaveDataToDisk(null);
        }
    }
}
using Abstractions;
using Cysharp.Threading.Tasks;
using Enums;
using EventBus;
using EventBus.Events;
using Helpers;
using Pools;
using UnityEngine;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private VfxPool _vfxPool;
        
        private void OnEnable()
        {
            EventBusManager.SubscribeWithResult<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>>(SpawnFromVfxPool);
            EventBusManager.Subscribe<ReturnToPoolEvent<VFXType, BaseVFX>>(ReturnToVfxPool);
        }

        private void OnDisable()
        {
            EventBusManager.UnsubscribeWithResult<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>>(SpawnFromVfxPool);
            EventBusManager.Unsubscribe<ReturnToPoolEvent<VFXType, BaseVFX>>(ReturnToVfxPool);
        }

        private async UniTask<BaseVFX> SpawnFromVfxPool(SpawnFromObjectPoolEvent<VFXType> @event)
        {
            var spawnedVfx = _vfxPool.SpawnFromPool(@event.ObjectType, @event.Position, @event.Rotation, @event.Parent, @event.UpdatePositionAndRotation);
            if (spawnedVfx == null)
            {
                LoggerUtil.LogError("Vfx pool is empty!");
                return null;
            }

            await spawnedVfx.Init();
            return spawnedVfx;
        }

        private void ReturnToVfxPool(ReturnToPoolEvent<VFXType, BaseVFX> @event)
        {
            if (@event.PoolObject != null)
            {
                _vfxPool.ReturnToPool(@event.ObjectType, @event.PoolObject);
            }
            else
            {
                LoggerUtil.LogError("VFX is null!");
            }
        }
    }
}
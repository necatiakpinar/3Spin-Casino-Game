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

        private EventBinding<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>> _spawnFromVfxPoolEventBinding;
        private EventBinding<ReturnToPoolEvent<VFXType, BaseVFX>> _returnToVfxPoolEventBinding;

        private void OnEnable()
        {
            _spawnFromVfxPoolEventBinding = new EventBinding<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>>(SpawnFromVfxPool);
            EventBus<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>>.Register(_spawnFromVfxPoolEventBinding);

            _returnToVfxPoolEventBinding = new EventBinding<ReturnToPoolEvent<VFXType, BaseVFX>>(ReturnToVfxPool);
            EventBus<ReturnToPoolEvent<VFXType, BaseVFX>>.Register(_returnToVfxPoolEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SpawnFromObjectPoolEvent<VFXType>, UniTask<BaseVFX>>.Deregister(_spawnFromVfxPoolEventBinding);
            EventBus<ReturnToPoolEvent<VFXType, BaseVFX>>.Deregister(_returnToVfxPoolEventBinding);
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
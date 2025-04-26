using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace EventBus.Events
{
    public struct SpawnFromObjectPoolEvent<T> : IEvent where T : Enum
    {
        public readonly T ObjectType;
        public Vector2 Position;
        public Quaternion Rotation;
        public readonly Transform Parent;
        public readonly bool UpdatePositionAndRotation;

        public SpawnFromObjectPoolEvent(T objectType, Vector2 position, Quaternion rotation = default, Transform parent = null, bool updatePositionAndRotation = true)
        {
            ObjectType = objectType;
            Position = position;
            Rotation = rotation;
            Parent = parent;
            UpdatePositionAndRotation = updatePositionAndRotation;
        }
    }

    public struct ReturnToPoolEvent<TK, T> : IEvent where T : MonoBehaviour, IPoolable<T> where TK : Enum
    {
        public readonly TK ObjectType;
        public readonly T PoolObject;

        public ReturnToPoolEvent(TK objectType, T poolObject)
        {
            ObjectType = objectType;
            PoolObject = poolObject;
        }
    }
}
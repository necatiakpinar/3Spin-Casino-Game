using System;
using Interfaces;
using UnityEngine;

namespace Miscs
{
    [Serializable]
    public class PoolObject<T,TK> where T: Enum  where TK: IPoolable<TK> 
    {
        [SerializeField] private T _objectType;
        [SerializeField] private TK _objectPf;
        [SerializeField] private int _size;
        
        public T ObjectType => _objectType;
        public TK ObjectPf => _objectPf;
        public int Size => _size;
    }
}
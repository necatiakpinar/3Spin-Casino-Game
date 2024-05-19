using System.Threading.Tasks;
using Abstractions;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class TileMono : MonoBehaviour
    {
        private Vector2Int _coordinates;
        private BaseSlotObjectMono _slotObjectMono;
        public Vector2Int Coordinates => _coordinates;
        public BaseSlotObjectMono SlotObjectMono => _slotObjectMono;
        
        public void Init(Vector2Int coordinates, BaseSlotObjectMono slotObjectMono)
        {
            _coordinates = coordinates;
            _slotObjectMono = slotObjectMono;
            slotObjectMono.transform.localPosition = Vector2.zero;
        }
        
        public void DropObjectToBottom(TileMono bottomTile, int speed)
        {
            _slotObjectMono.MoveToTile(bottomTile, speed);
        }

        public void SetSlotObject(BaseSlotObjectMono slotObjectMono)
        {
            _slotObjectMono = slotObjectMono;
        }
        
    }
}
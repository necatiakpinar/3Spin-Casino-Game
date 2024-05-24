using Addressables;
using Data.ScriptableObjects.Properties;
using DG.Tweening;
using Enums;
using UnityEngine;
using UnityEngine.U2D;

namespace Abstractions
{
    public abstract class BaseSlotObjectMono : MonoBehaviour
    {
        [SerializeField] private SlotObjectType _type;
        [SerializeField] private SlotObjectPropertiesDataSo _properties;
        
        private SpriteRenderer _spriteRenderer;
        private SpriteAtlas _normalSpriteAtlas;
        private SpriteAtlas _blurredSpriteAtlas;
        private Sprite _normalSprite;
        private Sprite _blurredSprite;
        private GridPropertiesDataSo _gridProperties;
        
        public SlotObjectType Type => _type;
        public SlotObjectPropertiesDataSo Properties => _properties;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private async void Start()
        {
            Init(); 
        }
        
        protected virtual async void Init()
        {
            _properties = await AddressableLoader.LoadAssetAsync<SlotObjectPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotObjectProperties));
            _normalSpriteAtlas = await AddressableLoader.LoadAssetAsync<SpriteAtlas>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SA_NormalSlotObjects));
            _blurredSpriteAtlas = await AddressableLoader.LoadAssetAsync<SpriteAtlas>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SA_BlurredSlotObjects));
            _gridProperties = await AddressableLoader.LoadAssetAsync<GridPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_GridPropertiesData));
            

            _normalSprite = _normalSpriteAtlas.GetSprite(_type.ToString());
            _blurredSprite = _blurredSpriteAtlas.GetSprite(_type + _properties.BlurredSuffix);

            SetSprite(false);
        }
        
        public void SetSprite(bool isBlurred)
        {
            _spriteRenderer.sprite = isBlurred ? _blurredSprite : _normalSprite;
        }

        public void MoveToTile(TileMono targetTile, int speed)
        {
            if (targetTile.Coordinates.y == _gridProperties.Height - 1)
                _spriteRenderer.enabled = false;
            else
                _spriteRenderer.enabled = true;

            var duration = speed / _properties._milliSeconds;

            transform.SetParent(targetTile.transform);
            transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.Linear).OnComplete(() => { targetTile.SetSlotObject(this); });
        }
    }
}
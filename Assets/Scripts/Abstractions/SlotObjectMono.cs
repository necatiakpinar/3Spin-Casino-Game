using Addressables;
using Cysharp.Threading.Tasks;
using Data.ScriptableObjects.Properties;
using DG.Tweening;
using Enums;
using Helpers;
using Interfaces;
using Loggers;
using UnityEngine;
using UnityEngine.U2D;
using UnityObjects;

namespace Abstractions
{
    public class SlotObjectMono : MonoBehaviour, ISlotObject
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

        public ITransform Transform => new UnityTransform(transform);

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private async void Start()
        {
            await Init();
        }

        private async UniTask Init()
        {
            await LoadAddressables();

            _normalSprite = _normalSpriteAtlas.GetSprite(_type.ToString());
            _blurredSprite = _blurredSpriteAtlas.GetSprite(_type + _properties.BlurredSuffix);
            SetSprite(false);
        }

        private async UniTask LoadAddressables()
        {
            _properties = await AddressableLoader.LoadAssetAsync<SlotObjectPropertiesDataSo>(
                AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_SlotObjectProperties));
            _normalSpriteAtlas = await AddressableLoader.LoadAssetAsync<SpriteAtlas>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SA_NormalSlotObjects));
            _blurredSpriteAtlas =
                await AddressableLoader.LoadAssetAsync<SpriteAtlas>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SA_BlurredSlotObjects));
            _gridProperties =
                await AddressableLoader.LoadAssetAsync<GridPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_GridPropertiesData));
        }

        public void SetSprite(bool isBlurred)
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            _spriteRenderer.sprite = isBlurred ? _blurredSprite : _normalSprite;
        }

        public async UniTask MoveToTile(ITile targetTile, int speed)
        {
            var targetTileMono = targetTile as TileMono;
            if (targetTileMono == null)
            {
                LoggerUtil.LogError("Target tile is not a TileMono!");
                return;
            }

            if (targetTileMono.Coordinates.y == _gridProperties.Height - 1)
                _spriteRenderer.enabled = false;
            else
                _spriteRenderer.enabled = true;

            var duration = speed / _properties.MilliSeconds;

            transform.SetParent(targetTileMono.transform);
            transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                targetTileMono.SetSlotObject(this);
            });

            await UniTask.CompletedTask;
        }
    }
}

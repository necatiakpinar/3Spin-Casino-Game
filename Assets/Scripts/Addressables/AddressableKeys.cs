namespace Addressables
{
    public static class AddressableKeys
    {
            public enum AssetKeys
            {
                //Sprite Atlases
                SA_BlurredSlotObjects,
                SA_NormalSlotObjects,
                
                //Scriptable Objects
                SO_ResultPossibilitiesData,
                SO_SlotObjectCurrenciesData,
                
                //Properties
                SO_SlotObjectProperties,
                SO_SlotColumnPropertiesData,
                SO_GridPropertiesData,
                
            }
            public static string GetKey(AssetKeys key)
            {
                return key.ToString();
            }
        }
}
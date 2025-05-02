using EventBus.Events;

namespace Interfaces
{
    public interface ISaveSystemController
    {
        IGameplayData PersistentData { get; }
        IGameplayData CachedPersistentData { get; }
        string DataFilePath { get; }
        ICryptoHelper CryptoHelper { get; }
        public void SaveDataToDisk(SaveDataEvent @event);
        void LoadSaveDataFromDisk();
        void AddEventBindings();
        void RemoveEventBindings();
    }
}
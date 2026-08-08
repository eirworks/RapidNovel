namespace RapidNovel.Models.Interfaces;

public interface IConfigService
{
    IModelConfig Config { get; set; }
    void Initialize();
    void StoreConfig();
    void LoadConfig();
}
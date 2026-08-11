using RapidNovel.Models.Configuration;

namespace RapidNovel.Models.Interfaces;

public interface IConfigService
{
    AppConfig AppConfig { get; set; }
    void Initialize();
    void StoreConfig();
    void LoadConfig();
}
using RapidNovel.Models.Enums;

namespace RapidNovel.Models.Configuration;

public record AiProviderConfig(
    string Name,
    AiProvider? Provider,
    string BaseUrl,
    string ApiKey
    );
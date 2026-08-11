using RapidNovel.Models.Enums;

namespace RapidNovel.Models.Configuration;

public record AiProviderConfig(
    AiProvider? Provider,
    string BaseUrl,
    string ApiKey
    );
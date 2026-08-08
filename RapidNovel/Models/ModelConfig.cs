using System.Collections.Generic;
using RapidNovel.Models.Enums;
using RapidNovel.Models.Interfaces;

namespace RapidNovel.Models;

public class ModelConfig: IModelConfig
{
    public ModelProvider Provider { get; set; } = ModelProvider.DeepSeek;
    public string Model { get; set; } = "deepseek-v4-flash";
    public List<ConfigModelKey> Keys { get; set; } = [];
}
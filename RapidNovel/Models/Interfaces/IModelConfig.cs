using System.Collections.Generic;
using RapidNovel.Models.Enums;

namespace RapidNovel.Models.Interfaces;

public interface IModelConfig
{
    ModelProvider Provider { get; set; }
    string Model { get; set; }
    List<ConfigModelKey> Keys { get; set; }
}
using System;
using System.Collections.Generic;

namespace RapidNovel.Models;

public record Character(
    string Id,
    string FirstName,
    string? LastName,
    bool IsFemale,
    DateTime? Birthday,
    string? Description,
    List<string> Aliases
    );
namespace NuanSystem.Application.Common.Models;

public enum FailureKind
{
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Forbidden = 4,
    Unavailable = 5,
    Unexpected = 6
}

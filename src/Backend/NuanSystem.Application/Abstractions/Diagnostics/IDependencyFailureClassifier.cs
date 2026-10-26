using NuanSystem.Application.Common.Exceptions;

namespace NuanSystem.Application.Abstractions.Diagnostics;

public interface IDependencyFailureClassifier
{
    DependencyFailureException? Classify(Exception exception);
}

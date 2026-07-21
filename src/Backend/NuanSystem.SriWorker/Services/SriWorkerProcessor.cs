using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.SriWorker.Options;

namespace NuanSystem.SriWorker.Services;

public sealed class SriWorkerProcessor(
    ISriWorkerCompanyRepository companyRepository,
    ISriWorkerQueueRepository queueRepository,
    ISriAuthorizationProvider provider,
    IOptionsMonitor<SriWorkerOptions> options,
    ILogger<SriWorkerProcessor> logger) : ISriWorkerProcessor
{
    public async Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default)
    {
        var current = options.CurrentValue;
        if (!current.Enabled) return 0;

        var processed = 0;
        foreach (var company in await companyRepository.GetEnabledCompaniesAsync(cancellationToken))
        {
            await queueRepository.ReleaseExpiredLeasesAsync(company.CompanyId, current.NormalizedWorkerInstance, current.MaxAttempts, cancellationToken);
            var jobs = await queueRepository.ClaimAsync(company.CompanyId, company.Environment, current.NormalizedWorkerInstance,
                Math.Clamp(current.BatchSize, 1, 100), Math.Clamp(current.LeaseSeconds, 30, 3600),
                Math.Clamp(current.MaxAttempts, 1, 20), cancellationToken);

            await Parallel.ForEachAsync(jobs, new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Clamp(current.MaxConcurrency, 1, 16),
                CancellationToken = cancellationToken
            }, async (job, token) => await ProcessJobAsync(company, job, current, token));
            processed += jobs.Count;
        }
        return processed;
    }

    private async Task ProcessJobAsync(SriWorkerCompanyDto company, SriClaimedDocumentDto job, SriWorkerOptions current, CancellationToken cancellationToken)
    {
        var maskedKey = Mask(job.AccessKey);
        var result = await provider.QueryAsync(job.Environment, job.AccessKey, cancellationToken);
        SriWorkerCompletionCode completion;

        if (result.Outcome == SriAuthorizationOutcome.Authorized)
        {
            completion = await queueRepository.CompleteAuthorizedAsync(company.CompanyId, new SriAuthorizedDocumentData(
                job.Id, current.NormalizedWorkerInstance, job.AttemptCount,
                result.AuthorizationNumber!, result.AuthorizationAt!.Value, result.ProviderEnvironment!, result.IssuerRuc!,
                result.DocumentTypeCode!, result.XmlContent!, result.Sha256!, "application/xml", result.RemoteCorrelationId), cancellationToken);
        }
        else
        {
            var outcome = ResolveOutcome(job, result, current);
            var nextAttempt = outcome == "Retry"
                ? DateTime.UtcNow.Add(SriRetrySchedule.Calculate(job.Id, job.AttemptCount, current))
                : (DateTime?)null;
            completion = await queueRepository.CompleteAttemptAsync(company.CompanyId, new SriAttemptCompletionData(
                job.Id, current.NormalizedWorkerInstance, job.AttemptCount, outcome,
                result.ErrorCategory, result.ErrorCode, result.ErrorMessage, result.RemoteCorrelationId, nextAttempt), cancellationToken);
        }

        if (completion != SriWorkerCompletionCode.Updated)
        {
            logger.LogWarning("Resultado SRI no persistido Company={CompanyCode} QueueId={QueueId} AccessKey={AccessKey} Completion={Completion}",
                company.CompanyCode, job.Id, maskedKey, completion);
            return;
        }

        logger.LogInformation("Consulta SRI procesada Company={CompanyCode} QueueId={QueueId} AccessKey={AccessKey} Outcome={Outcome} Attempt={Attempt}",
            company.CompanyCode, job.Id, maskedKey, result.Outcome, job.AttemptCount);
    }

    private static string ResolveOutcome(SriClaimedDocumentDto job, SriAuthorizationResult result, SriWorkerOptions options)
    {
        if (result.Outcome == SriAuthorizationOutcome.PermanentFailure) return "Failed";
        if (result.Outcome == SriAuthorizationOutcome.NotFound)
        {
            var windowExpired = DateTime.UtcNow - job.CreatedAt >= TimeSpan.FromMinutes(Math.Clamp(options.NotFoundWindowMinutes, 1, 1440));
            return job.AttemptCount >= Math.Clamp(options.NotFoundMaxAttempts, 1, options.MaxAttempts) || windowExpired ? "NotFound" : "Retry";
        }
        return "Retry";
    }

    internal static string Mask(string accessKey) => accessKey.Length < 12 ? "********" : $"{accessKey[..4]}*************************************{accessKey[^4..]}";
}

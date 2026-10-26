SET NOCOUNT ON;

IF COL_LENGTH(N'dbo.PurchaseOrderApprovals', N'UserId') IS NULL
BEGIN
    ALTER TABLE dbo.PurchaseOrderApprovals ADD UserId int NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_PurchaseOrderApprovals_OpenLevel'
      AND object_id = OBJECT_ID(N'dbo.PurchaseOrderApprovals')
)
BEGIN
    CREATE UNIQUE INDEX UX_PurchaseOrderApprovals_OpenLevel
        ON dbo.PurchaseOrderApprovals (PurchaseOrderId, ApprovalLevel)
        WHERE Status = N'Pending';
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_PURCHASEORDERS_WORKFLOW
    @Id int,
    @ExpectedStatusesJson nvarchar(max),
    @NextStatus nvarchar(40),
    @Action nvarchar(40),
    @ApprovalLevel int,
    @Observation nvarchar(1000) = NULL,
    @UserId int,
    @UserName nvarchar(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ApprovalLevel <> 1
       OR ISJSON(@ExpectedStatusesJson) <> 1
       OR @UserId <= 0
       OR (@Action = N'RequestApproval' AND @NextStatus <> N'PendingApproval')
       OR (@Action = N'Approve' AND @NextStatus <> N'Approved')
       OR (@Action = N'Reject' AND @NextStatus <> N'Rejected')
       OR @Action NOT IN (N'RequestApproval', N'Approve', N'Reject')
    BEGIN
        SELECT N'Conflict' AS Outcome;
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @CurrentStatus nvarchar(40);
        SELECT @CurrentStatus = h.Status
        FROM dbo.PurchaseOrderHeaders AS h WITH (UPDLOCK, HOLDLOCK)
        WHERE h.Id = @Id
          AND h.IsDeleted = 0;

        IF @CurrentStatus IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT N'NotFound' AS Outcome;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 1
            FROM OPENJSON(@ExpectedStatusesJson)
            WHERE [value] = @CurrentStatus
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT N'Conflict' AS Outcome;
            RETURN;
        END;

        IF @Action = N'RequestApproval'
        BEGIN
            IF EXISTS
            (
                SELECT 1
                FROM dbo.PurchaseOrderApprovals WITH (UPDLOCK, HOLDLOCK)
                WHERE PurchaseOrderId = @Id
                  AND ApprovalLevel = @ApprovalLevel
                  AND Status = N'Pending'
            )
            BEGIN
                ROLLBACK TRANSACTION;
                SELECT N'Conflict' AS Outcome;
                RETURN;
            END;

            INSERT INTO dbo.PurchaseOrderApprovals
                (PurchaseOrderId, ApprovalLevel, UserId, UserName, Status, Observation)
            VALUES
                (@Id, @ApprovalLevel, @UserId, @UserName, N'Pending', @Observation);
        END;
        ELSE
        BEGIN
            UPDATE dbo.PurchaseOrderApprovals
            SET UserId = @UserId,
                UserName = @UserName,
                RespondedAt = SYSUTCDATETIME(),
                Status = CASE WHEN @Action = N'Approve' THEN N'Approved' ELSE N'Rejected' END,
                Observation = @Observation
            WHERE PurchaseOrderId = @Id
              AND ApprovalLevel = @ApprovalLevel
              AND Status = N'Pending';

            IF @@ROWCOUNT <> 1
            BEGIN
                ROLLBACK TRANSACTION;
                SELECT N'Conflict' AS Outcome;
                RETURN;
            END;
        END;

        UPDATE dbo.PurchaseOrderHeaders
        SET Status = @NextStatus,
            UpdatedByUserId = @UserId,
            UpdatedByUserName = @UserName,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id
          AND IsDeleted = 0
          AND Status = @CurrentStatus;

        IF @@ROWCOUNT <> 1
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT N'Conflict' AS Outcome;
            RETURN;
        END;

        COMMIT TRANSACTION;

        SELECT N'Updated' AS Outcome;
        EXEC dbo.SP_NA_GET_PURCHASEORDERS_BUSCARPORID @Id = @Id;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

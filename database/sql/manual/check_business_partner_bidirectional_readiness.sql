/*
    Diagnostico previo de BusinessPartner para un tenant central o sucursal.
    Cada conjunto de resultados identifica una condicion que requiere revision.
*/
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.BusinessPartners', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartners', N'GlobalId') IS NULL
        EXEC(N'SELECT N''MissingBusinessPartnerGlobalId'' AS Finding, bp.Id, bp.Code FROM dbo.BusinessPartners AS bp ORDER BY bp.Id;');
    ELSE
        EXEC(N'SELECT N''MissingBusinessPartnerGlobalId'' AS Finding, bp.Id, bp.Code FROM dbo.BusinessPartners AS bp WHERE bp.GlobalId IS NULL ORDER BY bp.Id;');

    EXEC(N'
        SELECT N''DuplicateBusinessPartnerCode'' AS Finding, bp.Code, COUNT_BIG(1) AS FindingCount
        FROM dbo.BusinessPartners AS bp
        GROUP BY bp.Code
        HAVING COUNT_BIG(1) > 1
        ORDER BY bp.Code;

        SELECT N''BusinessPartnerCodeTooLong'' AS Finding, bp.Id, bp.Code, LEN(bp.Code) AS CodeLength
        FROM dbo.BusinessPartners AS bp
        WHERE LEN(bp.Code) > 50
        ORDER BY bp.Id;

        SELECT N''DuplicateNormalizedIdentificationByRole'' AS Finding,
               bp.PartnerType,
               bp.IdentificationTypeId,
               normalized.NormalizedIdentificationNumber,
               COUNT_BIG(1) AS FindingCount
        FROM dbo.BusinessPartners AS bp
        CROSS APPLY
        (
            SELECT REPLACE(
                TRANSLATE(
                    UPPER(LTRIM(RTRIM(bp.IdentificationNumber)) COLLATE Latin1_General_100_BIN2),
                    N''.-'' + NCHAR(9) + NCHAR(10) + NCHAR(11) + NCHAR(12) + NCHAR(13)
                        + NCHAR(32) + NCHAR(133) + NCHAR(160) + NCHAR(5760)
                        + NCHAR(8192) + NCHAR(8193) + NCHAR(8194) + NCHAR(8195)
                        + NCHAR(8196) + NCHAR(8197) + NCHAR(8198) + NCHAR(8199)
                        + NCHAR(8200) + NCHAR(8201) + NCHAR(8202) + NCHAR(8232)
                        + NCHAR(8233) + NCHAR(8239) + NCHAR(8287) + NCHAR(12288),
                    REPLICATE(N'' '', 27)),
                N'' '', N'''') AS NormalizedIdentificationNumber
        ) AS normalized
        WHERE bp.IsDeleted = 0 AND bp.IsActive = 1
        GROUP BY bp.PartnerType, bp.IdentificationTypeId, normalized.NormalizedIdentificationNumber
        HAVING COUNT_BIG(1) > 1
        ORDER BY bp.PartnerType, bp.IdentificationTypeId, normalized.NormalizedIdentificationNumber;');

    IF COL_LENGTH(N'dbo.BusinessPartners', N'GlobalId') IS NULL
        EXEC(N'
            SELECT N''LegacyBothBusinessPartner'' AS Finding,
                   bp.Id,
                   CAST(NULL AS uniqueidentifier) AS GlobalId,
                   bp.Code,
                   (SELECT COUNT_BIG(1) FROM dbo.BusinessPartnerAddresses AS addressItem WHERE addressItem.BusinessPartnerId = bp.Id) AS AddressCount,
                   (SELECT COUNT_BIG(1) FROM dbo.BusinessPartnerContacts AS contactItem WHERE contactItem.BusinessPartnerId = bp.Id) AS ContactCount
            FROM dbo.BusinessPartners AS bp
            WHERE bp.PartnerType = N''Both''
            ORDER BY bp.Id;');
    ELSE
        EXEC(N'
            SELECT N''LegacyBothBusinessPartner'' AS Finding,
                   bp.Id,
                   bp.GlobalId,
                   bp.Code,
                   (SELECT COUNT_BIG(1) FROM dbo.BusinessPartnerAddresses AS addressItem WHERE addressItem.BusinessPartnerId = bp.Id) AS AddressCount,
                   (SELECT COUNT_BIG(1) FROM dbo.BusinessPartnerContacts AS contactItem WHERE contactItem.BusinessPartnerId = bp.Id) AS ContactCount
            FROM dbo.BusinessPartners AS bp
            WHERE bp.PartnerType = N''Both''
            ORDER BY bp.Id;');
END;

IF OBJECT_ID(N'dbo.BusinessPartnerSapMapping', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        SELECT N''DuplicateSapCardCode'' AS Finding,
               mapping.SapCardCode,
               COUNT_BIG(1) AS FindingCount
        FROM dbo.BusinessPartnerSapMapping AS mapping
        WHERE NULLIF(LTRIM(RTRIM(mapping.SapCardCode)), N'''') IS NOT NULL
        GROUP BY mapping.SapCardCode
        HAVING COUNT_BIG(1) > 1
        ORDER BY mapping.SapCardCode;

        SELECT N''SapCardCodeTooLong'' AS Finding,
               mapping.BusinessPartnerId,
               mapping.SapCardCode,
               LEN(mapping.SapCardCode) AS SapCardCodeLength
        FROM dbo.BusinessPartnerSapMapping AS mapping
        WHERE LEN(mapping.SapCardCode) > 15
        ORDER BY mapping.BusinessPartnerId;');
END;

IF OBJECT_ID(N'dbo.BusinessPartnerAddresses', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerAddresses', N'GlobalId') IS NULL
        EXEC(N'SELECT N''MissingAddressGlobalId'' AS Finding, addressItem.Id, addressItem.BusinessPartnerId FROM dbo.BusinessPartnerAddresses AS addressItem ORDER BY addressItem.Id;');
    ELSE
        EXEC(N'SELECT N''MissingAddressGlobalId'' AS Finding, addressItem.Id, addressItem.BusinessPartnerId FROM dbo.BusinessPartnerAddresses AS addressItem WHERE addressItem.GlobalId IS NULL ORDER BY addressItem.Id;');
END;

IF OBJECT_ID(N'dbo.BusinessPartnerContacts', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.BusinessPartnerContacts', N'GlobalId') IS NULL
        EXEC(N'SELECT N''MissingContactGlobalId'' AS Finding, contactItem.Id, contactItem.BusinessPartnerId FROM dbo.BusinessPartnerContacts AS contactItem ORDER BY contactItem.Id;');
    ELSE
        EXEC(N'SELECT N''MissingContactGlobalId'' AS Finding, contactItem.Id, contactItem.BusinessPartnerId FROM dbo.BusinessPartnerContacts AS contactItem WHERE contactItem.GlobalId IS NULL ORDER BY contactItem.Id;');
END;

IF OBJECT_ID(N'dbo.LocalOutbox', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        SELECT N''PendingBusinessPartnerLocalOutbox'' AS Finding,
               item.Id, item.EventId, item.EntityName, item.EntityGlobalId, item.Status, item.CreatedAt
        FROM dbo.LocalOutbox AS item
        WHERE item.EntityName IN (N''BusinessPartner'', N''BusinessPartnerProposal'', N''BusinessPartnerProposalResult'')
          AND item.Status IN (N''Pending'', N''InProcess'', N''Error'')
        ORDER BY item.CreatedAt, item.Id;');
END;

IF OBJECT_ID(N'dbo.SyncOutbox', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        SELECT N''PendingBusinessPartnerSyncOutbox'' AS Finding,
               item.Id, item.EventId, item.EntityName, item.EntityGlobalId, item.Status, item.CreatedAt
        FROM dbo.SyncOutbox AS item
        WHERE item.EntityName IN (N''BusinessPartner'', N''BusinessPartnerProposal'', N''BusinessPartnerProposalResult'')
          AND item.Status IN (N''Pending'', N''InProcess'', N''Error'')
        ORDER BY item.CreatedAt, item.Id;');
END;

IF OBJECT_ID(N'dbo.SyncInbox', N'U') IS NOT NULL
BEGIN
    EXEC(N'
        SELECT N''PendingBusinessPartnerSyncInbox'' AS Finding,
               item.Id, item.EventId, item.EntityName, item.EntityGlobalId, item.Status, item.ReceivedAt
        FROM dbo.SyncInbox AS item
        WHERE item.EntityName IN (N''BusinessPartner'', N''BusinessPartnerProposal'', N''BusinessPartnerProposalResult'')
          AND item.Status IN (N''Pending'', N''InProcess'', N''Error'')
        ORDER BY item.ReceivedAt, item.Id;');
END;

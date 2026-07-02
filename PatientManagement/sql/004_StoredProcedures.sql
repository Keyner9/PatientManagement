CREATE OR ALTER PROCEDURE [dbo].[usp_GetPatientsCreatedAfterDate]
    @CreatedAfter DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [PatientId],
           [DocumentType],
           [DocumentNumber],
           [FirstName],
           [LastName],
           [BirthDate],
           [PhoneNumber],
           [Email],
           [CreatedAt]
    FROM [dbo].[Patients]
    WHERE [CreatedAt] > @CreatedAfter
    ORDER BY [CreatedAt] DESC;
END;
GO

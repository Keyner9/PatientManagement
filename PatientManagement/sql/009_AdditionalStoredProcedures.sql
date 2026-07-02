USE [PatientManagementDb];
GO

-- ========================================================
-- Procedure: usp_GetDoctorAppointmentsSummary
-- Description: Returns appointment count and total fee
--              per doctor within an optional date range.
-- Parameters: @StartDate, @EndDate (optional)
-- ========================================================
CREATE OR ALTER PROCEDURE [dbo].[usp_GetDoctorAppointmentsSummary]
    @StartDate DATETIME2 = NULL,
    @EndDate   DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.[DoctorId],
        d.[FirstName],
        d.[LastName],
        d.[Specialty],
        COUNT(a.[AppointmentId]) AS [TotalAppointments],
        ISNULL(SUM(a.[Fee]), 0) AS [TotalBilled]
    FROM [dbo].[Doctors] d
    LEFT JOIN [dbo].[Appointments] a ON d.[DoctorId] = a.[DoctorId]
        AND (@StartDate IS NULL OR a.[AppointmentDate] >= @StartDate)
        AND (@EndDate IS NULL OR a.[AppointmentDate] <= @EndDate)
    GROUP BY d.[DoctorId], d.[FirstName], d.[LastName], d.[Specialty]
    ORDER BY [TotalBilled] DESC, [TotalAppointments] DESC;
END;
GO

-- ========================================================
-- Procedure: usp_GetPatientAppointmentHistory
-- Description: Returns the full appointment history for a
--              specific patient, including doctor details.
-- Parameters: @PatientId
-- ========================================================
CREATE OR ALTER PROCEDURE [dbo].[usp_GetPatientAppointmentHistory]
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.[AppointmentId],
        a.[AppointmentDate],
        a.[Diagnosis],
        a.[Treatment],
        a.[Fee],
        d.[FirstName] + ' ' + d.[LastName] AS [DoctorName],
        d.[Specialty],
        d.[PhoneNumber] AS [DoctorPhone]
    FROM [dbo].[Appointments] a
    INNER JOIN [dbo].[Doctors] d ON a.[DoctorId] = d.[DoctorId]
    WHERE a.[PatientId] = @PatientId
    ORDER BY a.[AppointmentDate] DESC;
END;
GO

-- ========================================================
-- Procedure: usp_GetMultiSpecialtyPatients
-- Description: Returns patients who have been attended in
--              more than one medical specialty.
-- ========================================================
CREATE OR ALTER PROCEDURE [dbo].[usp_GetMultiSpecialtyPatients]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.[PatientId],
        p.[FirstName],
        p.[LastName],
        p.[DocumentType],
        p.[DocumentNumber],
        COUNT(DISTINCT d.[Specialty]) AS [SpecialtyCount],
        STRING_AGG(DISTINCT d.[Specialty], ', ') AS [Specialties]
    FROM [dbo].[Patients] p
    INNER JOIN [dbo].[Appointments] a ON p.[PatientId] = a.[PatientId]
    INNER JOIN [dbo].[Doctors] d ON a.[DoctorId] = d.[DoctorId]
    GROUP BY p.[PatientId], p.[FirstName], p.[LastName], p.[DocumentType], p.[DocumentNumber]
    HAVING COUNT(DISTINCT d.[Specialty]) > 1
    ORDER BY [SpecialtyCount] DESC;
END;
GO

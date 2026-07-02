USE [PatientManagementDb];
GO

-- =====================================================
-- Query 1: Top 5 patients with the most appointments
-- =====================================================
SELECT TOP 5
    p.[PatientId],
    p.[FirstName],
    p.[LastName],
    p.[DocumentType],
    p.[DocumentNumber],
    COUNT(a.[AppointmentId]) AS [TotalAppointments]
FROM [dbo].[Patients] p
INNER JOIN [dbo].[Appointments] a ON p.[PatientId] = a.[PatientId]
GROUP BY p.[PatientId], p.[FirstName], p.[LastName], p.[DocumentType], p.[DocumentNumber]
ORDER BY COUNT(a.[AppointmentId]) DESC;
GO

-- =====================================================
-- Query 2: Doctors with no appointments
-- =====================================================
SELECT
    d.[DoctorId],
    d.[FirstName],
    d.[LastName],
    d.[Specialty],
    d.[DocumentNumber]
FROM [dbo].[Doctors] d
LEFT JOIN [dbo].[Appointments] a ON d.[DoctorId] = a.[DoctorId]
WHERE a.[AppointmentId] IS NULL;
GO

-- =====================================================
-- Query 3: Total amount billed per doctor
-- =====================================================
SELECT
    d.[DoctorId],
    d.[FirstName],
    d.[LastName],
    d.[Specialty],
    COUNT(a.[AppointmentId]) AS [TotalAppointments],
    SUM(a.[Fee]) AS [TotalBilled]
FROM [dbo].[Doctors] d
INNER JOIN [dbo].[Appointments] a ON d.[DoctorId] = a.[DoctorId]
GROUP BY d.[DoctorId], d.[FirstName], d.[LastName], d.[Specialty]
ORDER BY SUM(a.[Fee]) DESC;
GO

-- =====================================================
-- Query 4: Patients attended in more than one specialty
-- =====================================================
SELECT
    p.[PatientId],
    p.[FirstName],
    p.[LastName],
    p.[DocumentNumber],
    COUNT(DISTINCT d.[Specialty]) AS [SpecialtyCount],
    STRING_AGG(DISTINCT d.[Specialty], ', ') AS [Specialties]
FROM [dbo].[Patients] p
INNER JOIN [dbo].[Appointments] a ON p.[PatientId] = a.[PatientId]
INNER JOIN [dbo].[Doctors] d ON a.[DoctorId] = d.[DoctorId]
GROUP BY p.[PatientId], p.[FirstName], p.[LastName], p.[DocumentNumber]
HAVING COUNT(DISTINCT d.[Specialty]) > 1
ORDER BY COUNT(DISTINCT d.[Specialty]) DESC;
GO

-- =====================================================
-- Query 5: Patients attended in the last month
-- =====================================================
SELECT DISTINCT
    p.[PatientId],
    p.[FirstName],
    p.[LastName],
    p.[DocumentNumber],
    p.[PhoneNumber],
    p.[Email],
    a.[AppointmentDate],
    d.[FirstName] + ' ' + d.[LastName] AS [DoctorName],
    d.[Specialty]
FROM [dbo].[Patients] p
INNER JOIN [dbo].[Appointments] a ON p.[PatientId] = a.[PatientId]
INNER JOIN [dbo].[Doctors] d ON a.[DoctorId] = d.[DoctorId]
WHERE a.[AppointmentDate] >= DATEADD(MONTH, -1, GETUTCDATE())
ORDER BY a.[AppointmentDate] DESC;
GO

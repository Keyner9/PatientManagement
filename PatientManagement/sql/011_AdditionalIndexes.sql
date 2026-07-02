USE [PatientManagementDb];
GO

-- ============================================================
-- Index 1: IX_Appointments_PatientId
-- Purpose: Optimizes queries that filter or join Appointments
--          by PatientId, such as Query 1 (Top 5 patients with
--          most appointments) and Query 5 (patients in last
--          month). Also speeds up FK lookups.
-- ============================================================
CREATE NONCLUSTERED INDEX [IX_Appointments_PatientId]
    ON [dbo].[Appointments]([PatientId] ASC)
    INCLUDE ([DoctorId], [AppointmentDate], [Fee]);
GO

-- ============================================================
-- Index 2: IX_Appointments_DoctorId
-- Purpose: Optimizes queries that group or join by DoctorId,
--          such as Query 2 (doctors with no appointments) and
--          Query 3 (total billed per doctor). The INCLUDE
--          columns cover the Fee and AppointmentDate needed
--          for aggregations without key lookups.
-- ============================================================
CREATE NONCLUSTERED INDEX [IX_Appointments_DoctorId]
    ON [dbo].[Appointments]([DoctorId] ASC)
    INCLUDE ([PatientId], [AppointmentDate], [Fee]);
GO

-- ============================================================
-- Index 3: IX_Appointments_AppointmentDate
-- Purpose: Optimizes date-range queries such as Query 5
--          (patients attended in the last month) and the
--          usp_GetDoctorAppointmentsSummary procedure when
--          filtering by date window. The INCLUDE clause
--          covers all columns needed by these queries.
-- ============================================================
CREATE NONCLUSTERED INDEX [IX_Appointments_AppointmentDate]
    ON [dbo].[Appointments]([AppointmentDate] ASC)
    INCLUDE ([PatientId], [DoctorId], [Fee]);
GO

-- ============================================================
-- Index 4: IX_Doctors_Specialty
-- Purpose: Optimizes Query 4 (patients attended in more than
--          one specialty), which groups by Specialty after
--          joining Doctors with Appointments. Also speeds up
--          any future filtering or grouping by specialty.
-- ============================================================
CREATE NONCLUSTERED INDEX [IX_Doctors_Specialty]
    ON [dbo].[Doctors]([Specialty] ASC)
    INCLUDE ([FirstName], [LastName]);
GO

-- ============================================================
-- Index 5: IX_Appointments_PatientId_AppointmentDate
-- Purpose: Covering index for usp_GetPatientAppointmentHistory,
--          which retrieves all appointments for a patient
--          ordered by AppointmentDate DESC. This single index
--          covers the WHERE, ORDER BY, and all selected
--          columns (via included columns), eliminating key
--          lookups entirely.
-- ============================================================
CREATE NONCLUSTERED INDEX [IX_Appointments_PatientId_AppointmentDate]
    ON [dbo].[Appointments]([PatientId] ASC, [AppointmentDate] DESC)
    INCLUDE ([DoctorId], [Diagnosis], [Treatment], [Fee]);
GO

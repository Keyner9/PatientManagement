CREATE TABLE [dbo].[Appointments] (
    [AppointmentId]  INT             IDENTITY (1, 1) NOT NULL,
    [PatientId]      INT             NOT NULL,
    [DoctorId]       INT             NOT NULL,
    [AppointmentDate] DATETIME2      NOT NULL,
    [Diagnosis]      NVARCHAR (500)  NULL,
    [Treatment]      NVARCHAR (500)  NULL,
    [Fee]            DECIMAL (10, 2) NOT NULL DEFAULT 0,
    [CreatedAt]      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([AppointmentId] ASC),
    CONSTRAINT [FK_Appointments_Patients]
        FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patients]([PatientId]),
    CONSTRAINT [FK_Appointments_Doctors]
        FOREIGN KEY ([DoctorId]) REFERENCES [dbo].[Doctors]([DoctorId])
);
GO

INSERT INTO [dbo].[Appointments] ([PatientId], [DoctorId], [AppointmentDate], [Diagnosis], [Treatment], [Fee], [CreatedAt])
VALUES
    (1, 1, '2026-06-15 09:00:00', 'Chest pain, suspected angina', 'Prescribed nitroglycerin and rest', 150.00, '2026-06-15 08:30:00'),
    (1, 4, '2026-06-20 10:30:00', 'Regular check-up', 'Routine blood work', 80.00, '2026-06-20 10:00:00'),
    (1, 2, '2026-06-25 11:00:00', 'Child vaccination follow-up', 'Applied booster dose', 60.00, '2026-06-25 10:30:00'),
    (2, 2, '2026-06-18 08:00:00', 'Pediatric check-up', 'Growth and development evaluation', 90.00, '2026-06-18 07:30:00'),
    (2, 4, '2026-06-22 14:00:00', 'General fatigue', 'Vitamin deficiency test', 70.00, '2026-06-22 13:30:00'),
    (3, 3, '2026-06-10 15:00:00', 'Skin rash on arms', 'Prescribed antihistamine cream', 120.00, '2026-06-10 14:30:00'),
    (3, 1, '2026-06-28 09:30:00', 'Hypertension follow-up', 'Adjusted medication dosage', 130.00, '2026-06-28 09:00:00'),
    (4, 4, '2026-06-12 10:00:00', 'Annual physical exam', 'All tests normal', 85.00, '2026-06-12 09:30:00'),
    (4, 5, '2026-06-19 16:00:00', 'Ankle sprain', 'Bandage and anti-inflammatory', 110.00, '2026-06-19 15:30:00'),
    (5, 1, '2026-06-05 11:30:00', 'Heart palpitations', 'ECG performed, diagnosed arrhythmia', 200.00, '2026-06-05 11:00:00'),
    (5, 3, '2026-06-14 09:00:00', 'Allergic reaction', 'Antihistamine injection', 95.00, '2026-06-14 08:30:00'),
    (5, 4, '2026-06-21 13:00:00', 'Digestive issues', 'Gastroenteritis diagnosed', 75.00, '2026-06-21 12:30:00'),
    (6, 6, '2026-06-08 15:30:00', 'Blurred vision', 'Prescribed corrective lenses', 180.00, '2026-06-08 15:00:00'),
    (6, 4, '2026-06-17 08:00:00', 'Migraine', 'Pain management plan', 65.00, '2026-06-17 07:30:00'),
    (7, 1, '2026-06-02 10:00:00', 'Chest discomfort', 'Stress test recommended', 160.00, '2026-06-02 09:30:00'),
    (8, 2, '2026-06-11 14:30:00', 'Child development assessment', 'Speech therapy referral', 85.00, '2026-06-11 14:00:00'),
    (8, 4, '2026-06-25 11:30:00', 'Seasonal allergies', 'Antihistamine prescription', 60.00, '2026-06-25 11:00:00'),
    (9, 5, '2026-06-07 09:00:00', 'Lower back pain', 'Physical therapy sessions prescribed', 140.00, '2026-06-07 08:30:00'),
    (9, 4, '2026-06-16 10:00:00', 'Respiratory infection', 'Antibiotics prescribed', 75.00, '2026-06-16 09:30:00'),
    (10, 3, '2026-06-09 16:00:00', 'Acne outbreak', 'Topical treatment prescribed', 105.00, '2026-06-09 15:30:00');
GO

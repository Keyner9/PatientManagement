CREATE TABLE [dbo].[Doctors] (
    [DoctorId]      INT           IDENTITY (1, 1) NOT NULL,
    [DocumentType]  NVARCHAR (10) NOT NULL,
    [DocumentNumber] NVARCHAR (20) NOT NULL,
    [FirstName]     NVARCHAR (80) NOT NULL,
    [LastName]      NVARCHAR (80) NOT NULL,
    [Specialty]     NVARCHAR (100) NOT NULL,
    [PhoneNumber]   NVARCHAR (20) NULL,
    [Email]         NVARCHAR (120) NULL,
    [CreatedAt]     DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Doctors] PRIMARY KEY CLUSTERED ([DoctorId] ASC)
);
GO

INSERT INTO [dbo].[Doctors] ([DocumentType], [DocumentNumber], [FirstName], [LastName], [Specialty], [PhoneNumber], [Email], [CreatedAt])
VALUES
    ('DNI', '11111111', 'Ricardo', 'Mendoza Torres', 'Cardiology', '555-0201', 'ricardo.mendoza@hospital.com', '2026-05-01 08:00:00'),
    ('DNI', '22222222', 'Elena', 'Vargas Rivas', 'Pediatrics', '555-0202', 'elena.vargas@hospital.com', '2026-05-01 08:00:00'),
    ('DNI', '33333333', 'Fernando', 'Castillo Núñez', 'Dermatology', '555-0203', 'fernando.castillo@hospital.com', '2026-05-01 08:00:00'),
    ('CE', 'A12345678', 'Gabriela', 'Paredes Luna', 'General Medicine', '555-0204', 'gabriela.paredes@hospital.com', '2026-05-01 08:00:00'),
    ('DNI', '44444444', 'Héctor', 'Ríos Salazar', 'Traumatology', '555-0205', 'hector.rios@hospital.com', '2026-05-01 08:00:00'),
    ('DNI', '55555555', 'Marina', 'Delgado Ruiz', 'Ophthalmology', '555-0206', 'marina.delgado@hospital.com', '2026-05-01 08:00:00');
GO

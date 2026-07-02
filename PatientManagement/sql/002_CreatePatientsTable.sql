CREATE TABLE [dbo].[Patients] (
    [PatientId]     INT           IDENTITY (1, 1) NOT NULL,
    [DocumentType]  NVARCHAR (10) NOT NULL,
    [DocumentNumber] NVARCHAR (20) NOT NULL,
    [FirstName]     NVARCHAR (80) NOT NULL,
    [LastName]      NVARCHAR (80) NOT NULL,
    [BirthDate]     DATE          NOT NULL,
    [PhoneNumber]   NVARCHAR (20) NULL,
    [Email]         NVARCHAR (120) NULL,
    [CreatedAt]     DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([PatientId] ASC)
);
GO

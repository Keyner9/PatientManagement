CREATE UNIQUE NONCLUSTERED INDEX [UIX_Patients_DocumentType_DocumentNumber]
    ON [dbo].[Patients]([DocumentType] ASC, [DocumentNumber] ASC);
GO

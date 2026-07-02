INSERT INTO [dbo].[Patients] ([DocumentType], [DocumentNumber], [FirstName], [LastName], [BirthDate], [PhoneNumber], [Email], [CreatedAt])
VALUES
    ('DNI', '12345678', 'Carlos', 'García López', '1985-03-15', '555-0101', 'carlos.garcia@email.com', '2026-06-01 08:30:00'),
    ('DNI', '23456789', 'María', 'Rodríguez Martínez', '1990-07-22', '555-0102', 'maria.rodriguez@email.com', '2026-06-05 10:15:00'),
    ('DNI', '34567890', 'Juan', 'Pérez Sánchez', '1978-11-10', '555-0103', 'juan.perez@email.com', '2026-06-10 14:00:00'),
    ('CE', '987654321', 'Ana', 'Torres Castillo', '1995-01-30', '555-0104', 'ana.torres@email.com', '2026-06-15 09:45:00'),
    ('DNI', '45678901', 'Luis', 'Hernández Vargas', '1982-09-05', NULL, NULL, '2026-06-20 11:30:00'),
    ('CE', '876543210', 'Sofía', 'Ramírez Paredes', '2000-12-18', '555-0105', 'sofia.ramirez@email.com', '2026-06-25 16:00:00'),
    ('DNI', '56789012', 'Diego', 'Fernández Ortiz', '1975-04-02', '555-0106', 'diego.fernandez@email.com', '2026-07-01 07:00:00'),
    ('DNI', '67890123', 'Laura', 'Díaz Mendoza', '1992-08-14', NULL, 'laura.diaz@email.com', '2026-07-05 13:20:00'),
    ('CE', '765432109', 'Pedro', 'Guzmán Ríos', '1988-05-27', '555-0107', 'pedro.guzman@email.com', '2026-07-10 15:45:00'),
    ('DNI', '78901234', 'Valentina', 'Cruz Morales', '1998-02-09', NULL, NULL, '2026-07-15 18:00:00');
GO

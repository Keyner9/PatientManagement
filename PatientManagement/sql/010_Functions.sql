USE [PatientManagementDb];
GO

-- ========================================================
-- Function: fn_CalculateAge
-- Description: Calculates the age of a person given their
--              birth date, as of today (or a reference date).
-- Parameters: @BirthDate (DATE), @ReferenceDate (optional,
--             defaults to GETUTCDATE())
-- Returns: INT (age in years)
-- Usage: SELECT dbo.fn_CalculateAge('1990-03-15', DEFAULT);
-- ========================================================
CREATE OR ALTER FUNCTION [dbo].[fn_CalculateAge]
(
    @BirthDate     DATE,
    @ReferenceDate DATE = NULL
)
RETURNS INT
AS
BEGIN
    IF @ReferenceDate IS NULL
        SET @ReferenceDate = CAST(GETUTCDATE() AS DATE);

    RETURN DATEDIFF(YEAR, @BirthDate, @ReferenceDate)
           - CASE
                 WHEN MONTH(@ReferenceDate) < MONTH(@BirthDate)
                      OR (MONTH(@ReferenceDate) = MONTH(@BirthDate)
                          AND DAY(@ReferenceDate) < DAY(@BirthDate))
                 THEN 1
                 ELSE 0
             END;
END;
GO

USE ARCHIVO;
GO

SELECT
    name,
    physical_name,
    type_desc
FROM sys.database_files;
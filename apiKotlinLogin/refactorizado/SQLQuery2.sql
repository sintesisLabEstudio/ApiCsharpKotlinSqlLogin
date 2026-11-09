SELECT name AS [Nombre lógico], type_desc AS [Tipo], physical_name AS [Ruta en disco]
FROM sys.master_files
WHERE database_id = DB_ID(N'ARCHIVO')
ORDER BY file_id;
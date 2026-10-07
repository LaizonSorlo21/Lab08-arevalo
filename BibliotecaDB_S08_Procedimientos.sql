/* =====================================================================
   BibliotecaDB - Laboratorio 08: procedimientos almacenados
   REQUISITO: BibliotecaDB ya existe (script BibliotecaDB.sql del Lab 07).
   Este script NO crea ni borra tablas; solo crea/actualiza procedimientos.
   Se puede ejecutar varias veces (CREATE OR ALTER).
   Ejecutar en SSMS o con: sqlcmd -S . -E -i BibliotecaDB_S08_Procedimientos.sql
   ===================================================================== */
USE BibliotecaDB;
GO

/* ------------------------------- AUTORES ------------------------------- */
CREATE OR ALTER PROCEDURE dbo.usp_Autores_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AutorId, Nombre, Nacionalidad, Activo
    FROM dbo.Autores
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

/* ------------------------------- LIBROS -------------------------------- */
-- Lista libros activos con el nombre del autor; @Titulo NULL/'' = todos.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_Listar
    @Titulo NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, a.Nombre AS NombreAutor,
           l.Ejemplares, l.Activo
    FROM dbo.Libros l
    INNER JOIN dbo.Autores a ON a.AutorId = l.AutorId
    WHERE l.Activo = 1
      AND (@Titulo IS NULL OR @Titulo = N'' OR l.Titulo LIKE N'%' + @Titulo + N'%')
    ORDER BY l.Titulo;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Libros_BuscarPorTitulo
    @Titulo NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, a.Nombre AS NombreAutor,
           l.Ejemplares, l.Activo
    FROM dbo.Libros l
    INNER JOIN dbo.Autores a ON a.AutorId = l.AutorId
    WHERE l.Activo = 1
      AND l.Titulo LIKE N'%' + @Titulo + N'%'
    ORDER BY l.Titulo;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Libros_ObtenerPorId
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, a.Nombre AS NombreAutor,
           l.Ejemplares, l.Activo
    FROM dbo.Libros l
    INNER JOIN dbo.Autores a ON a.AutorId = l.AutorId
    WHERE l.LibroId = @LibroId AND l.Activo = 1;
END
GO

-- Devuelve el nuevo LibroId, o -1 si el ISBN ya existe.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_Insertar
    @Titulo     NVARCHAR(200),
    @ISBN       NVARCHAR(20),
    @AutorId    INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN = @ISBN)
    BEGIN
        SELECT -1;
        RETURN;
    END

    INSERT INTO dbo.Libros (Titulo, ISBN, AutorId, Ejemplares, Activo)
    VALUES (@Titulo, @ISBN, @AutorId, @Ejemplares, 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- Devuelve 1 si actualizo, 0 si no existe, -1 si el ISBN pertenece a otro libro.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_Actualizar
    @LibroId    INT,
    @Titulo     NVARCHAR(200),
    @ISBN       NVARCHAR(20),
    @AutorId    INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN = @ISBN AND LibroId <> @LibroId)
    BEGIN
        SELECT -1;
        RETURN;
    END

    UPDATE dbo.Libros
    SET Titulo = @Titulo, ISBN = @ISBN, AutorId = @AutorId, Ejemplares = @Ejemplares
    WHERE LibroId = @LibroId AND Activo = 1;

    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END;
END
GO

-- Eliminacion LOGICA: Activo = 0 (nunca DELETE fisico).
CREATE OR ALTER PROCEDURE dbo.usp_Libros_EliminarLogico
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Libros SET Activo = 0 WHERE LibroId = @LibroId AND Activo = 1;
    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END;
END
GO

/* ------------------------------- SOCIOS -------------------------------- */
CREATE OR ALTER PROCEDURE dbo.usp_Socios_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SocioId, DNI, Nombre, Email, Activo
    FROM dbo.Socios
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Socios_ExisteDNI
    @DNI NVARCHAR(8)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.Socios WHERE DNI = @DNI) THEN 1 ELSE 0 END;
END
GO

-- Devuelve el nuevo SocioId, o -1 si el DNI ya existe.
CREATE OR ALTER PROCEDURE dbo.usp_Socios_Insertar
    @DNI    NVARCHAR(8),
    @Nombre NVARCHAR(100),
    @Email  NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Socios WHERE DNI = @DNI)
    BEGIN
        SELECT -1;
        RETURN;
    END

    INSERT INTO dbo.Socios (DNI, Nombre, Email, Activo)
    VALUES (@DNI, @Nombre, @Email, 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

/* ------------------------------ PRESTAMOS ------------------------------ */
-- Reporte por intervalo de FechaPrestamo (ambos extremos inclusive).
CREATE OR ALTER PROCEDURE dbo.usp_Prestamos_Reporte
    @Desde DATE,
    @Hasta DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Nombre AS Socio,
           l.Titulo AS Libro,
           p.FechaPrestamo,
           p.FechaLimite,
           p.Estado
    FROM dbo.Prestamos p
    INNER JOIN dbo.DetallePrestamo d ON d.PrestamoId = p.PrestamoId
    INNER JOIN dbo.Libros l          ON l.LibroId    = d.LibroId
    INNER JOIN dbo.Socios s          ON s.SocioId    = p.SocioId
    WHERE p.FechaPrestamo BETWEEN @Desde AND @Hasta
    ORDER BY p.FechaPrestamo, s.Nombre, l.Titulo;
END
GO

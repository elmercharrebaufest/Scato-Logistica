CREATE TABLE [dbo].[CredencialMercadoPago]
(
    [Id] INT IDENTITY (1, 1) NOT NULL, 
    [PublicKey] NVARCHAR(MAX) NOT NULL, 
    [AccessToken] NVARCHAR(MAX) NOT NULL, 
    [AppId] BIGINT NULL, 
    [SecretKey] NVARCHAR(50) NULL, 
    [RedirectUri] NVARCHAR(MAX) NULL, 
    [TokenActualizarPermiso] NVARCHAR(50) NULL, 
    [UsuarioVendedorId] BIGINT NULL, 
    [FechaVencimientoPermisos] DATETIME NULL,
    CONSTRAINT [PK_dbo.CredencialMercadoPago] PRIMARY KEY CLUSTERED ([Id] ASC), 
)

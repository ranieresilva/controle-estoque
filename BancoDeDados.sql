USE [master]
GO
CREATE DATABASE [controle-estoque]
GO
USE [controle-estoque]
GO
CREATE USER [admin] FOR LOGIN [admin] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [admin]
GO
CREATE TABLE [dbo].[grupo_produto] (
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [nvarchar](50) NULL,
	[ativo] [bit] NULL,
	CONSTRAINT [PK_grupo_produto] PRIMARY KEY ([id])
)
GO
CREATE TABLE [dbo].[produto](
	[id] [int] NULL,
	[nome] [nchar](10) NULL
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[perfil] (
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [varchar](20) NOT NULL,
	[ativo] [bit] NOT NULL,
	CONSTRAINT [PK_perfil] PRIMARY KEY ([id])
)
GO
CREATE TABLE [dbo].[unidade_medida] (
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [varchar](30) NOT NULL,
	[sigla] [varchar](3) NOT NULL,
	[ativo] [bit] NOT NULL,
	CONSTRAINT [PK_unidade_medida] PRIMARY KEY ([id])
)
GO
CREATE TABLE [dbo].[usuario] (
	[id] [int] IDENTITY(1,1) NOT NULL,
	[login] [nvarchar](50) NOT NULL,
	[senha] [nvarchar](32) NOT NULL,
	[nome] [nvarchar](100) NOT NULL,
	[id_perfil] [int] NOT NULL,
	CONSTRAINT [PK_usuario] PRIMARY KEY ([id])
)
GO
ALTER TABLE [dbo].[usuario]  WITH CHECK ADD FOREIGN KEY([id_perfil]) REFERENCES [dbo].[perfil] ([id])
GO

CREATE TABLE [dbo].[marca_produto] (
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [varchar](50) NOT NULL,
	[ativo] [bit] NOT NULL,
	CONSTRAINT [PK_marca_produto] PRIMARY KEY ([id])
)
GO
CREATE TABLE [dbo].[local_armazenamento] (
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [varchar](50) NOT NULL,
	[ativo] [bit] NOT NULL,
	CONSTRAINT [PK_local_armazenamento] PRIMARY KEY ([id])
)
GO
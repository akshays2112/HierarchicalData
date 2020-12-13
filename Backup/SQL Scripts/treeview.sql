--IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'treeview')
--	DROP DATABASE [treeview]
--GO

CREATE DATABASE [treeview]  ON (NAME = N'treeview_Data', FILENAME = N'd:\MSSQL2K\MSSQL$SQL2K\data\treeview_Data.MDF' , SIZE = 1, FILEGROWTH = 10%) LOG ON (NAME = N'treeview_Log', FILENAME = N'd:\MSSQL2K\MSSQL$SQL2K\data\treeview_Log.LDF' , SIZE = 1, FILEGROWTH = 10%)
 COLLATE SQL_Latin1_General_CP1_CI_AS
GO

use [treeview]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DataTable]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[DataTable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LoadTableLaptopDiskSize]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[LoadTableLaptopDiskSize]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LoadTableLaptopMemory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[LoadTableLaptopMemory]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LoadTableLaptopType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[LoadTableLaptopType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LoadTablePeopleNames]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[LoadTablePeopleNames]
GO

CREATE TABLE [dbo].[DataTable] (
	[id] [int] IDENTITY (1, 1) NOT NULL ,
	[treekey] [varchar] (4000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[parentid] [int] NOT NULL ,
	[data] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoadTableLaptopDiskSize] (
	[DiskSize] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoadTableLaptopMemory] (
	[Memory] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoadTableLaptopType] (
	[Name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LoadTablePeopleNames] (
	[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO


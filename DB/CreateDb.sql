USE [master]
GO

/****** Object:  Database [StockService.Core.DTOs.StockScannerContext]    Script Date: 01/11/2014 22:38:04 ******/
CREATE DATABASE [StockService.Core.DTOs.StockScannerContext] ON  PRIMARY 
( NAME = N'StockService.Core.DTOs.StockScannerContext', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\StockService.Core.DTOs.StockScannerContext.mdf' , SIZE = 2304KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'StockService.Core.DTOs.StockScannerContext_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\StockService.Core.DTOs.StockScannerContext_log.LDF' , SIZE = 576KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [StockService.Core.DTOs.StockScannerContext].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET ARITHABORT OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET AUTO_CLOSE ON 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET  ENABLE_BROKER 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET  READ_WRITE 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET  MULTI_USER 
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [StockService.Core.DTOs.StockScannerContext] SET DB_CHAINING OFF 
GO


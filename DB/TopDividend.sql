USE [StockService.Core.DTOs.StockScannerContext]
GO
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TopDividendStocks
	-- Add the parameters for the stored procedure here
	@IndustryId int=0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select top 10 sq.Dividend, c.Name
	from StockQuotes sq 
	join Companies c on sq.CompanyId = c.CompanyId
	join Industries i on i.IndustryId = c.IndustryId 
	where i.IndustryId=@IndustryId
	order by sq.Dividend desc
END
GO

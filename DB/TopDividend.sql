USE [StockService.Core.DTOs.StockScannerContext]
GO

/****** Object:  StoredProcedure [dbo].[TopDividendStocks]    Script Date: 02/05/2014 19:54:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TopDividendStocks]
	-- Add the parameters for the stored procedure here
	@IndustryId int=0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select top 10 cs.TrailingAnnualDividendYieldPerc, c.Name
	from CompanyStatistics cs 
	join Companies c on cs.CompanyId = c.CompanyId
	join Industries i on i.IndustryId = c.IndustryId 
	where i.IndustryId=@IndustryId and cs.TrailingAnnualDividendYieldPerc is not null
	order by cs.TrailingAnnualDividendYieldPerc desc
END


GO



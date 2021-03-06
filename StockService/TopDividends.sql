--select TOP 100 [TrailingAnnualDividendYield]
--from CompanyStatistics
--where [TrailingAnnualDividendYield] is not null
--Order By [TrailingAnnualDividendYield] Desc


Select sq.Volume from StockQuotes sq join (select c.CompanyId
from Companies c
join Industries i on i.IndustryId = c.IndustryId where i.IndustryId=1) on CompanyId = sq.CompanyId



Select top 10 sq.Volume, c.Name
from StockQuotes sq 
join Companies c on sq.CompanyId = c.CompanyId
join Industries i on i.IndustryId = c.IndustryId 
where i.IndustryId=1
order by sq.Volume desc

USE [StockService.Core.DTOs.StockScannerContext]
GO

/****** Object:  StoredProcedure [dbo].[TopVolume]    Script Date: 11/26/2013 20:28:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TopVolume]
	-- Add the parameters for the stored procedure here
	@IndustryId int=0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select top 10 sq.Volume, c.Name
	from StockQuotes sq 
	join Companies c on sq.CompanyId = c.CompanyId
	join Industries i on i.IndustryId = c.IndustryId 
	where i.IndustryId=@IndustryId
	order by sq.Volume desc
END

GO



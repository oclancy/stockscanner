USE [StockService.Core.DTOs.StockScannerContext]
GO
/****** Object:  StoredProcedure [dbo].[TopVolume]    Script Date: 02/12/2014 22:28:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[TopVolume]
	-- Add the parameters for the stored procedure here
	@IndustryId int=0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select top 10 sq.Volume as Value, c.Name, c.Symbol
	from StockQuotes sq 
	join Companies c on sq.CompanyId = c.CompanyId
	join Industries i on i.IndustryId = c.IndustryId 
	where i.IndustryId=@IndustryId
	order by sq.Volume desc
END


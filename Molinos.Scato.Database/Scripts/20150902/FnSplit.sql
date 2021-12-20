USE [Scato_Branch]
GO

/****** Object:  UserDefinedFunction [dbo].[FnSplit]    Script Date: 08/19/2015 13:19:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[FnSplit]
(
@List nvarchar(2000),
@SplitOn nvarchar(5)
)  
RETURNS @RtnValue table 
(

Id int identity(1,1),
Value nvarchar(100)
) 
AS  
BEGIN
While (Charindex(@SplitOn,@List)>0)
Begin 
Insert Into @RtnValue (value)
Select
Value = ltrim(rtrim(Substring(@List,1,Charindex(@SplitOn,@List)-1))) 
Set @List = Substring(@List,Charindex(@SplitOn,@List)+len(@SplitOn),len(@List))
End 

Insert Into @RtnValue (Value)
Select Value = ltrim(rtrim(@List))
Return
END

GO



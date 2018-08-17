------------------------------------------------------------------------------------
-- =============================================
-- NotificationGetByContactId
-- =============================================
IF EXISTS (
  SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'NotificationGetByContactId' 
)
   DROP PROCEDURE dbo.NotificationGetByContactId
GO
CREATE PROCEDURE dbo.NotificationGetByContactId
    @contactId nvarchar(20),@top int
AS
   SET NOCOUNT ON;
    --collect the notification IDs in a variable table
    DECLARE @tempnotification TABLE 
    ( 
        Id nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL 
    )
    insert into @tempnotification(Id) 
    SELECT n.Id from [Notification] n inner join Contact c on n.TypeCode = c.Id
    where n.Type = 1 --contact
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  
        (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01'  WHEN DATEPART(hh,n.ValidTo) = 0 THEN  dateadd(dd,1,n.ValidTo) ELSE ISNULL(n.ValidTo,'2200-01-01') END)  
    and n.Id not in (select Id from @tempnotification)  
   
    insert into @tempnotification(Id)
    SELECT n.Id from [Notification] n inner join Contact c on n.TypeCode = c.AccountId
    where n.Type = 0 --account
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  
        (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01'  WHEN DATEPART(hh,n.ValidTo) = 0 THEN  dateadd(dd,1,n.ValidTo) ELSE ISNULL(n.ValidTo,'2200-01-01') END)    
    and n.Id not in (select Id from @tempnotification)
        
    insert into @tempnotification(Id)
    SELECT n.Id from [Notification] n inner join Account a on n.TypeCode = a.SchemeId
      inner join Contact c on a.Id = c.AccountId
    where n.Type = 3 --scheme
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  
        (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01'  WHEN DATEPART(hh,n.ValidTo) = 0 THEN  dateadd(dd,1,n.ValidTo) ELSE ISNULL(n.ValidTo,'2200-01-01') END)   
    and n.Id not in (select Id from @tempnotification) 
    
    insert into @tempnotification(Id)
    SELECT n.Id from [Notification] n inner join Scheme s on n.TypeCode = s.ClubId
        inner join Account a on a.SchemeId = s.Id
        inner join Contact c on a.Id = c.AccountId
    where n.Type = 2 --club
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  
        (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01'  WHEN DATEPART(hh,n.ValidTo) = 0 THEN dateadd(dd,1,n.ValidTo) ELSE ISNULL(n.ValidTo,'2200-01-01') END)   
    and n.Id not in (select Id from @tempnotification)            
   
   --same as in notificaitonview, but filter on the contactId
       SELECT top(@top) n.[Id]
      ,n.[Type]
      ,n.[TypeCode]
      ,n.[PrimaryText]
      ,n.[SecondaryText]
      ,n.[DisplayFrequency]
      ,n.[ValidFrom]
      ,n.[ValidTo]
      ,n.[Created]
      ,n.[CreatedBy]
      ,n.[LastModifiedDate]
      ,n.[DateLastModified]
      ,n.[QRText]
      ,n.[NotificationType]
	  ,n.[Status]
    ,isnull(nl.ContactId,'') As ContactId
    ,isnull(nl.DeviceId,'') As DeviceId
    ,isnull(nl.DateDisplayed,'2000-01-01') As DateDisplayed
    ,isnull(nl.DateClosed,'2000-01-01') As DateClosed
    ,isnull(nl.ReplicationCounter,0) As ReplicationCounter
    ,isnull(nl.NotificationStatus,0) as NotificationStatus
    from [Notification] n inner join @tempnotification tn on tn.Id = n.Id  
      left outer join [NotificationLog] nl on n.Id = nl.Id  and nl.ContactId = @contactId
      
    where tn.Id not in (select Id from NotificationLog nl where ContactId = @contactId and NotificationStatus = 2) --not closed
	and n.[Status] = 1
    order by n.[Created] desc
GO
-- =============================================
-- Example to execute the stored procedure NotificationGetByContactId
-- =============================================
-- EXECUTE dbo.NotificationGetByContactId 'MO000008', 11


-- =============================================
-- OneListSearch
-- =============================================
IF EXISTS (
  SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'OneListSearch' 
)
   DROP PROCEDURE dbo.OneListSearch
GO
CREATE PROCEDURE dbo.OneListSearch
    @contactid nvarchar(20), @textsearch nvarchar(200), @top int
AS
   --@textsearch.  Spaces in @textsearch are treated like "AND". 'milk skim' becomes 'milk AND skim'

   SET NOCOUNT ON;
    --check that at least one char is passed in as string
    If len(@textsearch )<1 or @textsearch  is null  
    Begin
      SELECT * FROM [OneList] WHERE 1 = 0
      return
    End
    
    --need to check if full text is supported, azure does not support it
    DECLARE @isItemFullTextIndexed INT; 
    DECLARE @isFullTextInstalled INT;
    SELECT @isItemFullTextIndexed = COLUMNPROPERTY(OBJECT_ID('dbo.OneListItem'), 'ItemDescription', 'IsFulltextIndexed') 
    SELECT @isFullTextInstalled  = FullTextServiceProperty('IsFullTextInstalled')
    DECLARE @idx int  = 1   
    DECLARE @minSearchLength int  = 2  
    DECLARE @slice nvarchar(200) = N''  
    DECLARE @delimiter nchar(1)  = N' '   
    DECLARE @tempsearch nvarchar(500) = N''
    DECLARE @sql nvarchar(500) = N''  
    DECLARE @search nvarchar(200) = @textsearch  

    SET @search = REPLACE(@search,'''','''''')
    SET @search = REPLACE(@search,'*','') --dont allow star. used in full text search
    SET @textsearch = REPLACE(@textsearch ,'''','''''')

    set @search = LTrim(RTrim(@search))
    IF (@isItemFullTextIndexed  = 1 and @isFullTextInstalled = 1) 
    Begin
      -- 'Fulltext column',  must use dynamic sql since sql azure wont compile the SP otherwise
      --Spaces in @search are treated like AND.  'milk skim' needs to be converted to 'milk AND skim'
      --check if I need to replace spaces with AND.
     
        while @idx!= 0       
        begin       
            set @idx = charindex(@delimiter,@search);      
            if @idx!=0       
                set @slice = left(@search,@idx - 1);
            else       
                set @slice = @search;
            
            --at least 2 char long 
            if(len(@slice)>= @minSearchLength and len(@tempsearch)=0)  
                set @tempsearch = '"' + ltrim(rtrim(@slice)) + '*"';
            else if(len(@slice)>= @minSearchLength)  
                set @tempsearch = @tempsearch + ' AND "' + ltrim(rtrim(@slice)) + '*"';
                      
            set @search = right(@search,len(@search) - @idx);
            set @search = ltrim(rtrim(@search))
            if len(@search) = 0 break;    
        end  
      -- 
      --Set @search = N' ''*' + @tempsearch + '*'' '
      --Set @search =  '"' + @tempsearch + '*"' 
      Set @search =   @tempsearch  
       
      if len(@search)=0 
        set @sql = N'SELECT * FROM [OneList] WHERE 1=0' --return empty row
      else
      begin 
          set @sql = N'SELECT distinct TOP(' + convert(nvarchar(20),@top) +') s.* FROM [OneList] s inner join OneListItem sl '
          set @sql = @sql + N' on sl.OneListId = s.Id '
          set @sql = @sql + N' AND s.ContactId = N''' + @contactid + ''''
          set @sql = @sql + ' WHERE CONTAINS(sl.[ItemDescription],'''+ @search +''')'
          set @sql = @sql + N' OR s.Description like N''%' + @textsearch + '%'''
          --set @sql = @sql + N' ORDER BY s.1,i.Description'
      end

       -- select @sql
      EXECUTE( @sql)
      --exec sp_executesql @sql
 
    End 
    ELSE 
    Begin
      -- 'No Fulltext column'  
      --Spaces in @search are treated like AND.  'milk skim' needs to be converted to 'milk AND skim'
      --check if I need to replace spaces with AND.
      --This will do a full table scan, so always use Fulltext search when possible
        while @idx!= 0       
        begin       
            set @idx = charindex(@delimiter,@search);      
            if @idx!=0       
                set @slice = left(@search,@idx - 1);
            else       
                set @slice = @search;
              
            if(len(@slice)>= @minSearchLength and len(@tempsearch)=0)  
                set @tempsearch = 'like N''%' + ltrim(rtrim(@slice)) + '%''' ;
            else if(len(@slice)>= @minSearchLength)  
                set @tempsearch = @tempsearch + ' AND sl.ItemDescription LIKE N''%' + ltrim(rtrim(@slice)) + '%'' ' ;
                      
            set @search = right(@search,len(@search) - @idx);
            set @search = ltrim(rtrim(@search))
            if len(@search) = 0 break;    
        end  

      Set @search =   @tempsearch  
      if len(@search)=0 
        set @sql = N'SELECT * FROM [OneList] WHERE 1=0' --return empty row
      else
      begin 
          set @sql = N'SELECT distinct TOP(' + convert(nvarchar(20),@top) +') s.* FROM [OneList] s inner join OneListItem sl '
          set @sql = @sql + N' on sl.OneListId = s.Id '
          set @sql = @sql + N' AND s.ContactId = N''' + @contactid + ''''
          set @sql = @sql + ' WHERE sl.ItemDescription ' + @tempsearch
          set @sql = @sql + N' OR s.Description like N''%' + @textsearch + '%'''
         -- set @sql = @sql + N' ORDER BY s.Description,i.Description'
      end
      -- select @sql
      EXECUTE( @sql)        
    End
GO
-- =============================================
-- Example to execute the stored procedure OneListSearch
-- =============================================
-- EXECUTE dbo.OneListSearch 'Mxx','week shoppi',99   -- 'milk skim'         


-- =============================================
-- NotificationSearch
-- =============================================
IF EXISTS (
  SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'NotificationSearch' 
)
   DROP PROCEDURE dbo.NotificationSearch
GO
CREATE PROCEDURE dbo.NotificationSearch
    @contactId nvarchar(20),@search nvarchar(200), @top int
AS
   --@search.  Spaces in @search are treated like "AND". 'milk skim' becomes 'milk AND skim'

   SET NOCOUNT ON;
    --check that at least one char is passed in as string
    If len(@search)<1 or @search is null  
    Begin
      SELECT n.Id, n.Type, n.TypeCode, n.PrimaryText, n.SecondaryText, n.DisplayFrequency, n.ValidFrom, n.ValidTo, n.Created, n.LastModifiedDate, n.CreatedBy, 
            n.DateLastModified, n.QRText, n.NotificationType, n.Status, ISNULL(nl.ContactId, N'') AS ContactId, ISNULL(nl.DeviceId, N'') AS DeviceId, ISNULL(nl.DateDisplayed, 
            N'2000-01-01') AS DateDisplayed, ISNULL(nl.DateClosed, N'2000-01-01') AS DateClosed, ISNULL(nl.ReplicationCounter, 0) AS ReplicationCounter, 
            ISNULL(nl.NotificationStatus, 0) AS NotificationStatus 
            FROM dbo.Notification AS n 
            LEFT OUTER JOIN dbo.NotificationLog AS nl ON n.Id = nl.Id WHERE 1 = 0
      return
    End
    
    --need to check if full text is supported, azure does not support it
    DECLARE @idx int  = 1     
    DECLARE @minSearchLength int  = 2
    DECLARE @slice nvarchar(200) = N''  
    DECLARE @delimiter nchar(1)  = N' '   
    DECLARE @tempsearch nvarchar(500) = N''
    DECLARE @sql nvarchar(1100) = N''  
    DECLARE @searchIn nvarchar(200) = LTrim(RTrim(@search)) 
    SET @search = REPLACE(@search,'''','''''')
    SET @search = REPLACE(@search,'*','') --dont allow star. used in full text search
    
    SET @searchIn = REPLACE(@searchIn,'''','''''')
    set @search = LTrim(RTrim(@search))
    --- BEGIN --- --- ---
    --Code taken from NotificationGetByContactId
    --collect the notification IDs in a variable table
    Create table #tempnotification
    ( 
        Id nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL 
    )

    insert into #tempnotification(Id) 
    SELECT n.Id from [Notification] n inner join Contact c on n.TypeCode = c.Id
    where n.Type = 1 --contact  
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')  and  (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01' ELSE ISNULL(n.ValidTo,'2200-01-01') END)
    and n.Id not in (select Id COLLATE DATABASE_DEFAULT from #tempnotification)   
   
    insert into #tempnotification(Id)
    SELECT n.Id from [Notification] n inner join Contact c on n.TypeCode = c.AccountId
    where n.Type = 0 --account
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01' ELSE ISNULL(n.ValidTo,'2200-01-01') END)
    and n.Id not in (select Id  COLLATE DATABASE_DEFAULT from #tempnotification)
        
    insert into #tempnotification(id)
    SELECT n.Id from [Notification] n inner join Account a on n.TypeCode = a.SchemeId
      inner join Contact c on a.Id = c.AccountId
    where n.Type = 3 --scheme
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01' ELSE ISNULL(n.ValidTo,'2200-01-01') END) 
    and n.Id not in (select Id  COLLATE DATABASE_DEFAULT from #tempnotification) 
    
    insert into #tempnotification(Id)
    SELECT n.Id from [Notification] n inner join Scheme s on n.TypeCode = s.ClubId
        inner join Account a on a.SchemeId = s.Id
        inner join Contact c on a.Id = c.AccountId
    where n.Type = 2 --club
    and c.Id = @contactId
    and GETDATE() between ISNULL(n.ValidFrom,'2000-01-01')   and  (CASE  WHEN YEAR(n.ValidTo) = 1900 THEN '2200-01-01' ELSE ISNULL(n.ValidTo,'2200-01-01') END)
    and n.Id not in (select Id  COLLATE DATABASE_DEFAULT from #tempnotification) 
      
    --now we have the notifications for this contact
    --- END --- --- ---        
    Begin
      -- 'No Fulltext column'  
      --Spaces in @search are treated like AND.  'milk skim' needs to be converted to 'milk AND skim'
      --check if I need to replace spaces with AND.
      --This will do a full table scan, so always use Fulltext search when possible
        while @idx!= 0       
        begin       
            set @idx = charindex(@delimiter,@search);      
            if @idx!=0       
                set @slice = left(@search,@idx - 1);
            else       
                set @slice = @search;
              
            if(len(@slice)>= @minSearchLength and len(@tempsearch)=0)  
                set @tempsearch = 'like N''%' + ltrim(rtrim(@slice)) + '%''' ;
            else if(len(@slice)>= @minSearchLength)  
                set @tempsearch = @tempsearch + ' AND PrimaryText LIKE N''%' + ltrim(rtrim(@slice)) + '%'' ' ;
                      
            set @search = right(@search,len(@search) - @idx);
            set @search = ltrim(rtrim(@search))
            if len(@search) = 0 break;    
        end  
 
      if len(@search)=0 
        set @sql = N'SELECT n.Id, n.Type, n.TypeCode, n.PrimaryText, n.SecondaryText, n.DisplayFrequency, n.ValidFrom, n.ValidTo, n.Created, n.LastModifiedDate, n.CreatedBy, 
            n.DateLastModified, n.QRText, n.NotificationType, n.Status, ISNULL(nl.ContactId, N'') AS ContactId, ISNULL(nl.DeviceId, N'') AS DeviceId, ISNULL(nl.DateDisplayed, 
			' + N'2000-01-01' + ') AS DateDisplayed, ISNULL(nl.DateClosed, ' + N'2000-01-01' + ') AS DateClosed, ISNULL(nl.ReplicationCounter, 0) AS ReplicationCounter, 
            ISNULL(nl.NotificationStatus, 0) AS NotificationStatus
            FROM dbo.Notification AS n 
            LEFT OUTER JOIN dbo.NotificationLog AS nl ON n.Id = nl.Id WHERE 1=0' --return empty row, Select statement = old notificationview 
      else
      begin 
        set @sql = N'SELECT TOP(' + convert(nvarchar(20),@top) +') '
        set @sql = @sql + N' n.[Id], n.[Type],n.[TypeCode],n.[PrimaryText],n.[SecondaryText],n.[DisplayFrequency] ' 
        set @sql = @sql + N' ,n.[ValidFrom],n.[ValidTo],n.[Created],n.[CreatedBy],n.[LastModifiedDate],n.[DateLastModified] ' 
        set @sql = @sql + N' ,n.[QRText],n.[NotificationType] ,isnull(nl.ContactId,'''') As ContactId,isnull(nl.DeviceId,'''') As DeviceId '
        set @sql = @sql + N' ,isnull(nl.DateDisplayed,''2000-01-01'') As DateDisplayed,isnull(nl.DateClosed,''2000-01-01'') As DateClosed '
        set @sql = @sql + N' ,isnull(nl.ReplicationCounter,0) As ReplicationCounter ' 
        set @sql = @sql + N' ,isnull(nl.NotificationStatus,0) as NotificationStatus ' 
        set @sql = @sql + N'  ' 
        set @sql = @sql + N' from [Notification] n inner join #tempnotification tn on tn.Id = n.Id COLLATE DATABASE_DEFAULT  '
        set @sql = @sql + N' left outer join [NotificationLog] nl on n.Id = nl.Id  and nl.ContactId =  N''' + @contactId + ''' '
        set @sql = @sql + N' WHERE (PrimaryText ' + @tempsearch
        set @sql = @sql + N' OR SecondaryText like N''%' + @searchIn + '%''  )'
        set @sql = @sql + N'  '
        set @sql = @sql + N' and tn.id  COLLATE DATABASE_DEFAULT not in (select Id from NotificationLog nl where ContactId = N''' + @contactId + ''' and NotificationStatus = 2)'
        set @sql = @sql + N' ORDER BY PrimaryText'
      end
 
      --   select @sql
      EXECUTE( @sql)  
    End

GO
-- =============================================
-- Example to execute the stored procedure NotificationSearch
-- =============================================
-- EXECUTE dbo.NotificationSearch 'MO000001','cd',99   

 
-- =============================================
-- ActivityLogSave
-- =============================================
IF EXISTS (
  SELECT * FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'ActivityLogSave' 
)
   DROP PROCEDURE dbo.ActivityLogSave
GO
CREATE PROCEDURE dbo.ActivityLogSave
    @solution nchar(1),@type nchar(2),@typeValue nvarchar(50),@deviceId nvarchar(50),@securityToken nvarchar(50) ,@ipAddress nvarchar(50)
AS
    /*

    */
    SET NOCOUNT ON;
        
    --get 
    DECLARE @contactId nvarchar(50)
    if  LEN(@securityToken) > 0 
    begin 
        select @contactId=ContactId 
            FROM DeviceSecurity (NOLOCK)   
        where SecurityToken  = @securityToken 
    end
    if (@solution is null OR  LEN(@solution) = 0)
    begin
        set @solution = N'L' --default to loy
    end
 
    INSERT INTO [ActivityLog]
                ([Solution]
                ,[Type]
                ,[TypeValue]
                ,[ContactId]
                ,[DeviceId]
                ,[IPAddress]
                ,[DateCreated])
            VALUES
                (@solution
                ,ISNULL(@type,N'')
                ,ISNULL(@typeValue,N'')
                ,ISNULL(@contactId,N'')
                ,ISNULL(@deviceId,N'')
                ,ISNULL(@ipAddress,N'')
                ,SYSDATETIME())
GO
-- =============================================
-- Example to execute the stored procedure Login
-- =============================================
-- EXECUTE dbo.ActivityLogSave @solution='L',@type='IT',@typeValue='40020',@deviceId ='mydevice',@ipAddress='172.2.2.2',
--   @securityToken='17571081759F415C83B69F3F9D63B83B|14-11-26 14:26:29'    select * from ActivityLog
 

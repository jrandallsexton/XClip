SELECT * FROM [XClip].[dbo].[BatchSources]

  --delete from BatchSources
  --DBCC CHECKIDENT ('[BatchSources]', RESEED, 0);

--  declare @SourceId int

--select top 1 @SourceId = BI.[Id]
--FROM [BatchSources] BI
--WHERE
--	(BI.Reviewed IS NULL) AND
--	(BI.Deleted IS NULL) AND
--	(BI.Skipped IS NULL) AND
--	(BI.FileExt = '.mp4')
--ORDER BY NEWID()

--select BS.Id, BS.[Filename], BS.[FileExt], BS.[Filesize], BS.[Created]
--from [BatchSources] BS
--where BS.Id = @SourceId

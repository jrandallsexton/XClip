--select * from dbo.BatchSources bs where bs.CollectionId = 2
select * from dbo.Collections c

--delete from dbo.collections
--DBCC CHECKIDENT ('dbo.collections', RESEED, 0)
--insert into dbo.collections (userId, Name, created) values (1, 'Test Collection', getutcdate())
--insert into dbo.collections (userId, Name, created) values (2, 'Drone Footage', getutcdate())
--select * from dbo.Collections c

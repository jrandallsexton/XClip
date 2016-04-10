DELETE FROM BatchSourcesTags
delete from Tags
DBCC CHECKIDENT ('[Tags]', RESEED, 0);
DBCC CHECKIDENT ('[BatchSourcesTags]', RESEED, 0);
  delete from BatchSources
  DBCC CHECKIDENT ('[BatchSources]', RESEED, 0);
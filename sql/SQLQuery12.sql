--SELECT
--	BS.[Filename],
--	T.[Text] AS [Tag]
--FROM [XClip].[dbo].[BatchSources] BS
--INNER JOIN BatchSourcesTags BST ON BST.BatchSourceId = BS.Id
--INNER JOIN Tags T ON T.Id = BST.TagId
--ORDER BY BS.[Filename], T.[Text]

SELECT T.Id, T.[Text]
FROM Tags T
INNER JOIN [BatchSourcesTags] BST ON BST.TagId = T.Id
INNER JOIN [BatchSources] BS ON BS.Id = BST.BatchSourceId
WHERE BS.UId = '3356CC63-6392-46DE-A1A8-CBF0D588F4B3'
ORDER BY T.[Text]
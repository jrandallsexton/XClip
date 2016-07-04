
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using TMCP.Core.Data;

namespace XClip.Repositories
{
    public class TagRepository : RepositoryBase
    {
        public Dictionary<int, string> Tags(int userId, int? collectionId)
        {
            var sql = string.Empty;

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UserId", userId)
            };

            if (collectionId.HasValue)
            {
                sql = "select t.[id], t.[Text] from [tags] t where t.[UserId] = @UserId and t.[CollectionId] = @CollectionId order by t.[text]";
                paramList.Add(new SqlParameter("CollectionId", collectionId.Value));
            }
            else
            {
                sql = "select t.[id], t.[Text] from [tags] t where t.[UserId] = @UserId order by t.[text]";
            }

            var values = new Dictionary<int, string>();

            using (var rdr = new SqlBaseDal().OpenDataReaderInLine(sql, paramList))
            {
                if ((rdr == null) || (!rdr.HasRows))
                    return values;

                while (rdr.Read())
                {
                    values.Add(rdr.GetInt32(0), rdr.GetString(1));
                }
            }

            return values;

        }

        public Dictionary<int, string> GetTags(Guid mediaId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT T.Id, T.[Text]");
            sql.AppendLine("FROM Tags T");
            sql.AppendLine("INNER JOIN [BatchSourcesTags] BST ON BST.TagId = T.Id");
            sql.AppendLine("INNER JOIN [BatchSources] BS ON BS.Id = BST.BatchSourceId");
            sql.AppendLine("WHERE BS.UId = @UId");
            sql.AppendLine("ORDER BY T.[Text]");

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UId", mediaId)
            };

            var values = new Dictionary<int, string>();

            using (var rdr = new SqlBaseDal().OpenDataReaderInLine(sql.ToString(), paramList))
            {
                if ((rdr == null) || (!rdr.HasRows))
                    return values;

                while (rdr.Read())
                {
                    values.Add(rdr.GetInt32(0), rdr.GetString(1));
                }
            }

            return values;
        }

        public IEnumerable<string> GetBlacklist()
        {
            var values = new List<string>();

            using (var rdr = new SqlBaseDal().OpenDataReaderInLine("select t.[Text] from [tagsIgnore] t order by t.[text]", new List<SqlParameter>()))
            {
                if ((rdr == null) || (!rdr.HasRows))
                    return values;

                while (rdr.Read())
                {
                    values.Add(rdr.GetString(0));
                }
            }

            return values;
        }

        public int Save(string tagText)
        {
            const string sql = "INSERT INTO [Tags] ([Text]) VALUES (@Tag) SELECT SCOPE_IDENTITY()";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("Tag", tagText)
            };
            return base.ExecuteIdentity(sql, paramList);
        }

        public void AddTag(int sourceId, int tagId)
        {
            const string sql = "INSERT INTO [BatchSourcesTags] ([BatchSourceId], [TagId]) VALUES (@BatchSourceId, @TagId)";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("BatchSourceId", sourceId),
                new SqlParameter("TagId", tagId)
            };
            base.ExecuteInLineSql(sql, paramList);
        }

        public bool TagExists(int sourceId, int tagId)
        {
            const string sql = "SELECT COUNT(*) FROM [BatchSourcesTags] WHERE [BatchSourceId] = @BatchSourceId AND [TagId] = @TagId";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("BatchSourceId", sourceId),
                new SqlParameter("TagId", tagId)
            };
            var count = new SqlBaseDal().ExecuteScalarInLine(sql, paramList);
            return count > 0;
        }

    }
}
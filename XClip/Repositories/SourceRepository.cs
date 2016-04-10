using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMCP.Core.Data;
using XClip.DataObjects;

namespace XClip.Repositories
{
    public class SourceRepository : RepositoryBase
    {
        public int Save(int collectionId, XSource source)
        {
            const string select =
                "SELECT COUNT([Id]) FROM [BatchSources] WHERE [CollectionId] = @CollectionId AND [FileName] = @Filename AND [FileExt] = @FileExt AND [Filesize] = @FileSize AND [Filedate] = @FileDate";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("CollectionId", collectionId),
                new SqlParameter("Filename", source.FName),
                new SqlParameter("FileExt", source.FExt),
                new SqlParameter("FileSize", source.FSize),
                new SqlParameter("FileDate", source.FDate)
            };

            var id = new SqlBaseDal().ExecuteScalarInLine(select, paramList);

            if (id > 0)
                return id;

            var paramList2 = new List<SqlParameter>
            {
                new SqlParameter("CollectionId", collectionId),
                new SqlParameter("Filename", source.FName),
                new SqlParameter("FileExt", source.FExt),
                new SqlParameter("FileSize", source.FSize),
                new SqlParameter("FileDate", source.FDate)
            };
            const string sql = "INSERT INTO [BatchSources] ([CollectionId], [Filename], [FileExt], [Filesize], [Filedate]) VALUES (@CollectionId, @Filename, @FileExt, @FileSize, @FileDate) SELECT SCOPE_IDENTITY()";

            return base.ExecuteIdentity(sql, paramList2);
        }

        public Guid UId(int collectionId, int sourceId)
        {
            const string sql = "SELECT [UId] FROM [BatchSources] WHERE [Id] = @Id AND [CollectionId] = @CollectionId";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("CollectionId", collectionId),
                new SqlParameter("Id", sourceId)
            };

            return new SqlBaseDal().ExecuteScalarGuidInLine(sql, paramList);
        }

        public void MarkAsSkipped(Guid sourceId)
        {
            const string sql = "UPDATE [BatchSources] SET [Skipped] = @GetUtcDate() WHERE [UId] = @UId";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UId", sourceId)
            };
            base.ExecuteInLineSql(sql, paramList);
        }
    }
}
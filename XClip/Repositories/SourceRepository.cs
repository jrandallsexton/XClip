using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TMCP.Core.Data;
using XClip.DataObjects;

namespace XClip.Repositories
{
    public interface ISourceRepository
    {
        int Save(int collectionId, XSource source);
        Guid UId(int collectionId, int sourceId);
        void MarkAsSkipped(Guid sourceId);
        void MarkAsDeleted(Guid sourceId);
        XSource Get(Guid uId);
    }

    public class SourceRepository : RepositoryBase, ISourceRepository
    {
        private readonly IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["xclipDb"].ConnectionString);

        public int Save(int collectionId, XSource source)
        {
            const string select =
                "SELECT COUNT([Id]) FROM [BatchSources] WHERE [CollectionId] = @CollectionId AND [FileName] = @Filename AND [FileExt] = @FileExt AND [Filesize] = @FileSize AND [Filedate] = @FileDate";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("CollectionId", collectionId),
                new SqlParameter("Filename", source.Filename),
                new SqlParameter("FileExt", source.FileExt),
                new SqlParameter("FileSize", source.FileSize),
                new SqlParameter("FileDate", source.FileDate)
            };

            var id = new SqlBaseDal().ExecuteScalarInLine(select, paramList);

            if (id > 0)
                return id;

            var paramList2 = new List<SqlParameter>
            {
                new SqlParameter("UId", source.UId),
                new SqlParameter("CollectionId", collectionId),
                new SqlParameter("Filename", source.Filename),
                new SqlParameter("FileExt", source.FileExt),
                new SqlParameter("FileSize", source.FileSize),
                new SqlParameter("FileDate", source.FileDate)
            };
            const string sql = "INSERT INTO [BatchSources] ([UId], [CollectionId], [Filename], [FileExt], [Filesize], [Filedate]) VALUES (@UId, @CollectionId, @Filename, @FileExt, @FileSize, @FileDate) SELECT SCOPE_IDENTITY()";

            return base.ExecuteIdentity(sql, paramList2);
        }

        public Guid UId(int collectionId, int sourceId)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("CollectionId", collectionId),
                new SqlParameter("Id", sourceId)
            };

            return new SqlBaseDal().ExecuteScalarGuidInLine(Queries.XSourceGetUId, paramList);
        }

        public void MarkAsSkipped(Guid sourceId)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UId", sourceId)
            };
            base.ExecuteInLineSql(Queries.XSourceMarkSkipped, paramList);
        }

        public void MarkAsDeleted(Guid sourceId)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UId", sourceId)
            };
            base.ExecuteInLineSql(Queries.XSourceMarkDeleted, paramList);
        }

        public XSource Get(Guid uId)
        {
            var sql =
                $"SELECT [Id], [UId], [CollectionId], [Filename], [FileExt], [FileSize], [FileDate], [Created], [Reviewed], [Deleted], [Skipped], [SaveSource] FROM [BatchSources] WHERE [UId] = '{uId.ToString()}'";

            return _db.Query<XSource>(sql).FirstOrDefault();
        }
    }
}
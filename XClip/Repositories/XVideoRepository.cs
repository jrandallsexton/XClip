
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

using XClip.DataObjects;

namespace XClip.Repositories
{
    public interface IXVideoRepository
    {
        void Save(int userId, XVideo xVideo);
        XVideo Get(Guid uId);
    }

    public class XVideoRepository : RepositoryBase, IXVideoRepository
    {
        public void Save(int userId, XVideo xVideo)
        {
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO dbo.[XVideos] ([UId], [Name], [Description])");
            sql.AppendLine("VALUES (@UId, @Name, @Desc)");
            sql.AppendLine("SELECT SCOPE_IDENTITY()");

            var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@UId", xVideo.UId),
                    new SqlParameter("@Name", xVideo.Name),
                    new SqlParameter("@Desc", xVideo.Description)
                };

            xVideo.Id = base.ExecuteIdentity(sql.ToString(), paramList);
        }

        public XVideo Get(Guid uId)
        {
            const string sql = "SELECT [Id], [Name], [Description] FROM dbo.[XVideos] WHERE [UId] = @UId";

            var paramList = new List<SqlParameter> { new SqlParameter("@UId", uId) };

            using (var rdr = base.OpenDataReaderInLine(sql, paramList))
            {
                if (rdr == null)
                    return null;

                var video = new XVideo
                {
                    UId = uId,
                    Id = rdr.GetInt32(0),
                    Name = rdr.GetString(1)
                };

                if (!rdr.IsDBNull(2))
                    video.Description = rdr.GetString(2);

                return video;
            }
        }
    }
}
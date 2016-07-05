
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XClip.DataObjects;

namespace XClip.Repositories
{
    public class XVideoItemRepository : RepositoryBase
    {
        public void Save(int videoId, XVideoItem item)
        {
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO dbo.[XVideoItems] ([UId], [XVideoId], [BatchItemId], [Index])");
            sql.AppendLine("VALUES (@UId, @XVideoId, @BatchItemId, @Index");
            sql.AppendLine("SELECT SCOPE_IDENTITY()");

            var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@UId", item.UId),
                    new SqlParameter("@XVideoId", videoId),
                    new SqlParameter("@BatchItemId", item.XBatchItemId),
                    new SqlParameter("@Index", item.Index)
                };

            item.Id = base.ExecuteIdentity(sql.ToString(), paramList);
        }

        public List<XVideoItem> GetVideoItems(int xVideoId)
        {
            var values = new List<XVideoItem>();

            var sql = "SELECT [UId], [BatchItemId], [Index] FROM dbo.[XVideoItems] XVI WHERE XVI.[Id] = @Id";

            var paramList = new List<SqlParameter> { new SqlParameter("@Id", xVideoId) };

            using (var rdr = base.OpenDataReaderInLine(sql, paramList))
            {
                if (rdr == null)
                    return values;

                while (rdr.Read())
                {
                    var item = new XVideoItem
                    {
                        UId = rdr.GetGuid(0),
                        XBatchItemId = rdr.GetInt32(1),
                        Index = rdr.GetInt32(2)
                    };
                    values.Add(item);
                }
            }

            return values;
        }
    }
}
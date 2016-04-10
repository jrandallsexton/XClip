using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMCP.Core.Data;
using XClip.DataObjects;

namespace XClip.Repositories
{
    public class BatchRepository : SqlBaseDal
    {
        public bool FileExists(string fName)
        {

            const string sql = "SELECT COUNT(*) FROM [BatchSources] WHERE [Filename] = @Fname";

            var paramList = new List<SqlParameter> { new SqlParameter("@Fname", fName) };

            return base.ExecuteScalarInLine(sql, paramList) > 0;

        }

        public bool Save(string fName, string fExt, long fileSize, DateTime created)
        {
            const string sql = "INSERT INTO [BatchSources] ([Filename], [FileExt], [Filesize], [Filedate]) VALUES (@Fname, @FExt, @FSize, @FDate)";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@Fname", fName),
                new SqlParameter("@FExt", fExt),
                new SqlParameter("@FSize", fileSize),
                new SqlParameter("@FDate", created)
            };

            return base.ExecuteInLineSql(sql, paramList);

        }

        public XSource RandomSource(int collectionId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("declare @SourceId int");
            sb.AppendLine("select top 1 @SourceId = BI.[Id]");
            sb.AppendLine("FROM [BatchSources] BI");
            sb.AppendLine("WHERE (BI.Reviewed IS NULL) AND");
            sb.AppendLine("(BI.Deleted IS NULL) AND");
            sb.AppendLine("(BI.Skipped IS NULL) AND");
            sb.AppendLine("(BI.FileExt = '.mp4') AND");
            sb.AppendLine("(BI.CollectionId = @CollectionId)");
            sb.AppendLine("ORDER BY NEWID()");
            sb.AppendLine("SELECT BS.Id, BS.UId, BS.[Filename], BS.[FileExt], BS.[Filesize], BS.[Created]");
            sb.AppendLine("from [BatchSources] BS");
            sb.AppendLine("where BS.Id = @SourceId");

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("CollectionId", collectionId)
            };

            using (var rdr = base.OpenDataReaderInLine(sb.ToString(), paramList))
            {
                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                rdr.Read();

                return new XSource(rdr.GetInt32(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt64(4), rdr.GetDateTime(5));
            }

        }

        public KeyValuePair<Guid, string> Source(Guid id)
        {
            var sql = "SELECT [Filename] FROM [BatchSources] WHERE [Id] = '" + id.ToString() + "'";

            using (var rdr = base.OpenDataReaderInLine(sql, new List<SqlParameter>()))
            {
                if ((rdr == null) || (!rdr.HasRows)) { return new KeyValuePair<Guid, string>(Guid.Empty, string.Empty); }

                rdr.Read();

                return new KeyValuePair<Guid, string>(id, rdr.GetString(0));
            }
        }

        private bool SaveBatchItemTags(Guid batchItemId, List<int> tagIds)
        {
            const string sql = "INSERT INTO [BatchItemsTags] ([ItemId], [TagId]) VALUES ('{0}', {1})";

            foreach (var i in tagIds)
            {
                base.ExecuteInLineSql(string.Format(sql, batchItemId, i), new List<SqlParameter>());
            }

            return true;
        }

        public bool BatchItemsSave(Guid batchId, List<XBatchItem> items)
        {

            const string sql = "INSERT INTO [BatchItems] ([Id], [BatchId], [Index], [StartTime], [StopTime], [Duration]) VALUES (@Id, @BatchId, @Index, @Start, @Stop, @Dur)";

            var i = 0;

            foreach (var ci in items)
            {

                var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@Id", ci.Id),
                    new SqlParameter("@BatchId", batchId),
                    new SqlParameter("@Index", i),
                    new SqlParameter("@Start", ci.Start),
                    new SqlParameter("@Stop", ci.Stop),
                    new SqlParameter("@Dur", ci.Duration)
                };

                base.ExecuteInLineSql(sql, paramList);
                this.SaveBatchItemTags(ci.Id, ci.Tags);

                i++;

            }

            return true;

        }

        public bool BatchInfoSave(XBatch batch)
        {
            const string sql = "INSERT INTO [Batches] ([Id], [SourceId], [OutputMask]) VALUES (@Id, @SrcId, @OM)";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@Id", batch.Id),
                new SqlParameter("@SrcId", batch.SrcId),
                new SqlParameter("@OM", batch.OutputMask)
            };

            if (this.ExecuteInLineSql(sql, paramList))
            {
                if (this.BatchItemsSave(batch.Id, batch.Items))
                {
                    return this.MarkSourceAsReviewed(batch.SrcId);
                }
            }

            return false;

        }

        public bool MarkSourceAsReviewed(Guid id)
        {
            const string sql = "UPDATE [BatchSources] SET [Reviewed] = GetDate() WHERE [Id] = @Id";
            var paramList = new List<SqlParameter> { new SqlParameter("@Id", id) };

            return base.ExecuteInLineSql(sql, paramList);
        }

        public bool MarkSourceAsDeleted(Guid id)
        {
            const string sql = "UPDATE [BatchSources] SET [Deleted] = GetDate() WHERE [Id] = @Id";
            var paramList = new List<SqlParameter> { new SqlParameter("@Id", id) };

            return base.ExecuteInLineSql(sql, paramList);
        }

        private List<XBatchItem> GetBatchItems(Guid batchId)
        {
            var values = new List<XBatchItem>();

            var sql = new StringBuilder();
            sql.AppendLine("select BI.[Id], BI.[Index], BI.StartTime, BI.StopTime, BI.Duration");
            sql.AppendLine("from BatchItems BI");
            sql.AppendLine("where BI.BatchId = @Id");
            sql.AppendLine("order by bi.[Index]");

            var paramList = new List<SqlParameter> { new SqlParameter("@Id", batchId) };

            using (var rdr = base.OpenDataReaderInLine(sql.ToString(), paramList))
            {

                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                while (rdr.Read())
                {
                    var info = new XBatchItem
                    {
                        Id = rdr.GetGuid(0),
                        Index = rdr.GetInt32(1),
                        Start = rdr.GetString(2),
                        Stop = rdr.GetString(3)
                    };
                    info.Duration = (decimal.Parse(info.Stop) - decimal.Parse(info.Start)).ToString(CultureInfo.InvariantCulture);

                    values.Add(info);
                }

            }

            return values;
        }

        public XBatch Get(Guid id)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT B.[Id], bsrc.[Filename], b.[OutputMask]");
            sql.AppendLine("FROM [Batches] B");
            sql.AppendLine("inner join [BatchSources] BSrc on bSrc.Id = B.SourceId ");
            sql.AppendLine("where B.[Id] = @Id");

            var paramList = new List<SqlParameter> { new SqlParameter("@Id", id) };

            XBatch info = null;

            using (var rdr = base.OpenDataReaderInLine(sql.ToString(), paramList))
            {

                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                rdr.Read();

                info = new XBatch
                {
                    Id = id,
                    SrcId = rdr.GetGuid(0),
                    Filename = rdr.GetString(1),
                    OutputMask = rdr.GetString(2)
                };

            }

            info.Items = this.GetBatchItems(info.Id);

            return info;

        }

        public void BatchStart(Guid id)
        {
            var sql = string.Format("UPDATE [Batches] SET [ProcStart] = GetDate() WHERE [Id] = '{0}'", id);
            base.ExecuteInLineSql(sql, new List<SqlParameter>());
        }

        public void BatchEnd(Guid id)
        {
            var sql = string.Format("UPDATE [Batches] SET [Completed] = GetDate(), [ProcEnd] = GetDate() WHERE [Id] = '{0}'", id);
            base.ExecuteInLineSql(sql, new List<SqlParameter>());
        }

        public List<Guid> openBatchIds()
        {
            const string sql = "SELECT [Id] FROM [Batches] WHERE [Completed] IS NULL ORDER BY [Created]";

            var ids = new List<Guid>();

            using (var rdr = base.OpenDataReaderInLine(sql, new List<SqlParameter>()))
            {
                while (rdr.Read())
                {
                    ids.Add(rdr.GetGuid(0));
                }
            }

            return ids;
        }

        public Dictionary<Guid, string> AllOpenSources()
        {
            var values = new Dictionary<Guid, string>();

            const string sql = "select BS.Id, BS.[Filename] from [BatchSources] BS WHERE BS.[Reviewed] IS NULL AND BS.[Deleted] IS NULL AND BS.[Skipped] IS NULL ORDER BY BS.[Created] DESC";

            using (var rdr = base.OpenDataReaderInLine(sql, new List<SqlParameter>()))
            {
                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                while (rdr.Read())
                {
                    values.Add(rdr.GetGuid(0), rdr.GetString(1));
                }
            }

            return values;
        }
    }
}


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
    public class BatchRepository : RepositoryBase
    {

        private readonly SqlBaseDal _dal = new SqlBaseDal();

        public bool FileExists(string fName)
        {

            var paramList = new List<SqlParameter> { new SqlParameter("@Fname", fName) };

            return _dal.ExecuteScalarInLine(Queries.FileExists, paramList) > 0;

        }

        public XSource RandomSource(int collectionId)
        {

            var paramList = new List<SqlParameter> { new SqlParameter("CollectionId", collectionId) };

            using (var rdr = _dal.OpenDataReaderInLine(Queries.RandomSource, paramList))
            {
                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                rdr.Read();

                return new XSource(rdr.GetInt32(0), rdr.GetGuid(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt64(4), rdr.GetDateTime(5));
            }

        }

        public KeyValuePair<Guid, string> Source(Guid id)
        {
            var sql = "SELECT [Filename] FROM [BatchSources] WHERE [Id] = '" + id.ToString() + "'";

            using (var rdr = _dal.OpenDataReaderInLine(sql, new List<SqlParameter>()))
            {
                if ((rdr == null) || (!rdr.HasRows)) { return new KeyValuePair<Guid, string>(Guid.Empty, string.Empty); }

                rdr.Read();

                return new KeyValuePair<Guid, string>(id, rdr.GetString(0));
            }
        }

        private bool SaveBatchItemTags(int batchItemId, IEnumerable<int> tagIds)
        {
            const string sql = "INSERT INTO [BatchItemsTags] ([ItemId], [TagId]) VALUES ({0}, {1})";

            foreach (var i in tagIds)
            {
                base.ExecuteInLineSql(string.Format(sql, batchItemId, i), new List<SqlParameter>());
            }

            return true;
        }

        public bool BatchItemsSave(int batchId, List<XBatchItem> items)
        {

            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO [BatchItems] ([UId], [BatchId], [Index], [StartTime], [StopTime], [Duration])");
            sql.AppendLine("VALUES (@UId, @BatchId, @Index, @Start, @Stop, @Dur)");
            sql.AppendLine("SELECT SCOPE_IDENTITY()");

            var i = 0;

            foreach (var batchItem in items)
            {

                var paramList = new List<SqlParameter>
                {
                    new SqlParameter("@UId", batchItem.UId),
                    new SqlParameter("@BatchId", batchId),
                    new SqlParameter("@Index", i),
                    new SqlParameter("@Start", batchItem.Start),
                    new SqlParameter("@Stop", batchItem.Stop),
                    new SqlParameter("@Dur", batchItem.Duration)
                };

                var batchItemId = base.ExecuteIdentity(sql.ToString(), paramList);
                this.SaveBatchItemTags(batchItemId, batchItem.Tags);

                i++;

            }

            return true;

        }

        public bool BatchInfoSave(XBatch batch)
        {
            const string sql = "INSERT INTO [Batches] ([UId], [SourceId], [OutputMask]) VALUES (@UId, @SrcId, @OM) SELECT SCOPE_IDENTITY()";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UId", batch.UId),
                new SqlParameter("@SrcId", batch.SrcId),
                new SqlParameter("@OM", batch.OutputMask)
            };

            batch.Id = base.ExecuteIdentity(sql, paramList);
            return this.BatchItemsSave(batch.Id, batch.Items) && this.MarkSourceAsReviewed(batch.SrcId);
        }

        public void MarkStarted(Guid batchUId)
        {
            const string sql = "UPDATE dbo.[Batches] SET [ProcStart] = GetUtcDate() WHERE [UId] = @UId";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UId", batchUId)
            };

            base.ExecuteInLineSql(sql, paramList);
        }

        public void MarkCompleted(Guid batchUId, DateTime end)
        {
            var sql = new StringBuilder();
            sql.AppendLine("BEGIN TRANSACTION");
            sql.AppendLine("DECLARE @BatchId int");
            sql.AppendLine("SELECT @BatchId FROM dbo.[Batches] B WHERE B.[UId] = @UId");
            sql.AppendLine("UPDATE dbo.[BatchItems] SET [Completed] = GetUtcDate() WHERE [BatchId] = @BatchId");
            sql.AppendLine("UPDATE dbo.[Batches] SET [ProcEnd] = @End, [Completed] = GetUtcDate() WHERE [UId] = @UId");
            sql.AppendLine("COMMIT TRANSACTION");

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("@End", end),
                new SqlParameter("@UId", batchUId)
            };

            base.ExecuteInLineSql(sql.ToString(), paramList);
        }

        public bool MarkSourceAsReviewed(Guid id)
        {
            const string sql = "UPDATE [BatchSources] SET [Reviewed] = GetDate() WHERE [UId] = @UId";
            var paramList = new List<SqlParameter> { new SqlParameter("@UId", id) };

            return base.ExecuteInLineSql(sql, paramList);
        }

        public bool MarkSourceAsDeleted(Guid id)
        {
            const string sql = "UPDATE [BatchSources] SET [Deleted] = GetDate() WHERE [Id] = @Id";
            var paramList = new List<SqlParameter> { new SqlParameter("@Id", id) };

            return base.ExecuteInLineSql(sql, paramList);
        }

        private List<XBatchItem> GetBatchItems(int batchId)
        {
            var values = new List<XBatchItem>();

            var sql = new StringBuilder();
            sql.AppendLine("select BI.[Id], BI.[UId], BI.[Index], BI.StartTime, BI.StopTime, BI.Duration");
            sql.AppendLine("from BatchItems BI");
            sql.AppendLine("where BI.BatchId = @Id");
            sql.AppendLine("order by bi.[Index]");

            var paramList = new List<SqlParameter> { new SqlParameter("@Id", batchId) };

            using (var rdr = _dal.OpenDataReaderInLine(sql.ToString(), paramList))
            {

                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                while (rdr.Read())
                {
                    var info = new XBatchItem
                    {
                        Id = rdr.GetInt32(0),
                        UId = rdr.GetGuid(1),
                        Index = rdr.GetInt32(2),
                        Start = rdr.GetString(3),
                        Stop = rdr.GetString(4)
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
            sql.AppendLine("SELECT B.[Id], B.SourceId, bsrc.[Filename], b.[OutputMask]");
            sql.AppendLine("FROM [Batches] B");
            sql.AppendLine("inner join [BatchSources] BSrc on bSrc.UId = B.SourceId ");
            sql.AppendLine("where B.[UId] = @UId");

            var paramList = new List<SqlParameter> { new SqlParameter("@UId", id) };

            XBatch info = null;

            using (var rdr = _dal.OpenDataReaderInLine(sql.ToString(), paramList))
            {

                if ((rdr == null) || (!rdr.HasRows)) { return null; }

                rdr.Read();

                info = new XBatch
                {
                    Id = rdr.GetInt32(0),
                    UId = id,
                    SrcId = rdr.GetGuid(1),
                    Filename = rdr.GetString(2),
                    OutputMask = rdr.GetString(3)
                };

            }

            info.Items = this.GetBatchItems(info.Id);

            return info;

        }

        public void BatchStart(Guid id)
        {
            var sql = $"UPDATE [Batches] SET [ProcStart] = GetDate() WHERE [Id] = '{id}'";
            base.ExecuteInLineSql(sql, new List<SqlParameter>());
        }

        public void BatchEnd(Guid id)
        {
            var sql = $"UPDATE [Batches] SET [Completed] = GetDate(), [ProcEnd] = GetDate() WHERE [Id] = '{id}'";
            base.ExecuteInLineSql(sql, new List<SqlParameter>());
        }

        public List<Guid> OpenBatchIds(int collectionId)
        {
            const string sql = "SELECT b.[UId] FROM [XClip].[dbo].[Batches] B inner join dbo.BatchSources bs on bs.UId = b.SourceId where bs.CollectionId = @CollectionId and b.completed is null order by b.created";

            var ids = new List<Guid>();

            var paramList = new List<SqlParameter> { new SqlParameter("@CollectionId", collectionId) };

            using (var rdr = _dal.OpenDataReaderInLine(sql, paramList))
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

            using (var rdr = _dal.OpenDataReaderInLine(sql, new List<SqlParameter>()))
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
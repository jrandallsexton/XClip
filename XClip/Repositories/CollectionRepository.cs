using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMCP.Core.Data;

namespace XClip.Repositories
{
    public class CollectionRepository : RepositoryBase
    {
        public int Create(int userId, string name)
        {
            const string sql = "INSERT INTO [Collections] ([UserId], [Name]) VALUES (@UserId, @Name) SELECT SCOPE_IDENTITY()";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UserId", userId),
                new SqlParameter("Name", name)
            };
            return base.ExecuteIdentity(sql, paramList);
        }

        public void Delete(int id, int userId)
        {
            const string sql =
                "UPDATE [Collections] SET [Deleted] = GetUtcDate() WHERE [Id] = @Id AND [UserId] = @UserId";

            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UserId", userId),
                new SqlParameter("Id", id)
            };

            base.ExecuteInLineSql(sql, paramList);
        }
    }
}
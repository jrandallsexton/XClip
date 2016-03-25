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
            const string sql = "INSERT INTO [Collections] ([UserId], [Name]) VALUES (@UserId, @Name)";
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UserId", userId),
                new SqlParameter("Name", name)
            };
            return base.ExecuteIdentity(sql, paramList);
        }
    }
}
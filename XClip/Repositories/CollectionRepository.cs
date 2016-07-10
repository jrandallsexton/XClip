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
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UserId", userId),
                new SqlParameter("Name", name)
            };
            return base.ExecuteIdentity(Queries.XCollectionInsert, paramList);
        }

        public void Delete(int id, int userId)
        {
            var paramList = new List<SqlParameter>
            {
                new SqlParameter("UserId", userId),
                new SqlParameter("Id", id)
            };

            base.ExecuteInLineSql(Queries.XCollectionMarkDeleted, paramList);
        }
    }
}
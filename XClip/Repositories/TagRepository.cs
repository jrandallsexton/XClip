using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMCP.Core.Data;

namespace XClip.Repositories
{
    public class TagRepository : SqlBaseDal
    {
        public Dictionary<int, string> Tags()
        {

            var values = new Dictionary<int, string>();

            using (var rdr = base.OpenDataReaderInLine("select t.[id], t.[Text] from [tags] t order by t.[text]", new List<SqlParameter>()))
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
    }
}

using RegistShp.Util;
using System;
using System.Collections.Generic;
using System.Data;

namespace RegistShp.Dao
{
    class CountryDao
    {
        private CountryDao() { }

        public static int Drop()
        {
            string sql = "DROP TABLE country";
            return DbUtils.Execute(sql);
        }

        public static int UpdateVirtualShp(string path)
        {
            string sql = string.Format("CREATE VIRTUAL TABLE country USING VirtualShape('{0}', 'utf-8', 4326)", path);
            return DbUtils.Execute(sql);
        }

        public static List<long> FindAllCodeNew()
        {
            List<long> codeList = new List<long>();

            string sql = "SELECT code_new from country";
            try
            {
                DataTable table = DbUtils.Query(sql);
                foreach (DataRow row in table.Rows)
                {
                    codeList.Add((long)row["code_new"]);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return codeList;
        }
    }
}

using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace HealthCheckSample.Services.Util
{
    public class MSSqlDBService
    {
        private int defaultTimeout = 300;//預設timeout時間
        private ConfigService _configService;

        public MSSqlDBService(IServiceProvider service)
        {
             _configService = service.GetService<ConfigService>();
        }

        #region ConnectionString 

        /// <summary>
        /// Get Setting DB ConStr
        /// </summary>
        /// <param name="databasename"></param>
        /// <returns></returns>
        public string GetDefaultConnectionString(string databasename = "")
        {
            string ConnectionString = "";
 
            switch(databasename)
            {
                default:
                    ConnectionString = _configService.GetUserDBConString();
                    break;
            }

            return ConnectionString;
        }
        #endregion

        public class PageInfo
        {
            /// <summary>
            /// 每頁行數 預設10
            /// </summary>
            public int pageSize { get; set; } = 10;

            /// <summary>
            /// 預設第一頁
            /// </summary>
            public int pageIndex { get; set; } = 1;

            /// <summary>
            /// 總筆數
            /// </summary>
            public int count { get; set; } = 0;

            /// <summary>
            /// 總頁數
            /// </summary>
            public int pageCount
            {
                get
                {
                    if(count > 0)
                    {
                        return count % this.pageSize == 0 ? count / this.pageSize : count / this.pageSize + 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// MSSql 分頁查詢
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public dynamic GetPageList(string sql, string orderBy, int pageIndex, int pageSize, string ConStr = "")
        {
            #region 使用範例
            //var total = 0；
            //var re = GetPageList<User>("SELECT * FROM [TestDB].[dbo].[User]", "[CreatedOn] desc, [ID] asc", 1, 20, out total);
            #endregion
            int skip = 1;
            if(pageIndex > 0)
            {
                skip = (pageIndex - 1) * pageSize + 1;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM ({0}) AS Result;", sql);
            sb.AppendFormat(@"SELECT *
                        FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY {1}) AS RowNum
                             FROM  ({0}) AS Temp) AS Result
                        WHERE  RowNum >= {2} AND RowNum <= {3}
                        ORDER BY {1}", sql, orderBy, skip, pageIndex * pageSize);

            using(IDbConnection conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                using(var reader = conn.QueryMultiple(sb.ToString()))
                {
                    return new
                    {
                        totalCount = reader.ReadFirst<int>(),
                        data = reader.Read<dynamic>()
                    };
                }
            }
        }

        /// <summary>
        /// MSsql異步執行 可做新修刪
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>bool</returns>
        public async Task<bool> MSsqlExecuteAsync(string sql, DynamicParameters dynamics, string ConStr = "")
        {

            #region example : 新增範例
            //string sql =   @"
            //                INSERT INTO Card 
            //                (
            //                    [Name]
            //                   ,[Description]
            //                   ,[Attack]
            //                   ,[Health]
            //                   ,[Cost]
            //                ) 
            //                VALUES 
            //                (
            //                    @Name
            //                   ,@Description
            //                   ,@Attack
            //                   ,@Health
            //                   ,@Cost
            //                );

            //                SELECT @@IDENTITY;
            //            ";
            #endregion
            #region example : 修改範例
            //string sql =@"
            //        UPDATE Card
            //        SET 
            //             [Name] = @Name
            //            ,[Description] = @Description
            //            ,[Attack] = @Attack
            //            ,[Health] = @Health
            //            ,[Cost] = @Cost
            //        WHERE 
            //            Id = @id
            //        ";
            //var parameters = new DynamicParameters(parameter);
            //parameters.Add("Id", id, System.Data.DbType.Int32);
            #endregion
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    conn.Open();

                    var result = await conn.ExecuteAsync(sql, dynamics);
                    conn.Close();
                    return result > 0;
                }
            }
            catch(Exception e)
            {

                throw;
            }
        }

        /// <summary>
        /// MSsql同步執行 可做新修刪
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>bool</returns>
        public bool MSsqlExecute(string sql, DynamicParameters dynamics, string ConStr = "")
        {
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = conn.Execute(sql, dynamics);
                conn.Close();
                return result > 0;
            }
        }

        /// <summary>
        /// MSsql異步執行 可做新修刪 多筆
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public async Task<int> MSsqlExecuteMultipleAsync(string sql, List<DynamicParameters> list, string ConStr = "")
        {

            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = await conn.ExecuteAsync(sql, list);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql同步執行 可做新修刪 多筆
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public int MSsqlExecuteMultiple(string sql, List<DynamicParameters> list, string ConStr = "")
        {

            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = conn.Execute(sql, list);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql 取第一筆異步查詢
        /// </summary>
        /// <param name="sql">查詢字串</param>
        /// <param name="parameters">動態參數設置</param>
        /// <returns>IEnumerable<dynamic></returns>
        public async Task<dynamic> MSsqlQueryFirstOrDefaultAsync(string sql, DynamicParameters parameters, string ConStr = "")
        {
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(sql, parameters, commandTimeout: defaultTimeout);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql 取第一筆異步查詢
        /// </summary>
        /// <param name="sql">查詢字串</param>
        /// <param name="parameters">動態參數設置</param>
        /// <returns>IEnumerable<dynamic></returns>
        public async Task<T> MSsqlQueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters, string ConStr = "")
        {
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = await conn.QueryFirstOrDefaultAsync<T>(sql, parameters, commandTimeout: defaultTimeout);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql 取第一筆同步查詢
        /// </summary>
        /// <param name="sql">查詢字串</param>
        /// <param name="parameters">動態參數設置</param>
        /// <returns>IEnumerable<dynamic></returns>
        public dynamic MSsqlQueryFirstOrDefault(string sql, DynamicParameters parameters, string ConStr = "")
        {
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = conn.QueryFirstOrDefault<dynamic>(sql, parameters, commandTimeout: defaultTimeout);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql 取第一筆同步查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public T MSsqlQueryFirstOrDefault<T>(string sql, DynamicParameters parameters, string ConStr = "")
        {
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = conn.QueryFirstOrDefault<T>(sql, parameters, commandTimeout: defaultTimeout);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql 多筆異步查詢
        /// </summary>
        /// <param name="sql">查詢字串</param>
        /// <param name="parameters">動態參數設置</param>
        /// <returns>IEnumerable<dynamic></returns>
        public async Task<IEnumerable<dynamic>> MSsqlQueryAsync(string sql, DynamicParameters parameters, string ConStr = "")
        {
            #region example :
            // string sql = @"select * from User where id = ?"
            // parameters.Add("id", id);
            #endregion
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                try
                {
                    conn.Open();
                    var result = await conn.QueryAsync(sql, parameters, commandTimeout: defaultTimeout);
                    conn.Close();
                    return result.AsEnumerable();
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                    return null;
                }
            }
        }

        /// <summary>
        /// MSsql 多筆異步查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> MSsqlQueryAsync<T>(string sql, DynamicParameters parameters, string ConStr = "")
        {
            #region example :
            // string sql = @"select * from User where id = ?"
            // parameters.Add("id", id);
            #endregion
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                try
                {
                    conn.Open();
                    var result = await conn.QueryAsync<T>(sql, parameters, commandTimeout: defaultTimeout);
                    conn.Close();
                    return result.AsEnumerable();
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                    return null;
                }
            }
        }


        /// <summary>
        /// MSsql 多筆同步查詢
        /// </summary>
        /// <param name="sql">查詢字串</param>
        /// <param name="parameters">動態參數設置</param>
        /// <returns>IEnumerable<dynamic></returns>
        public IEnumerable<dynamic> MSsqlQuery(string sql, DynamicParameters parameters, string ConStr = "")
        {
            #region example :
            // string sql = @"select * from User where id = ?"
            // parameters.Add("id", id);
            #endregion
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = conn.Query(sql, parameters, commandTimeout: defaultTimeout);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// MSsql 多筆同步查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public IEnumerable<T> MSsqlQuery<T>(string sql, DynamicParameters parameters, string ConStr = "")
        {
            #region example :
            // string sql = @"select * from User where id = ?"
            // parameters.Add("id", id);
            #endregion
            using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
            {
                conn.Open();
                var result = conn.Query<T>(sql, parameters, commandTimeout: defaultTimeout);
                conn.Close();
                return result;
            }
        }

        /// <summary>
        ///  MSsql 單筆異步查詢: return a single object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public async Task<dynamic> MSsqlQuerySingleAsync(string sql, DynamicParameters parameters, string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    conn.Open();
                    var result = await conn.QuerySingleAsync<dynamic>(sql, parameters, commandTimeout: defaultTimeout);
                    conn.Close();
                    return result;
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// MSsql 單筆異步查詢: return a single object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public async Task<T> MSsqlQuerySingleAsync<T>(string sql, DynamicParameters parameters, string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    conn.Open();
                    var result = await conn.QuerySingleAsync<T>(sql, parameters, commandTimeout: defaultTimeout);
                    conn.Close();
                    return result;
                }
            }
            catch(Exception)
            {

                throw;
            }
        }


        /// <summary>
        ///  MSsql 單筆同步查詢: return a single object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public dynamic MSsqlQuerySingle(string sql, DynamicParameters parameters, string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    conn.Open();
                    var result = conn.QuerySingle<dynamic>(sql, parameters, commandTimeout: defaultTimeout);
                    conn.Close();
                    return result;
                }
            }
            catch(Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// MSsql 單筆同步查詢: return a single object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public T MSsqlQuerySingle<T>(string sql, DynamicParameters parameters, string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    conn.Open();
                    var result = conn.QuerySingle<T>(sql, parameters, commandTimeout: defaultTimeout);
                    conn.Close();
                    return result;
                }
            }
            catch(Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// MSsql 同步查詢: return a DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public DataTable MSsqlQueryDataTable(string sql, DynamicParameters parameters, string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    using(var comm = new SqlCommand(sql, conn))
                    {
                        conn.Open();

                        comm.Parameters.Add(parameters);

                        using(var reader = comm.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                DataTable dt = new DataTable();
                                dt.Load(reader);
                                conn.Close();
                                return dt;
                            }
                            return null;
                        }

                    }
                }
            }
            catch(Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// MSsql 異步查詢: return a DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public DataTable MSsqlQueryDataTableAsync(string sql, DynamicParameters parameters, string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    using(var comm = new SqlCommand(sql, conn))
                    {
                        conn.Open();

                        comm.Parameters.Add(parameters);

                        using(var reader = comm.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                DataTable dt = new DataTable();
                                dt.Load(reader);
                                conn.Close();
                                return dt;
                            }
                            return null;
                        }

                    }
                }
            }
            catch(Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 確認DB是否連線
        /// </summary>
        /// <param name="ConStr"></param>
        /// <returns></returns>
        public bool IsDBConnected(string ConStr = "")
        {
            try
            {
                using(var conn = new SqlConnection(GetDefaultConnectionString(ConStr)))
                {
                    conn.Open();
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}

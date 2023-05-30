using HealthCheckSample.Models;
using HealthCheckSample.Services.Util;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using static HealthCheckSample.Controllers.UserController;

namespace HealthCheckSample.Services
{
    /// <summary>
    /// 關於 使用者 table的相關方法
    /// </summary>
    public class UserService
    {
        private readonly MSSqlDBService mSSqlDBService;
        private readonly ILogger logger;

        public UserService(ILogger<UserService> logger, IServiceProvider service)
        {
            this.logger = logger;
            this.mSSqlDBService = service.GetService<MSSqlDBService>();
        }

        #region Get
        /// <summary>
        /// 取得一位使用者資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserVM> GetSingleUserAsync(int id)
        {
            UserVM user = new UserVM();
            try
            {
                string sql = $@"SELECT TOP (1) * FROM dbo.[User] u WHERE u.id =@id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id);

                user = await mSSqlDBService.MSsqlQueryFirstOrDefaultAsync<UserVM>(sql, parameters);
            }
            catch(Exception ex)
            {
                logger.LogError("{method} Error: {exMessage}", nameof(GetSingleUserAsync), ex.Message);
                user = null;
            }
            return user;
        }

        /// <summary>
        ///  取得所有使用者資料
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserVM>> GetAllUserAsync()
        {
            IEnumerable<UserVM> users = new List<UserVM>();
            try
            {
                string sql = $@"SELECT * FROM dbo.[User]";
                DynamicParameters parameters = new DynamicParameters();

                users = await mSSqlDBService.MSsqlQueryAsync<UserVM>(sql, parameters);
            }
            catch(Exception ex)
            {
                logger.LogError("{method} Error: {exMessage}", nameof(GetAllUserAsync), ex.Message);
                users = null;
            }

            return users;
        }
    #endregion

    #region Insert
    /// <summary>
    /// 新增一位使用者
    /// </summary>
    /// <param name="requset"></param>
    /// <returns></returns>
    public async Task<int> AddUserAsync(UserCreateRequset requset)
    {
        int id = 0;
        try
        {
            string sql = $@"INSERT INTO [dbo].[User](account, password, enable, create_id, create_time)  VALUES(@account, @password, @enable, @create_id, @create_time); SELECT SCOPE_IDENTITY() as id";

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("account", requset.account);
            parameters.Add("password", requset.password);
            parameters.Add("enable", requset.enable);
            parameters.Add("create_id", 0); //因是範例，建立者id請設為0
            parameters.Add("create_time", DateTime.Now);

            var row = await mSSqlDBService.MSsqlQuerySingleAsync(sql, parameters);
            id = Convert.ToInt16(row.id);
        }
        catch(Exception ex)
        {
            //將error 寫成結構化
            //var errorLog = new { method = nameof(UserService), exMessage = ex.Message };
            //logger.LogError("{@errorLog}", errorLog);

            logger.LogError("{method} Error: {exMessage}", nameof(AddUserAsync), ex.Message);
        }

        return id;
    }

    #endregion

    #region Update
    /// <summary>
    /// 更新一位使用者
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<bool> UpdateUserAsync(UserUpdateRequest request)
    {
        bool isSuccess = false;

        try
        {
            string sql = $@"UPDATE [dbo].[User] 
SET password=@password, enable =@enable, update_id =@update_id, update_time=@update_time 
WHERE id=@id";

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("password", request.password);
            parameters.Add("enable", request.enable);
            parameters.Add("update_id", 0); //因是範例，建立者id請設為0
            parameters.Add("update_time", DateTime.Now);
            parameters.Add("id", request.id);

            isSuccess = await mSSqlDBService.MSsqlExecuteAsync(sql, parameters);
        }
        catch(Exception ex)
        {
            logger.LogError("{method} Error: {exMessage}", nameof(UpdateUserAsync), ex.Message);
        }

        return isSuccess;
    }
    #endregion

    #region Delete
    /// <summary>
    /// 刪除一位使用者
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteUserAsync(int id)
    {
        bool isSuccess = false;

        try
        {
            string sql = $@"DELETE FROM [dbo].[User]  WHERE id = @id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("id", id);

            isSuccess = await mSSqlDBService.MSsqlExecuteAsync(sql, parameters);
        }
        catch(Exception ex)
        {
            logger.LogError("{method} Error: {exMessage}", nameof(DeleteUserAsync), ex.Message);
        }

        return isSuccess;
    }
    #endregion
}
}

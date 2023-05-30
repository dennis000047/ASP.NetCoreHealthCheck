using HealthCheckSample.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using HealthCheckSample.Models;

namespace HealthCheckSample.Controllers
{
    [EnableCors("AllowAny")]
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private readonly UserService userService;

        public UserController(IServiceProvider service)
        {
            this.userService = service.GetService<UserService>();
        }

        /// <summary>
        /// 取得單筆User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = nameof(Get))]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(int id)
        {
            var user = await userService.GetSingleUserAsync(id);

            if(user == null)
                return NotFound(new { message = "找不到使用者" });

            return Ok(user);
        }

        /// <summary>
        /// 取得所有User
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = nameof(GetAsync))]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAsync()
        {
            var users = await userService.GetAllUserAsync();

            if(users == null)
                return NotFound(new { message = "找不到使用者" });

            return Ok(users);
        }
 
        public class UserVM
        {
            public string account { get; set; }
            public string enable { get; set; }
            public string create_id { get; set; }
            public string create_time { get; set; }
        }

        /// <summary>
        /// UserCreate Request
        /// </summary>
        public class UserCreateRequset
        {
            public string account { get; set; }
            public string password { get; set; }
            public int enable { get; set; }
        }

        /// <summary>
        /// 新增一名User
        /// </summary>
        /// <param name="requset"></param>
        /// <returns></returns>
        [HttpPost(Name = nameof(Create))]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(UserCreateRequset requset)
        {
            var id = await userService.AddUserAsync(requset);

            if(id == 0)
                return Problem("新增使用者失敗", statusCode: 500);
                // another expression:  return StatusCode(500, new { message = "新增使用者失敗" })
            else
                return Ok(new { message = "新增使用者成功" });
            }

        /// <summary>
        /// UserUpdate Request
        /// </summary>
        public class UserUpdateRequest
        {
            public int id { get; set; }
            public string password { get; set; }
            public string confirmPassword { get; set; }
            public int enable { get; set; }
        }

        /// <summary>
        /// 編輯一位User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch(Name = nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(UserUpdateRequest request)
        {
            var user = await userService.GetSingleUserAsync(request.id);
            if(user == null)
                return NotFound(new { message = "找不到使用者" });

            var isSuccess = await userService.UpdateUserAsync(request);
            if(!isSuccess)
                return Problem("編輯使用者失敗", statusCode: 500);

            return Ok(new { message = "編輯使用者成功" });
        }

        /// <summary>
        /// 刪除一位User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete(Name = nameof(Delete))]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await  userService.GetSingleUserAsync(id);

            if(user == null)
                return NotFound(new { message = "找不到使用者" });

            var isSuccess = await userService.DeleteUserAsync(id);
            if(!isSuccess)
                return Problem("刪除使用者失敗", statusCode: 500);

            return Ok(new { message = "刪除使用者成功" });
        }
    }
}

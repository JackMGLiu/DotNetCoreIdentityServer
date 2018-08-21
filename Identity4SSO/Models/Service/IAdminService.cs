using System.Threading.Tasks;

namespace Identity4SSO.Models.Service
{
    public interface IAdminService
    {
        /// <summary>
        /// 根据用户名和密码查找用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="pwd">用户密码</param>
        /// <returns></returns>
        Task<Admin> GetAdminModel(string username, string pwd);
    }
}
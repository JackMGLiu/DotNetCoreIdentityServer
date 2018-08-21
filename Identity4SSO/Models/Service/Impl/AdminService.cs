using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Identity4SSO.Models.Service.Impl
{
    public class AdminService: IAdminService
    {
        public EFContext _db;
        public AdminService(EFContext db)
        {
           this._db = db;
        }

        public async Task<Admin> GetAdminModel(string username, string pwd)
        {
            var model = await _db.Admin.SingleOrDefaultAsync(m => m.UserName == username && m.Password == pwd);
            if (model == null)
            {
                return null;
            }
            else
            {
                return model;
            }
        }
    }
}
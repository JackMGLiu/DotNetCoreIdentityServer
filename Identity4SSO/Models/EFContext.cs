using Microsoft.EntityFrameworkCore;

namespace Identity4SSO.Models
{
    public class EFContext : DbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {

        }

        #region 实体集

        public DbSet<Admin> Admin { get; set; }//注意 这里这个Admin不能写成Admins否则会报错找不到Admins 因为我们现在数据库和表是现成的 这里就相当于实体对应的数据库是Admin

        #endregion
    }
}
using EntityFrameworkCore.Extensions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace EntityFrameworkCore.Extensions
{
    public class BaseDBContext : DbContext
    {
        private IPropertyAutoProvider _provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public BaseDBContext([NotNull] DbContextOptions options) : base(options)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            // 关闭DbContext默认事务
            Database.AutoTransactionsEnabled = false;
        }

        /// <summary>
        /// 重写 SaveChanges 方法
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().ToList();

            PropertyAutoProvider(entities);

            return base.SaveChanges();
        }

        /// <summary>
        /// 重写 SaveChanges 方法
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries().ToList();

            PropertyAutoProvider(entities);

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        private void PropertyAutoProvider(List<EntityEntry> entities)
        {
            foreach (var item in entities)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        foreach (var property in item.Properties)
                            _provider.InsertIntercept(new ColumnProperty(property));
                        break;
                    case EntityState.Modified:
                        foreach (var property in item.Properties)
                            _provider.UpdateIntercept(new ColumnProperty(property));
                        break;
                    case EntityState.Deleted:
                        foreach (var property in item.Properties)
                            _provider.DeleteIntercept(new ColumnProperty(property));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

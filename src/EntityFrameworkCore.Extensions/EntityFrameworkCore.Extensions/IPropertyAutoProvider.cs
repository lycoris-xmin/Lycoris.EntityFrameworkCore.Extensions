using EntityFrameworkCore.Extensions.Models;

namespace EntityFrameworkCore.Extensions
{
    public interface IPropertyAutoProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        void InsertIntercept(ColumnProperty property);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void UpdateIntercept(ColumnProperty property);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void DeleteIntercept(ColumnProperty property);
    }
}

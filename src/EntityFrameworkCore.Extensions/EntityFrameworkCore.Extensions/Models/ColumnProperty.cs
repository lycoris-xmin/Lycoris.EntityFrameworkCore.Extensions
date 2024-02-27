using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace EntityFrameworkCore.Extensions.Models
{
    public class ColumnProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyEntry PropertyEntry { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PropertyEntry"></param>
        public ColumnProperty(PropertyEntry PropertyEntry)
        {
            this.PropertyEntry = PropertyEntry;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type Type { get => this.PropertyEntry.Metadata.ClrType; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get => this.PropertyEntry.Metadata.Name; }

        /// <summary>
        /// 
        /// </summary>
        public object? Value { get => this.PropertyEntry.CurrentValue; set => this.PropertyEntry.CurrentValue = value; }

        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo? PropertyInfo { get => this.PropertyEntry.Metadata.PropertyInfo; }

        /// <summary>
        /// 
        /// </summary>
        public FieldInfo? FieldInfo { get => this.PropertyEntry.Metadata.FieldInfo; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsModified { get => this.PropertyEntry.IsModified; set => this.PropertyEntry.IsModified = value; }
    }
}

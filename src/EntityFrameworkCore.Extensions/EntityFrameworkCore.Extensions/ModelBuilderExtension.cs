﻿using EntityFrameworkCore.Extensions.Attributes;
using EntityFrameworkCore.Extensions.ColumnConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using System.Xml.XPath;

namespace EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="context"></param>
        public static void TableAutoBuilder(this ModelBuilder builder, DbContext context) => builder.TableAutoBuilder(context.GetType().Assembly);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        public static void TableAutoBuilder(this ModelBuilder builder, Assembly assembly)
        {

        }


        /// <summary>
        /// 表列处理
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="p"></param>
        /// <param name="column"></param>
        /// <param name="className"></param>
        /// <param name="navigator"></param>
        /// <param name="assembly"></param>
        private static void TableColumnAutoBuilder(this PropertyBuilder propertyBuilder, PropertyInfo? p, TableColumnAttribute column, string? className, XPathNavigator navigator, Assembly assembly)
        {
            if (p == null)
                return;

            // 主键自增
            if (column.IsIdentity)
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(long))
                    propertyBuilder.ValueGeneratedOnAdd();
            }
            else if (column.IsRowVersion)
                propertyBuilder.IsRowVersion();

            // 映射数据库列类型
            if (!string.IsNullOrEmpty(column.ColumnType))
                propertyBuilder.HasColumnType(column.ColumnType);

            // 字符串长度限制
            if (column.StringLength > 0 && p.PropertyType == typeof(string))
                propertyBuilder.HasMaxLength(column.StringLength);

            // 必填
            if (column.Required)
                propertyBuilder.IsRequired();

            // 主键不做以下处理
            if (!column.IsPrimary)
            {
                // 默认值
                if (column.DefaultValue != null)
                {
                    if (p.PropertyType == typeof(bool))
                        propertyBuilder.HasDefaultValue(column.DefaultValue).ValueGeneratedNever();
                    else
                        propertyBuilder.HasDefaultValue(column.DefaultValue);
                }
                else if (string.IsNullOrEmpty(column.ColumnType))
                {
                    if (p.PropertyType == typeof(string) && column.StringLength > 0)
                        propertyBuilder.HasDefaultValue("");
                    else if (new Type[] { typeof(int), typeof(long), typeof(ushort), typeof(uint) }.Contains(p.PropertyType))
                        propertyBuilder.HasDefaultValue(0);
                    else if (p.PropertyType == typeof(bool))
                        propertyBuilder.HasDefaultValue(false).ValueGeneratedNever();
                    else if (p.PropertyType == typeof(Enum))
                        propertyBuilder.HasDefaultValue(0);
                    else if (p.PropertyType == typeof(DateTime))
                        propertyBuilder.HasDefaultValue(new DateTime(2000, 1, 1));
                }

                if (column.JsonMap)
                    propertyBuilder.HasColumnType("json");
                else if (column.SqlPassword)
                    propertyBuilder.HasConversion<SqlPasswrodConverter>();
                else if (column.Sensitive)
                    propertyBuilder.HasConversion<SqlSensitiveConverter>();
            }

            // 注释（列注释）
            var memberName = $"P:{className}.{p.Name}";
            var summaryNode = navigator.SelectSingleNode($"/doc/members/member[@name='{memberName}']/summary");

            var comment = summaryNode?.InnerXml.Trim();
            comment ??= "";

            if (p.PropertyType.IsEnum)
            {
                var fields = p.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Static);

                comment += "：";

                var _navigator = assembly != p.PropertyType.Assembly ? GetOtherAssemblySummary(p.PropertyType) : navigator;

                foreach (var item in fields)
                {
                    // 列枚举注释
                    // 由于枚举值存放位置可能不在当前程序集，所以如果当前程序集没有找到列
                    var value = item.GetValue(null);
                    summaryNode = _navigator.SelectSingleNode($"/doc/members/member[@name='F:{p.PropertyType.FullName}.{value}']/summary");

                    comment += $"{(int)value!}-{summaryNode?.InnerXml.Trim() ?? ""},";
                }

                comment = comment.TrimEnd(',');
            }

            propertyBuilder.HasComment(comment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static XPathNavigator GetOtherAssemblySummary(Type propertyType)
        {
            var xmlDoc = new XPathDocument(Path.Combine(AppContext.BaseDirectory, $"{propertyType.Assembly.GetName().Name}.xml"));
            return xmlDoc.CreateNavigator();
        }
    }
}

namespace EntityFrameworkCore.Extensions.Attributes
{
    /// <summary>
    /// 雪花编号
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SnowflakeAttribute : Attribute
    {

    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace Reacher.Data.Models;
public class EnumTable<TEnum> where TEnum : struct
{
    protected EnumTable() { }
    private EnumTable(TEnum v)
    {
        Id = v;
    }
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public TEnum Id { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }

    public static implicit operator EnumTable<TEnum>(TEnum enumType) => new(enumType);
    public static implicit operator TEnum(EnumTable<TEnum> status) => status.Id;
}
public static class EnumTableExtensions
{
    public static List<TEnum> GetEnumValues<TEnum>() where TEnum : struct
    {
        var values = Enum.GetValues(typeof(TEnum))
                           .Cast<object>()
                           .Select(value => (TEnum)value);
        return values.ToList();
    }
    private static T Construct<T>()
    {
        Type t = typeof(T);

        ConstructorInfo ci = t.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null, new Type[0], null);

        return (T)ci.Invoke(new object[0]);
    }
    public static void SeedEnumValues<TEnum>(this EntityTypeBuilder<EnumTable<TEnum>> builder) where TEnum : struct
    {
        var enumValues = GetEnumValues<TEnum>();
        var values = enumValues.ConvertAll(temp =>
        {
            var name = temp.GetEnumDescription() ?? temp.ToString();
            var instance = Construct<EnumTable<TEnum>>();
            instance.Id = temp;
            instance.Name = name;
            return instance;
        });
        builder.HasData(values);
    }
    public static string GetEnumDescription<TEnum>(this TEnum item) => item.GetType()
       .GetField(item.ToString())
       .GetCustomAttributes(typeof(DescriptionAttribute), false)
       .Cast<DescriptionAttribute>()
       .FirstOrDefault()?.Description;

    public static void ThrowIfNotEnum<TEnum>()
        where TEnum : struct
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new Exception($"Invalid generic method argument of type {typeof(TEnum)}");
        }
    }
}

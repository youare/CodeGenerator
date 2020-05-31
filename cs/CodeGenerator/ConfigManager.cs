using CodeGenerator.Enums;
using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace CodeGenerator
{
    public interface IConfigManager
    {
        AppConfig AppConfig { get; }
    }
    public class ConfigManager : IConfigManager
    {
        public ConfigManager()
        {
            var schema = new XmlSchemaSet();
            schema.Add("http://dd.com/code_generator.xsd", $"{Constants.ConfigFileName}.xsd");
            var rd = XmlReader.Create($"{Constants.ConfigFileName}.xml");
            var doc = XDocument.Load(rd);
            doc.Validate(schema, (s, e) =>
            {
                if (e?.Exception != null) throw e.Exception;
            });
            AppConfig = GetConfig<AppConfig>(doc.Root);
        }

        private T GetConfig<T>(XElement rootElement)
        {
            var configInstance = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == prop.Name);
                if (string.IsNullOrEmpty(element?.Value)) continue;
                if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    var value = ConvertValue(prop, configInstance, element?.Value);
                    prop.SetValue(configInstance, value);
                }
                else
                {
                    var isList = prop.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
                    if (isList)
                    {
                        var listArgumentType = prop.PropertyType.GetGenericArguments()[0];
                        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(listArgumentType));
                        var listItems = element.Elements().ToList();
                        foreach (var listItem in listItems)
                        {
                            if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                            {
                                var result = ConvertValue(prop, configInstance, element?.Value);
                                list.GetType().GetMethod("Add").Invoke(list, new[] { result });
                            }
                            else
                            {
                                var result = GetType().GetMethod(nameof(GetConfig), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(listArgumentType).Invoke(this, new[] { listItem });
                                list.GetType().GetMethod("Add").Invoke(list, new[] { result });
                            }
                        }
                        prop.SetValue(configInstance, list);
                    }
                    else
                    {
                        var result = GetType().GetMethod(nameof(GetConfig), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(prop.PropertyType).Invoke(this, new[] { element });
                        prop.SetValue(configInstance, result);
                    }
                }
            }
            return configInstance;
        }
        private object ConvertValue(PropertyInfo property, object source, string value)
        {
            if (property.PropertyType == typeof(TimeSpan)) return TimeSpan.Parse(value);
            else if (property.PropertyType == typeof(int)) return Convert.ToInt32(value);
            else if (property.PropertyType == typeof(decimal)) return Convert.ToDecimal(value);
            else if (property.PropertyType == typeof(DateTime)) return DateTime.ParseExact(value, Constants.DateFormat, CultureInfo.InvariantCulture);
            else if (property.PropertyType == typeof(bool)) return Convert.ToBoolean(value);
            else if (property.Name == "EntityEntryType")
            {
                return value.ToEnum<EntityEntryTypeEnum>().Value;
            }
            else if (property.Name == "EntityRelationType") return value.ToEnum<EntityRelationTypeEnum>().Value;
            else return value;
        }
        public AppConfig AppConfig { get; private set; }
    }
    public class AppConfig
    {
        public string ServiceName { get; set; }
        public string ServerRootFolder { get; set; }
        public string JsRootFolder { get; set; }
        public string JsConstantPrefix { get; set; }
        public EntityConfig EntityConfig { get; set; }
        public DaoConfig DaoConfig { get; set; }
        public ValueObjectConfig ValueObjectConfig { get; set; }
        public AggregateConfig AggregateConfig { get; set; }
        public DtoConfig DtoConfig { get; set; }
        public ReaderConfig ReaderConfig { get; set; }
        public WriterConfig WriterConfig { get; set; }
        public QueryConfig QueryConfig { get; set; }
        public CommandConfig CommandConfig { get; set; }
        public AggregateFactoryConfig AggregateFactoryConfig { get; set; }
        public ControllerConfig ControllerConfig { get; set; }
    }
    public class EntityEntry
    {
        public string Name { get; set; }
        public EntityEntryTypeEnum EntityEntryType { get; set; }
        public string CSharpType
        {
            get
            {
                switch (EntityEntryType)
                {
                    case EntityEntryTypeEnum.Int: return Nullable? "int?":"int";
                    case EntityEntryTypeEnum.Long: return Nullable ? "long?" : "long";
                    case EntityEntryTypeEnum.String: return "string";
                    case EntityEntryTypeEnum.Date: return Nullable ? "DateTime?" : "DateTime";
                    case EntityEntryTypeEnum.DateTime: return Nullable ? "DateTime?" : "DateTime";
                    case EntityEntryTypeEnum.Decimal: return Nullable ? "decimal?" : "decimal";
                    case EntityEntryTypeEnum.Double: return Nullable ? "double?" : "double";
                    case EntityEntryTypeEnum.Guid: return Nullable ? "guid?" : "guid";
                    case EntityEntryTypeEnum.Bool: return Nullable ? "bool?" : "bool";
                    default: return "string";
                }
            }
        }
        public EntityRelationTypeEnum EntityRelationType { get; set; }
        public bool Nullable { get; set; }
        public string DbLine => $"{Name.FromPascalToSnakeCase()} {DbType} {DbIsNull} {DbEntityRelation}";

        private string DbIsNull => Nullable ? "" : "not null";
        private string DbEntityRelation { get
            {
                switch (EntityRelationType)
                {
                    case EntityRelationTypeEnum.Primary: return "primary key";
                    default: return "";
                }
            } }
        private string DbType
        {
            get
            {
                if (EntityRelationType == EntityRelationTypeEnum.Primary)
                {
                    switch (EntityEntryType)
                    {
                        case EntityEntryTypeEnum.Int: return "serial";
                        case EntityEntryTypeEnum.Long: return "bigserial";
                        case EntityEntryTypeEnum.String: return "text";
                        case EntityEntryTypeEnum.Guid: return "uuid";
                        default: return "text";
                    }
                }
                switch (EntityEntryType)
                {
                    case EntityEntryTypeEnum.Int: return "int";
                    case EntityEntryTypeEnum.Long: return "bigint";
                    case EntityEntryTypeEnum.String: return "text";
                    case EntityEntryTypeEnum.Date: return "date";
                    case EntityEntryTypeEnum.DateTime: return "timestamp";
                    case EntityEntryTypeEnum.Bool: return "boolean";
                    case EntityEntryTypeEnum.Guid: return "uuid";
                    default: return "text";
                }
            }
        }

    }
    public class EntityConfig
    {
        public string Name { get; set; }
        public List<EntityEntry> EntityEntryList { get; set; }
    }

    public class DaoConfig
    {
        public string NamespacePrefix { get; set; }
        public string ParentName { get; set; }
    }

    public class ValueObjectConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class AggregateConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class DtoConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class ReaderConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class WriterConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class QueryConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class CommandConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class AggregateFactoryConfig
    {
        public string NamespacePrefix { get; set; }
    }
    public class ControllerConfig
    {
        public string NamespacePrefix { get; set; }
    }
}

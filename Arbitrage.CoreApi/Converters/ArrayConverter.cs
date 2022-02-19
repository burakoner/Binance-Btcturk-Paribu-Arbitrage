using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Arbitrage.CoreApi.Converters
{
    /// <summary>
    /// Converter for arrays to properties
    /// </summary>
    public class ArrayConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(JToken))
            {
                return JToken.Load(reader);
            }

            object result = Activator.CreateInstance(objectType);
            JArray arr = JArray.Load(reader);
            return ParseObject(arr, result, objectType);
        }

        private static object ParseObject(JArray arr, object result, Type objectType)
        {
            foreach (PropertyInfo property in objectType.GetProperties())
            {
                ArrayPropertyAttribute attribute =
                    (ArrayPropertyAttribute)property.GetCustomAttribute(typeof(ArrayPropertyAttribute));
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Index >= arr.Count)
                {
                    continue;
                }

                if (property.PropertyType.BaseType == typeof(Array))
                {
                    Type objType = property.PropertyType.GetElementType();
                    JArray innerArray = (JArray)arr[attribute.Index];
                    int count = 0;
                    if (innerArray.Count == 0)
                    {
                        IList arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 0 });
                        property.SetValue(result, arrayResult);
                    }
                    else if (innerArray[0].Type == JTokenType.Array)
                    {
                        IList arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { innerArray.Count });
                        foreach (JToken obj in innerArray)
                        {
                            object innerObj = Activator.CreateInstance(objType);
                            arrayResult[count] = ParseObject((JArray)obj, innerObj, objType);
                            count++;
                        }
                        property.SetValue(result, arrayResult);
                    }
                    else
                    {
                        IList arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 1 });
                        object innerObj = Activator.CreateInstance(objType);
                        arrayResult[0] = ParseObject(innerArray, innerObj, objType);
                        property.SetValue(result, arrayResult);
                    }
                    continue;
                }

                JsonConverterAttribute converterAttribute = (JsonConverterAttribute)property.GetCustomAttribute(typeof(JsonConverterAttribute)) ?? (JsonConverterAttribute)property.PropertyType.GetCustomAttribute(typeof(JsonConverterAttribute));
                JsonConversionAttribute conversionAttribute = (JsonConversionAttribute)property.GetCustomAttribute(typeof(JsonConversionAttribute)) ?? (JsonConversionAttribute)property.PropertyType.GetCustomAttribute(typeof(JsonConversionAttribute));
                object value = null;
                if (converterAttribute != null)
                {
                    value = arr[attribute.Index].ToObject(property.PropertyType, new JsonSerializer { Converters = { (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType) } });
                }
                else if (conversionAttribute != null)
                {
                    value = arr[attribute.Index].ToObject(property.PropertyType);
                }
                else
                {
                    value = arr[attribute.Index];
                }

                if (value != null && property.PropertyType.IsInstanceOfType(value))
                {
                    property.SetValue(result, value);
                }
                else
                {
                    if (value is JToken token)
                    {
                        if (token.Type == JTokenType.Null)
                        {
                            value = null;
                        }
                    }

                    if ((property.PropertyType == typeof(decimal)
                     || property.PropertyType == typeof(decimal?))
                     && (value != null && value.ToString().Contains("e")))
                    {
                        if (decimal.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal dec))
                        {
                            property.SetValue(result, dec);
                        }
                    }
                    else
                    {
                        property.SetValue(result, value == null ? null : Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }
            return result;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            PropertyInfo[] props = value.GetType().GetProperties();
            IOrderedEnumerable<PropertyInfo> ordered = props.OrderBy(p => p.GetCustomAttribute<ArrayPropertyAttribute>()?.Index);

            int last = -1;
            foreach (PropertyInfo prop in ordered)
            {
                ArrayPropertyAttribute arrayProp = prop.GetCustomAttribute<ArrayPropertyAttribute>();
                if (arrayProp == null)
                {
                    continue;
                }

                if (arrayProp.Index == last)
                {
                    continue;
                }

                while (arrayProp.Index != last + 1)
                {
                    writer.WriteValue((string)null);
                    last += 1;
                }

                last = arrayProp.Index;
                JsonConverterAttribute converterAttribute = (JsonConverterAttribute)prop.GetCustomAttribute(typeof(JsonConverterAttribute));
                if (converterAttribute != null)
                {
                    writer.WriteRawValue(JsonConvert.SerializeObject(prop.GetValue(value), (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType)));
                }
                else if (!IsSimple(prop.PropertyType))
                {
                    serializer.Serialize(writer, prop.GetValue(value));
                }
                else
                {
                    writer.WriteValue(prop.GetValue(value));
                }
            }
            writer.WriteEndArray();
        }

        private static bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type == typeof(string)
              || type == typeof(decimal);
        }
    }

    /// <summary>
    /// Mark property as an index in the array
    /// </summary>
    public class ArrayPropertyAttribute : Attribute
    {
        /// <summary>
        /// The index in the array
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="index"></param>
        public ArrayPropertyAttribute(int index)
        {
            Index = index;
        }
    }

    /// <summary>
    /// Used for conversion in ArrayConverter
    /// </summary>
    public class JsonConversionAttribute : Attribute
    {
    }
}
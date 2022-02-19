using Arbitrage.CoreApi.Attributes;
using Arbitrage.CoreApi.Database;
using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Dapper;
using Gizza.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arbitrage.CoreApi.BaseStructure
{
    public abstract class BaseSettings
    {
        #region Core Properties
        public object this[string propertyName]
        {
            get => GetType().GetProperty(propertyName).GetValue(this, null);
            set => GetType().GetProperty(propertyName).SetValue(this, value, null);
        }

        [JsonIgnore]
        public AppConnections AppConn { get; private set; }
        [JsonIgnore]
        public Dictionary<AppSettingsSection, bool> LoadedSections { get; private set; }
        [JsonIgnore]
        public Dictionary<AppSettingsSection, bool> SavedSections { get; private set; }
        #endregion

        public BaseSettings(AppConnections appConn)
        {
            AppConn = appConn;
            LoadedSections = new Dictionary<AppSettingsSection, bool>();
            SavedSections = new Dictionary<AppSettingsSection, bool>();

            foreach (AppSettingsSection section in Enum.GetValues(typeof(AppSettingsSection)))
            {
                LoadedSections[section] = false;
                SavedSections[section] = false;
            }

            LoadAllSections();
        }

        public void LoadSection(AppSettingsSection section)
        {
            // Reset Flags
            LoadedSections[section] = false;
            SavedSections[section] = false;

            // Get Values
            var criterias = new { SectionId = section };
            IEnumerable<APP_SETTING> pocoSettings = AppConn.dbConn.GetConnection().Query<APP_SETTING>($"SELECT * FROM { Tables.APP_SETTINGS } WHERE SECTION=@SectionId", criterias);

            // Check Point
            if (pocoSettings == null || !pocoSettings.Any())
            {
                return;
            }

            // Set Values
            System.Reflection.PropertyInfo[] properties = GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                foreach (System.Reflection.CustomAttributeData attr in property.CustomAttributes)
                {
                    if (attr.AttributeType.Name == nameof(AppSettingsFlagAttribute))
                    {
                        AppSettingsSection attrSection = (AppSettingsSection)((int)attr.ConstructorArguments[0].Value);
                        int attrKeyCode = (int)attr.ConstructorArguments[1].Value;
                        string propertyName = property.Name;
                        object propertyValue = property.GetValue(this);

                        // Be Sure -> Section of Property
                        if (attrSection == section)
                        {
                            // Find Correct Row
                            APP_SETTING row = pocoSettings.Where(x => x.KEYCODE == attrKeyCode).FirstOrDefault();
                            if (row != null)
                            {
                                // String
                                if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(this, row.VALUE_TEXT.ToStringSafe());
                                }

                                // Boolean
                                else if (property.PropertyType == typeof(bool))
                                {
                                    property.SetValue(this, row.VALUE_LONG.ToLongSafe().ToBooleanSafe());
                                }
                                else if (property.PropertyType == typeof(bool?))
                                {
                                    property.SetValue(this, row.VALUE_LONG.HasValue ? row.VALUE_LONG.ToLongSafe().ToBooleanSafe() : (bool?)null);
                                }

                                // DateTime
                                else if (property.PropertyType == typeof(DateTime))
                                {
                                    property.SetValue(this, row.VALUE_LONG.ToLongSafe().FromUnixTimeMilliSeconds());
                                }
                                else if (property.PropertyType == typeof(DateTime?))
                                {
                                    property.SetValue(this, row.VALUE_LONG.HasValue ? row.VALUE_LONG.ToLongSafe().FromUnixTimeMilliSeconds() : (DateTime?)null);
                                }

                                // Decimal
                                else if (property.PropertyType == typeof(decimal))
                                {
                                    property.SetValue(this, row.VALUE_DECIMAL.ToDecimalSafe());
                                }
                                else if (property.PropertyType == typeof(decimal?))
                                {
                                    property.SetValue(this, row.VALUE_DECIMAL);
                                }

                                // Double
                                else if (property.PropertyType == typeof(double))
                                {
                                    property.SetValue(this, row.VALUE_DECIMAL.ToDoubleSafe());
                                }
                                else if (property.PropertyType == typeof(double?))
                                {
                                    property.SetValue(this, row.VALUE_DECIMAL.HasValue ? row.VALUE_DECIMAL.ToDecimalSafe().ToDoubleSafe() : (double?)null);
                                }

                                // Float
                                else if (property.PropertyType == typeof(float))
                                {
                                    property.SetValue(this, row.VALUE_DECIMAL.ToFloatSafe());
                                }
                                else if (property.PropertyType == typeof(float?))
                                {
                                    property.SetValue(this, row.VALUE_DECIMAL.HasValue ? row.VALUE_DECIMAL.ToDecimalSafe().ToFloatSafe() : (float?)null);
                                }

                                // Integer
                                else if (property.PropertyType == typeof(int))
                                {
                                    property.SetValue(this, row.VALUE_LONG.ToLongSafe().ToInt32Safe());
                                }
                                else if (property.PropertyType == typeof(int?))
                                {
                                    property.SetValue(this, row.VALUE_LONG.HasValue ? row.VALUE_LONG.ToLongSafe().ToInt32Safe() : (int?)null);
                                }

                                // Long
                                else if (property.PropertyType == typeof(long))
                                {
                                    property.SetValue(this, row.VALUE_LONG.ToLongSafe());
                                }
                                else if (property.PropertyType == typeof(long?))
                                {
                                    property.SetValue(this, row.VALUE_LONG);
                                }

                                // Enum
                                else if (property.PropertyType.IsEnum)
                                {
                                    Type type = GetEnumType(property.PropertyType.Name);
                                    if (type != null)
                                    {
                                        property.SetValue(this, Enum.ToObject(type, row.VALUE_LONG.ToInt32Safe()));
                                    }
                                }

                                else
                                {
                                    property.SetValue(this, null);
                                }
                            }
                        }
                    }
                }
            }

            // Set Flag
            LoadedSections[section] = true;
        }

        public void SaveSection(AppSettingsSection section)
        {
            // Delete Old Values
            var criterias = new { SectionId = section };
            AppConn.dbConn.GetConnection().Execute($"DELETE FROM {Tables.APP_SETTINGS} WHERE SECTION=@SectionId", criterias);

            // Insert New Values
            System.Reflection.PropertyInfo[] properties = GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                foreach (System.Reflection.CustomAttributeData attr in property.CustomAttributes)
                {
                    if (attr.AttributeType.Name == nameof(AppSettingsFlagAttribute))
                    {
                        AppSettingsSection attrSection = (AppSettingsSection)((int)attr.ConstructorArguments[0].Value);
                        int attrKeyCode = (int)attr.ConstructorArguments[1].Value;
                        string propertyName = property.Name;
                        object propertyValue = property.GetValue(this);

                        // Be Sure -> Section of Property
                        if (attrSection == section)
                        {
                            APP_SETTING set = new APP_SETTING
                            {
                                CAT = AppStatic.Now.ToUnixTimeMilliSeconds(),
                                SECTION = section,
                                KEYCODE = attrKeyCode,
                            };

                            // String
                            if (property.PropertyType == typeof(string))
                            {
                                set.VALUE_TEXT = propertyValue.ToStringSafe();
                            }

                            // Boolean
                            else if (property.PropertyType == typeof(bool))
                            {
                                set.VALUE_LONG = ((bool)propertyValue) ? 1 : 0;
                            }
                            else if (property.PropertyType == typeof(bool?))
                            {
                                set.VALUE_LONG = propertyValue != null ? (((bool?)propertyValue).Value ? 1 : 0) : null;
                            }

                            // DateTime
                            else if (property.PropertyType == typeof(DateTime))
                            {
                                set.VALUE_LONG = ((DateTime)propertyValue).ToUnixTimeMilliSeconds();
                            }
                            else if (property.PropertyType == typeof(DateTime?))
                            {
                                set.VALUE_LONG = propertyValue != null ? ((DateTime?)propertyValue).Value.ToUnixTimeMilliSeconds() : null;
                            }

                            // Decimal
                            else if (property.PropertyType == typeof(decimal))
                            {
                                set.VALUE_DECIMAL = propertyValue.ToDecimalSafe();
                            }
                            else if (property.PropertyType == typeof(decimal?))
                            {
                                set.VALUE_DECIMAL = propertyValue?.ToDecimalSafe();
                            }

                            // Double
                            else if (property.PropertyType == typeof(double))
                            {
                                set.VALUE_DECIMAL = propertyValue.ToDecimalSafe();
                            }
                            else if (property.PropertyType == typeof(double?))
                            {
                                set.VALUE_DECIMAL = propertyValue?.ToDecimalSafe();
                            }

                            // Float
                            else if (property.PropertyType == typeof(float))
                            {
                                set.VALUE_DECIMAL = propertyValue.ToDecimalSafe();
                            }
                            else if (property.PropertyType == typeof(float?))
                            {
                                set.VALUE_DECIMAL = propertyValue?.ToDecimalSafe();
                            }

                            // Integer
                            else if (property.PropertyType == typeof(int))
                            {
                                set.VALUE_LONG = propertyValue.ToLongSafe();
                            }
                            else if (property.PropertyType == typeof(int?))
                            {
                                set.VALUE_LONG = propertyValue?.ToLongSafe();
                            }

                            // Long
                            else if (property.PropertyType == typeof(long))
                            {
                                set.VALUE_LONG = propertyValue.ToLongSafe();
                            }
                            else if (property.PropertyType == typeof(long?))
                            {
                                set.VALUE_LONG = propertyValue?.ToLongSafe();
                            }

                            // Enum
                            else if (property.PropertyType.IsEnum)
                            {
                                set.VALUE_LONG = (int)propertyValue;
                            }

                            // SubmitChanges
                            set.SubmitChanges(AppConn.dbConn);
                        }
                    }
                }
            }

            // Set Flag
            SavedSections[section] = true;
        }

        public void LoadAllSections()
        {
            foreach (AppSettingsSection section in Enum.GetValues(typeof(AppSettingsSection)))
            {
                LoadSection(section);
            }
        }

        public void SaveAllSections()
        {
            foreach (AppSettingsSection section in Enum.GetValues(typeof(AppSettingsSection)))
            {
                SaveSection(section);
            }
        }

        public string JsonExport()
        {
            return JsonConvert.SerializeObject(this);
        }

        private static Type GetEnumType(string enumName)
        {
            // Try - 01
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(enumName);
                if (type == null)
                {
                    continue;
                }

                if (type.IsEnum)
                {
                    return type;
                }
            }

            // Try - 02
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.ExportedTypes.Where(x => x.IsEnum))
                {
                    if (type.Name == enumName)
                    {
                        return type;
                    }
                }
            }

            // Return Null
            return null;
        }

    }
}

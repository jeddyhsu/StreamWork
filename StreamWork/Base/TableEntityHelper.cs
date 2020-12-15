// -----------------------------------------------------------------------------------------
// <copyright file="TableEntityHelper.cs" company="Microsoft">
// Copyright 2012 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// -----------------------------------------------------------------------------------------
using System.Linq;

namespace Microsoft.WindowsAzure.Storage.Table
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Azure.Cosmos.Table;

    public static class TableEntityHelper
    {
        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";
        private const string Timestamp = "Timestamp";
        private const string ETag = "ETag";

        public static void ReadEntity(ITableEntity entity, IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
#if RT
            IEnumerable<PropertyInfo> objectProperties = entity.GetType().GetRuntimeProperties();
#else
            IEnumerable<PropertyInfo> objectProperties = entity.GetType().GetProperties();
#endif

            foreach (PropertyInfo property in objectProperties)
            {
                // reserved properties
                if (property.Name == PartitionKey ||
                    property.Name == RowKey ||
                    property.Name == Timestamp ||
                    property.Name == ETag)
                {
                    continue;
                }

                // Enforce public getter / setter
#if RT
                if (property.SetMethod == null || !property.SetMethod.IsPublic || property.GetMethod == null || !property.GetMethod.IsPublic)
#else
                if (property.GetSetMethod() == null || !property.GetSetMethod().IsPublic || property.GetGetMethod() == null || !property.GetGetMethod().IsPublic)
#endif
                {
                    continue;
                }

                // only proceed with properties that have a corresponding entry in the dictionary
                if (!properties.ContainsKey(property.Name))
                {
                    continue;
                }

                if (property.GetCustomAttribute(typeof(IgnorePropertyAttribute)) != null)
                    continue;

                EntityProperty entityProperty = properties[property.Name];

                if (IsPropertyNull(entityProperty))
                {
                    property.SetValue(entity, null, null);
                }
                else
                {
                    switch (entityProperty.PropertyType)
                    {
                        case EdmType.String:
                            if (property.PropertyType != typeof(string) && property.PropertyType != typeof(String))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.StringValue, null);
                            break;
                        case EdmType.Binary:
                            if (property.PropertyType != typeof(byte[]))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.BinaryValue, null);
                            break;
                        case EdmType.Boolean:
                            if (property.PropertyType != typeof(bool) && property.PropertyType != typeof(Boolean) && property.PropertyType != typeof(Boolean?) && property.PropertyType != typeof(bool?))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.BooleanValue, null);
                            break;
                        case EdmType.DateTime:
                            if (property.PropertyType == typeof(DateTime))
                            {
                                property.SetValue(entity, entityProperty.DateTimeOffsetValue.Value.UtcDateTime, null);
                            }
                            else if (property.PropertyType == typeof(DateTime?))
                            {
                                property.SetValue(entity, entityProperty.DateTimeOffsetValue.HasValue ? entityProperty.DateTimeOffsetValue.Value.UtcDateTime : (DateTime?)null, null);
                            }
                            else if (property.PropertyType == typeof(DateTimeOffset))
                            {
                                property.SetValue(entity, entityProperty.DateTimeOffsetValue.Value, null);
                            }
                            else if (property.PropertyType == typeof(DateTimeOffset?))
                            {
                                property.SetValue(entity, entityProperty.DateTimeOffsetValue, null);
                            }

                            break;
                        case EdmType.Double:
                            if (property.PropertyType != typeof(double) && property.PropertyType != typeof(Double) && property.PropertyType != typeof(Double?) && property.PropertyType != typeof(double?))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.DoubleValue, null);
                            break;
                        case EdmType.Guid:
                            if (property.PropertyType != typeof(Guid) && property.PropertyType != typeof(Guid?))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.GuidValue, null);
                            break;
                        case EdmType.Int32:
                            if (property.PropertyType != typeof(int) && property.PropertyType != typeof(Int32) && property.PropertyType != typeof(Int32?) && property.PropertyType != typeof(int?))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.Int32Value, null);
                            break;
                        case EdmType.Int64:
                            if (property.PropertyType != typeof(long) && property.PropertyType != typeof(Int64) && property.PropertyType != typeof(long?) && property.PropertyType != typeof(Int64?))
                            {
                                continue;
                            }

                            property.SetValue(entity, entityProperty.Int64Value, null);
                            break;
                    }
                }
            }
        }

        public static IDictionary<string, EntityProperty> WriteEntity(ITableEntity entity, OperationContext operationContext)
        {
            Dictionary<string, EntityProperty> retVals = new Dictionary<string, EntityProperty>();

#if RT
            IEnumerable<PropertyInfo> objectProperties = entity.GetType().GetRuntimeProperties();
#else
            IEnumerable<PropertyInfo> objectProperties = entity.GetType().GetProperties();
#endif

            foreach (PropertyInfo property in objectProperties)
            {
                // reserved properties
                if (property.Name == PartitionKey ||
                    property.Name == RowKey ||
                    property.Name == Timestamp ||
                    property.Name == ETag)
                {
                    continue;
                }

                // Enforce public getter / setter
#if RT
                if (property.SetMethod == null || !property.SetMethod.IsPublic || property.GetMethod == null || !property.GetMethod.IsPublic)
#else
                if (property.GetSetMethod() == null || !property.GetSetMethod().IsPublic || property.GetGetMethod() == null || !property.GetGetMethod().IsPublic)
#endif
                {
                    continue;
                }

                if (property.GetCustomAttribute(typeof(IgnorePropertyAttribute)) != null)
                    continue;

                EntityProperty newProperty = CreateEntityPropertyFromObject(property.GetValue(entity, null), false);

                // property will be null if unknown type
                if (newProperty != null)
                {
                    retVals.Add(property.Name, newProperty);
                }
            }

            return retVals;
        }

        private static bool IsPropertyNull(EntityProperty prop)
        {
            switch (prop.PropertyType)
            {
                case EdmType.Binary:
                    return prop.BinaryValue == null;
                case EdmType.Boolean:
                    return !prop.BooleanValue.HasValue;
                case EdmType.DateTime:
                    return !prop.DateTimeOffsetValue.HasValue;
                case EdmType.Double:
                    return !prop.DoubleValue.HasValue;
                case EdmType.Guid:
                    return !prop.GuidValue.HasValue;
                case EdmType.Int32:
                    return !prop.Int32Value.HasValue;
                case EdmType.Int64:
                    return !prop.Int64Value.HasValue;
                case EdmType.String:
                    return prop.StringValue == null;
                default:
                    throw new InvalidOperationException("Unknown type!");
            }
        }

        private static EntityProperty CreateEntityPropertyFromObject(object value, bool allowUnknownTypes)
        {
            if (value is string)
            {
                return new EntityProperty((string)value);
            }
            else if (value is byte[])
            {
                return new EntityProperty((byte[])value);
            }
            else if (value is bool)
            {
                return new EntityProperty((bool)value);
            }
            else if (value is bool?)
            {
                return new EntityProperty((bool?)value);
            }
            else if (value is DateTime)
            {
                return new EntityProperty((DateTime)value);
            }
            else if (value is DateTime?)
            {
                return new EntityProperty((DateTime?)value);
            }
            else if (value is DateTimeOffset)
            {
                return new EntityProperty((DateTimeOffset)value);
            }
            else if (value is DateTimeOffset?)
            {
                return new EntityProperty((DateTimeOffset?)value);
            }
            else if (value is double)
            {
                return new EntityProperty((double)value);
            }
            else if (value is double?)
            {
                return new EntityProperty((double?)value);
            }
            else if (value is Guid?)
            {
                return new EntityProperty((Guid?)value);
            }
            else if (value is Guid)
            {
                return new EntityProperty((Guid)value);
            }
            else if (value is int)
            {
                return new EntityProperty((int)value);
            }
            else if (value is int?)
            {
                return new EntityProperty((int?)value);
            }
            else if (value is long)
            {
                return new EntityProperty((long)value);
            }
            else if (value is long?)
            {
                return new EntityProperty((long?)value);
            }
            else if (value == null)
            {
                return new EntityProperty((string)null);
            }
            else if (allowUnknownTypes)
            {
                return new EntityProperty(value.ToString());
            }
            else
            {
                return null;
            }
        }
    }
}
﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace OKN.Core.Helpers
{
    public class EnumDescriptionSerializerProvider : BsonSerializationProviderBase
    {
        public override IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry registry)
        {
            if (!type.GetTypeInfo().IsEnum) return null;

            var enumSerializerType = typeof(EnumDescriptionSerializer<>).MakeGenericType(type);
            var enumSerializerConstructor = enumSerializerType.GetConstructor(new Type[0]);
            var enumSerializer = (IBsonSerializer)enumSerializerConstructor?.Invoke(new object[0]);

            return enumSerializer;
        }
    }

    public class EnumDescriptionSerializer<TEnum> : StructSerializerBase<TEnum> where TEnum : struct
    {
        public BsonType Representation => BsonType.String;

        public override TEnum Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == Representation)
            {
                var valAsString = context.Reader.ReadString();
                var enumValue = valAsString.GetValueFromDescription<TEnum>();
                return enumValue;
            }

            object obj = context.Reader.ReadInt32();

            return (TEnum)obj;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TEnum value)
        {
            context.Writer.WriteString(value.GetDescription());
        }
    }

    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description for the supplied Enum Value.
        /// </summary>
        /// <param name="val">The value for which to get the description attribute.</param>
        /// <returns>The <see cref="string"/> description.</returns>
        public static string GetDescription<TEnum>(this TEnum val) where TEnum : struct
        {
            var name = Enum.GetName(val.GetType(), val);

            var fields = val.GetType().GetTypeInfo().GetDeclaredField(name);

            // first try and pull out the EnumMemberAttribute, common when using a JsonSerializer
            if (fields.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() is EnumMemberAttribute jsonAttribute) return jsonAttribute.Value;

            // If that doesn't work, do the regular description, that still fails, just return a pretty ToString().
            return !(fields.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute attribute) ? name : attribute.Description;
        }

        /// <summary>
        /// Get the value of an <see cref="Enum"/> based on its description attribute.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Enum"/>.</typeparam>
        /// <param name="description">The Description attribute of the <see cref="Enum"/>.</param>
        /// <returns>The value of T or default(T) if the description is not found.</returns>
        public static T GetValueFromDescription<T>(this string description) where T : struct
        {
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException(nameof(description));

            var type = typeof(T);
            if (!type.GetTypeInfo().IsEnum) throw new ArgumentOutOfRangeException(nameof(T), $"{typeof(T)} is not an Enum.");
            var fields = type.GetRuntimeFields();

            foreach (var field in fields)
            {
                if (field.Name == description) return (T)field.GetValue(null);

                // first try and pull out the EnumMemberAttribute, common when using a JsonSerializer
                if (field.GetCustomAttribute(typeof(EnumMemberAttribute), false) is EnumMemberAttribute jsonAttribute && jsonAttribute.Value == description) return (T)field.GetValue(null);

                // If that doesn't work, do the regular description, that still fails, just return a pretty ToString().
                if (field.GetCustomAttribute(typeof(DescriptionAttribute), false) is DescriptionAttribute attribute && attribute.Description == description) return (T)field.GetValue(null);
            }

            throw new Exception($"Failed to parse value {description} into enum {typeof(T)}");
        }
    }
}

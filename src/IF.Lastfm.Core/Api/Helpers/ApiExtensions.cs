﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IF.Lastfm.Core.Api.Helpers
{
    public static class ApiExtensions
    {
        public static T GetAttribute<T>(this Enum enumValue)
        where T : Attribute
        {
            return enumValue
                .GetType()
                .GetTypeInfo()
                .GetDeclaredField(enumValue.ToString())
                .GetCustomAttribute<T>();
        }

        public static string GetApiName(this Enum enumValue)
        {
            var attribute = enumValue.GetAttribute<ApiNameAttribute>();

            return (attribute != null && !string.IsNullOrWhiteSpace(attribute.Text))
                ? attribute.Text
                : enumValue.ToString();
        }

        public static int ToUnixTimestamp(this DateTime dt)
        {
            var d = (dt - new DateTime(1970, 1, 1).ToUniversalTime()).TotalSeconds;

            return Convert.ToInt32(d);
        }

        public static DateTime ToDateTimeUtc(this double stamp)
        {
            var d = new DateTime(1970, 1, 1).ToUniversalTime();
            d = d.AddSeconds(stamp);
            return d;
        }

        public static int CountOrDefault<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null
                ? enumerable.Count()
                : 0;
        }
    }
}
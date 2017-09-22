using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;

namespace SlackBotCore.Objects.JsonHelpers
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class GetOnlyJsonPropertyAttribute : Attribute
    {
    }

    public class GetOnlyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property != null && property.Writable)
            {
                var attributes = property.AttributeProvider.GetAttributes(typeof(GetOnlyJsonPropertyAttribute), true);
                if (attributes != null && attributes.Count > 0)
                {
                    property.Writable = false;
                    property.ShouldSerialize = property.ShouldDeserialize = x => { return false; };
                }
            }
            return property;
        }
    }
}

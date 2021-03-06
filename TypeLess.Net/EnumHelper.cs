﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeLess.Net
{
    public class EnumAttr
    {
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public string GroupName { get; set; }
        public int Order { get; set; }
        public string Prompt { get; set; }
        public string ShortName { get; set; }

        public EnumAttr(string name, string description, string groupName, int order, string prompt, string shortName)
        {
            DisplayName = name ?? String.Empty;
            Description = description ?? String.Empty;
            GroupName = groupName ?? String.Empty;
            Order = order;
            Prompt = prompt ?? String.Empty;
            ShortName = shortName ?? String.Empty;
        }

        public static EnumAttr Empty
        {
            get
            {
                return new EnumAttr(String.Empty, String.Empty, String.Empty, 0, String.Empty, String.Empty);
            }
        }
    }

    public static class EnumHelper
    {

        public static string GetDescription(this Enum value)
        {
            if (value == null) {
                return String.Empty;
            }

            
            return value.InternalGetDescription();
        }

        public static IEnumerable<T> AllValues<T>()
        {
            return Enum.GetValues(typeof(T)).OfType<Enum>().Where(x => ((int)(object)x) > 0).OrderBy(x => x.GetDisplayAttributes().Order).Select(x => (T)(object)x).ToList();
        }

        public static string GetFlagDescription(this Enum value)
        {
            if (value == null)
            {
                return String.Empty;
            }

            var allValues = Enum.GetValues(value.GetType()).OfType<Enum>().Where(x => ((int)(object)x) > 0).Where(x => (((int)(object)value) & ((int)(object)x)) != 0).OrderBy(x => x.GetDisplayAttributes().Order).Select(e => e.InternalGetDescription()).ToList();

            if (allValues.Count() == 1) {
                return allValues[0];
            }

            return String.Join(" & ", new string[] { String.Join(", ", allValues.TakeWhile((x, i) => i < allValues.Count - 1)), allValues.LastOrDefault() ?? "" });
        }

        private static string InternalGetDescription(this Enum value)
        {
            if (value == null)
            {
                return String.Empty;
            }

            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return String.Empty;
            }

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return value.ToString();
        }

        public static EnumAttr GetDisplayAttributes(this Enum value)
        {
            if (value == null)
            {
                return EnumAttr.Empty;
            }


            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null) {
                return EnumAttr.Empty;
            }

            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length > 0)
            {
                var attr = attributes[0];
                return new EnumAttr(attr.Name, attr.Description, attr.GroupName, attr.Order, attr.Prompt, attr.ShortName);
            }
            else
            {
                return EnumAttr.Empty;
            }

        }

    }
}

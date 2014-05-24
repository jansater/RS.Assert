﻿
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace RS.Assert
{

    /// <summary>
    /// Throws arg null exception instead of arg exception just to avoid parameter name messages ... could use a custom exception though
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public static class AssertExtensions 
    {

        private static string TryReadLine(string filePath, int line) {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string row = null;
                        for (int i = 0; i < line && !reader.EndOfStream; i++)
                        {
                            row = reader.ReadLine();
                        }

                        return row;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                return String.Format("{0}<{1}>",
                    type.Name,
                    String.Join<string>(",", type.GenericTypeArguments.Select(x => GetTypeName(x)).ToArray()));
            }
            else
            {
                return type.Name;
            }
        }

        public static Assertion<T> If<T>(this T source, string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int? lineNumber = null, [CallerMemberName] string caller = null)
        {

            if (String.IsNullOrEmpty(name) && Debugger.IsAttached && file != null && lineNumber.HasValue) {
                var line = TryReadLine(file, lineNumber.Value);
                if (line != null) {
                    var match = Regex.Match(line, "[ \\t]*(?<caller>.*?).If\\(");
                    if (match.Success) {
                        var callingProperty = match.Result("$1");
                        if (!String.IsNullOrWhiteSpace(callingProperty)) {
                            name = callingProperty;
                        }
                    }
                }
            }

            return new Assertion<T>(name ?? GetTypeName(typeof(T)), source, Path.GetFileName(file), lineNumber, caller);
        }

        /// <summary>
        /// Determines whether the specified source is null. Automatically stops further processing if source is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Assertion<T> IsNull<T>(this Assertion<T> source)
        {

            if (source.IgnoreFurtherChecks) {
                return source;
            }

            if (source.Item == null)
            {
                source.StopIfNotValid();
                source.Append("is required");
            }
            return source;
        }

        public static Assertion<string> IsEmpty(this Assertion<string> source)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            if (source.Item.Length <= 0)
            {
                source.Append("must be non empty");
            }
            return source;
        }

        public static Assertion<T> IsTrue<T>(this Assertion<T> source, Func<T, bool> assertFunc, string msgIfFalse)
        {
            if (assertFunc == null) {
                throw new ArgumentNullException("assertFunc is null");
            }

            if (msgIfFalse == null)
            {
                throw new ArgumentNullException("msgIfFalse");
            }

            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            if (assertFunc(source.Item))
            {
                source.Append(msgIfFalse);
            }
            return source;
        }

        public static Assertion<T> IsFalse<T>(this Assertion<T> source, Func<T, bool> assertFunc, string msgIfTrue)
        {
            if (assertFunc == null)
            {
                throw new ArgumentNullException("assertFunc is null");
            }

            if (msgIfTrue == null)
            {
                throw new ArgumentNullException("msgIfTrue");
            }

            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            if (!assertFunc(source.Item))
            {
                source.Append(msgIfTrue);
            }
            return source;
        }

        public static Assertion<T> IsEmpty<T>(this Assertion<T> targetObject) where T : IEnumerable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            var c = GetCount(targetObject.Item);

            if (c == 0)
            {
                targetObject.Append("must be non empty");
            }
            return targetObject;
        }

        private static int GetCount(IEnumerable e) {
            var asList = e as IList;
            if (asList != null) {
                return asList.Count;
            }

            var enu = e.GetEnumerator();
            
            if (!enu.MoveNext()) {
                return 0;
            }
            int i = 1;
            for (; enu.MoveNext(); i++);
            return i;
        }

        public static Assertion<T> ContainsLessThan<T>(this Assertion<T> targetObject, int nElements)
            where T : IEnumerable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            var count = GetCount(targetObject.Item);

            if (count < nElements)
            {
                targetObject.Append("must contain more than " + nElements + " items");
            }
            return targetObject;
        }

        public static Assertion<T> ContainsMoreThan<T>(this Assertion<T> targetObject, int nElements)
            where T : IEnumerable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            var count = GetCount(targetObject.Item);

            if (count > nElements)
            {
                targetObject.Append("must contain less than " + nElements + " items");
            }
            return targetObject;
        }

        public static Assertion<T> IsZero<T>(this Assertion<T> targetObject) where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            dynamic d = targetObject.Item;
            if (d == 0)
            {
                targetObject.Append("must be non zero");
            }
            return targetObject;
        }

        public static Assertion<T> IsNotEqualTo<T>(this Assertion<T> targetObject, T comparedTo) where T : IComparable<T>
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            if (targetObject.Item == null)
            {
                if (comparedTo != null)
                {
                    targetObject.Append("must not be equal to " + comparedTo.ToString());
                   
                }
                return targetObject;
            }

            if (targetObject.Item.CompareTo(comparedTo) != 0)
            {
                targetObject.Append(string.Format("must not be equal to {0}", comparedTo == null ? "null" : comparedTo.ToString()));
            }
            return targetObject;
        }

        public static Assertion<T> IsEqualTo<T>(this Assertion<T> targetObject, T comparedTo) where T : IComparable<T>
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            if (targetObject.Item == null)
            {
                if (comparedTo == null)
                {
                    targetObject.Append("must be equal to " + comparedTo.ToString());

                }
                return targetObject;
            }

            if (targetObject.Item.CompareTo(comparedTo) == 0)
            {
                targetObject.Append(string.Format("must be equal to {0}", comparedTo == null ? "null" : comparedTo.ToString()));
            }
            return targetObject;
        }

        public static Assertion<T> IsSmallerThan<T>(this Assertion<T> targetObject, T comparedTo) where T : IComparable<T>
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            if (targetObject.Item.CompareTo(comparedTo) <= 0)
            {
                targetObject.Append("must be larger than " + comparedTo);
            }

            return targetObject;
        }

        public static Assertion<T> IsLargerThan<T>(this Assertion<T> targetObject, T comparedTo) where T : IComparable<T>
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            if (targetObject.Item.CompareTo(comparedTo) >= 0)
            {
                targetObject.Append("must be smaller than " + comparedTo);
            }

            return targetObject;
        }

        public static Assertion<T> IsPositive<T>(this Assertion<T> targetObject) where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            if (targetObject.Item.ToDouble(Thread.CurrentThread.CurrentCulture).CompareTo(0.0) > 0.0)
            {
                targetObject.Append("must be zero or negative");
            }

            return targetObject;
        }

        public static Assertion<T> IsNegative<T>(this Assertion<T> targetObject) where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            if (targetObject.Item.ToDouble(Thread.CurrentThread.CurrentCulture).CompareTo(0.0) < 0.0)
            {
                targetObject.Append("must be zero or positive");
            }

            return targetObject;
        }

        public static Assertion<string> IsEmptyOrWhitespace(this Assertion<string> source)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            if (string.IsNullOrWhiteSpace(source.Item))
            {
                source.Append("must not be empty");
            }
            return source;
        }

        public static Assertion<string> IsNotValidEmail(this Assertion<string> source)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            bool isEmail = Regex.IsMatch(source.Item, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
            if (string.IsNullOrWhiteSpace(source.Item) || !isEmail)
            {
                source.Append("must be a valid email address");
            }
            return source;
        }

        public static Assertion<string> IsShorterThan(this Assertion<string> source, int length)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            if (source.Item.Length < length)
            {
                source.Append(String.Format("must be longer than {0} characters", length - 1));
            }
            return source;
        }

        public static Assertion<string> IsLongerThan(this Assertion<string> source, int length)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            if (source.Item.Length > length)
            {
                source.Append(String.Format("must be shorter than {0} characters", length + 1));
            }
            return source;
        }

        public static Assertion<string> DoesNotContainAlphaChars(this Assertion<string> source)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            bool isValid = Regex.IsMatch(source.Item, @"\W|_");
            if (!isValid)
            {
                source.Append("must contain alpha numeric characters");
            }

            return source;
        }

        public static Assertion<string> DoesNotContainDigit(this Assertion<string> source)
        {
            if (source.IgnoreFurtherChecks)
            {
                return source;
            }

            bool isValid = Regex.IsMatch(source.Item, @"\d");
            if (!isValid)
            {
                source.Append("must contain at least 1 digit");
            }

            return source;
        }

        public static Assertion<T> IsNotWithin<T>(this Assertion<T> targetObject, T min, T max) where T : struct, 
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            dynamic d = targetObject.Item;
            if (d < min || d > max)
            {
                targetObject.Append(String.Format("must be within {0} and {1}", min, max));
            }
            return targetObject;
        }

        /// <summary>
        /// Make a call to this class IsValid method to determine whether the specified target object is valid. Normally used to define validation checks in for example dto's. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetObject">The target object.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static Assertion<T> IsInvalid<T>(this Assertion<T> targetObject) where T : class
        {
            if (targetObject.IgnoreFurtherChecks)
            {
                return targetObject;
            }

            targetObject = targetObject.IsNull();

            if (targetObject.Item != null) {
                dynamic d = targetObject.Item;
                try
                {
                    var classAssertions = d.IsInvalid() as IEnumerable<IAssertion>;
                    foreach (var item in classAssertions)
                    {
                        targetObject = targetObject.Combine(item);
                    }
                }
                catch (RuntimeBinderException)
                {
                    throw new MissingMethodException("You must define method public IEnumerable<IAssertion> IsInvalid() {} in class " + typeof(T).Name);
                }
            }
            
            return targetObject;
        }

    }
}



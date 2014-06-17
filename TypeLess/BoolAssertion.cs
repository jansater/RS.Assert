﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TypeLess.DataTypes;
using TypeLess.Properties;

namespace TypeLess
{
    public interface IBoolAssertionU : IAssertionU<bool>
    {
        new IBoolAssertion IsTrue { get; }
        new IBoolAssertion IsFalse { get; }
        IBoolAssertionU Or(bool obj, string withName = null);
        new IBoolAssertion IsNotEqualTo(bool comparedTo);
        new IBoolAssertion IsEqualTo(bool comparedTo);
        
    }

    public interface IBoolAssertion : IBoolAssertionU, IAssertion<bool> {
        
    }

#if !DEBUG
    [DebuggerStepThrough]
#endif
    internal class BoolAssertion : Assertion<bool>, IBoolAssertion
    {
        private List<BoolAssertion> _childAssertions = new List<BoolAssertion>();

        public BoolAssertion(string s, bool source, string file, int? lineNumber, string caller)
            :base (s, source, file, lineNumber, caller) {}

        public IBoolAssertion Combine(IBoolAssertion otherAssertion)
        {
            return (IBoolAssertion)base.Or(otherAssertion);
        }

        public new IBoolAssertion StopIfNotValid
        {
            get {
                base.IgnoreFurtherChecks = true;
                return this;
            }
        }

        public new IBoolAssertion IsTrue {
            get {

                Extend(x => AssertResult.New(x, Resources.BoolIsTrue));
                return this;
            }
        }

        public new IBoolAssertion IsFalse
        {
            get
            {
                Extend(x => AssertResult.New(!x, Resources.BoolIsFalse));
                return this;
            }
        }

        public IBoolAssertionU Or(bool obj, string withName = null)
        {
            AddWithOr(new BoolAssertion(withName, obj, null, null, null));
            return this;
        }

        public new IBoolAssertion IsNotEqualTo(bool comparedTo)
        {
            return (IBoolAssertion)base.IsNotEqualTo(comparedTo);
        }

        public new IBoolAssertion IsEqualTo(bool comparedTo)
        {
            return (IBoolAssertion)base.IsEqualTo(comparedTo);
        }

    }
}

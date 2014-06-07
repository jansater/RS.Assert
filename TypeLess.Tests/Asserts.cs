﻿using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using TypeLess.Extensions.Sweden;

namespace TypeLess.Tests
{
    public class Asserts
    {
        [Fact]
        public void WhenNullThenDontThrowIfJustToString()
        {

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = null;
                var msg = s.If("s").IsNull.ToString();

                Xunit.Assert.True(msg.Contains("required"));
            });

        }

        [Fact]
        public void WhenNullThenThrow()
        {

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = null;
                s.If("s").IsNull.ThenThrow();
                
            });

            Assert.True(res.Message.StartsWith("s is required"));

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int? s = null;
                s.If().IsNull.ThenThrow();
            });

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = null;
                s.If().IsNull.ThenThrow();
            });

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<int> s = null;
                s.If().IsNull.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                int? s = 1;
                s.If().IsNull.ThenThrow();
            });

        }

        [Fact]
        public void WhenNullThenThrowWithSpecificException()
        {

            var res = Xunit.Assert.Throws<ArgumentException>(() =>
            {
                string s = null;
                s.If("s").IsNull.ThenThrow<ArgumentException>();
            });

            Assert.True(res.Message.StartsWith("s is required"));

        }


        [Fact]
        public void WhenNullThenDontCheckFurther()
        {
            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = null;
                Xunit.Assert.Equal(1, s.If("s").IsNull.IsEmpty.ErrorCount);
            });

        }

        [Fact]
        public void WhenStopSetThenDontCheckFurther()
        {
            string s = "";
            Xunit.Assert.Equal(1, s.If("s")
                .IsEmpty
                .StopIfNotValid
                .IsNotValidEmail.ErrorCount);
        }

        [Fact]
        public void WhenEmptyStringThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "";
                s.If("s").IsEmpty.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be non empty"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "d";
                s.If().IsEmpty.ThenThrow();
            });
        }

        [Fact]
        public void WhenIsTrueThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "";
                s.If("s").IsTrue(x => true, "<name> must be false i guess").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be false i guess"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "d";
                s.If().IsTrue(x => false, "").ThenThrow();

            });
        }

        [Fact]
        public void WhenIsFalseThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "";
                s.If("s").IsFalse(x => false, "{0} must be true i guess").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be true i guess"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "d";
                s.If().IsFalse(x => true, "").ThenThrow();

            });
        }

        [Fact]
        public void WhenIsEmptyThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var l = new List<int>();
                l.If("s").IsEmpty.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be non empty"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                var l = new List<int>() { 1 };
                l.If().IsEmpty.ThenThrow();
            });
        }

        [Fact]
        public void WhenEnumerationIsEmptyThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var l = new List<int>() as IEnumerable;
                l.If("s").IsEmpty.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be non empty"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                var l = new List<int>() { 1 } as IEnumerable;
                l.If().IsEmpty.ThenThrow();
            });
        }

        [Fact]
        public void WhenContainsLessThanThenThrow()
        {

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var l = new List<int> { 1, 2 };
                l.If("s").ContainsLessThan(3).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must contain more than 3 items"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                var l = new List<int> { 1, 2 };
                l.If().ContainsLessThan(2).ThenThrow();
            });
        }

        [Fact]
        public void WhenContainsMoreThanThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var l = new List<int> { 1, 2, 3, 4 };
                l.If("s").ContainsMoreThan(3).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must contain less than 3 items"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                var l = new List<int> { 1, 2 };
                l.If().ContainsMoreThan(2).ThenThrow();
            });
        }

        [Fact]
        public void WhenZeroThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 0;
                i.If("s").IsZero.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be non zero"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 1;
                i.If().IsZero.ThenThrow();
            });

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                double d = 0.0;
                d.If().IsZero.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                double d = 0.0001;
                d.If().IsZero.ThenThrow();
            });

        }

        [Fact]
        public void WhenNotEqualToThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 0;
                i.If("s").IsNotEqualTo(1).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be equal to 1"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 1;
                i.If().IsNotEqualTo(1).ThenThrow();
            });

        }

        [Fact]
        public void WhenEqualToThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 1;
                i.If("s").IsEqualTo(1).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must not be equal to 1"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 0;
                i.If().IsEqualTo(1).ThenThrow();
            });

        }

        [Fact]
        public void WhenSmallerThanThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 1;
                i.If("s").IsSmallerThan(2).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be larger than 2"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 1;
                i.If().IsSmallerThan(0).ThenThrow();
            });

        }

        [Fact]
        public void WhenLargerThanThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 3;
                i.If("s").IsLargerThan(2).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be smaller than 2"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 1;
                i.If().IsLargerThan(2).ThenThrow();
            });

        }

        [Fact]
        public void WhenPositiveThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 3;
                i.If("s").IsPositive.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be zero or negative"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 0;
                i.If().IsPositive.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = -1;
                i.If().IsPositive.ThenThrow();
            });

        }

        [Fact]
        public void WhenNegativeThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = -1;
                i.If("s").IsNegative.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be zero or positive"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 0;
                i.If().IsNegative.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 1;
                i.If().IsNegative.ThenThrow();
            });

        }

        [Fact]
        public void WhenNullableThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = -1;
                i.If("s").IsNegative.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be zero or positive"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 0;
                i.If().IsNegative.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 1;
                i.If().IsNegative.ThenThrow();
            });

        }

        [Fact]
        public void WhenIsEmptyOrContainsWhitespaceThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "   ";
                s.If("s").IsEmptyOrWhitespace.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must not be empty"));

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "";
                s.If().IsEmptyOrWhitespace.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "sasd";
                s.If().IsEmptyOrWhitespace.ThenThrow();
            });

        }

        [Fact]
        public void WhenIsInvalidEmailThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asdad@";
                s.If("s").IsNotValidEmail.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be a valid email address"));

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asdad";
                s.If().IsNotValidEmail.ThenThrow();
            });

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asdad@asd..se";
                s.If().IsNotValidEmail.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asdasd@asdad.se";
                s.If().IsEmptyOrWhitespace.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asdasd.asdad@asdad.se";
                s.If().IsEmptyOrWhitespace.ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asdasd.asdad@asdad..asdad.se";
                s.If().IsEmptyOrWhitespace.ThenThrow();
            });

        }



        [Fact]
        public void WhenIsShorterThanThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "123";
                s.If("s").IsShorterThan(4).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be longer than 3 characters"));


            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "1234";
                s.If().IsShorterThan(4).ThenThrow();
            });

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var span = TimeSpan.FromHours(2);
                span.If().IsShorterThan(TimeSpan.FromHours(3)).ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                var span = TimeSpan.FromHours(2);
                span.If().IsShorterThan(TimeSpan.FromHours(1)).ThenThrow();
            });

        }

        [Fact]
        public void WhenIsLongerThanThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "123";
                s.If("s").IsLongerThan(2).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be shorter than 3 characters"));


            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "123";
                s.If().IsLongerThan(3).ThenThrow();
            });

            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var span = TimeSpan.FromHours(2);
                span.If().IsLongerThan(TimeSpan.FromHours(1)).ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                var span = TimeSpan.FromHours(2);
                span.If().IsLongerThan(TimeSpan.FromHours(3)).ThenThrow();
            });

        }

        [Fact]
        public void WhenDoesNotContainNonAlphaCharsThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asd123";
                s.If("s").DoesNotContainAlphaChars.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must contain alpha numeric characters"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "123@";
                s.If().DoesNotContainAlphaChars.ThenThrow();
            });

        }

        [Fact]
        public void WhenDoesNotContainDigitThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asd";
                s.If("s").DoesNotContainDigit.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must contain at least 1 digit"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asd123";
                s.If().DoesNotContainDigit.ThenThrow();
            });

        }

        [Fact]
        public void WhenDoesNotContainTextThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asd";
                s.If("s").DoesNotContain("e").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must contain text e"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asd123";
                s.If().DoesNotContain("asd").ThenThrow();
            });

        }

        [Fact]
        public void WhenDoesNotStartWithTextThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asd";
                s.If("s").DoesNotStartWith("sd").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must start with text sd"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asd123";
                s.If().DoesNotStartWith("asd").ThenThrow();
            });

        }

        [Fact]
        public void WhenDoesNotEndWithTextThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "asd";
                s.If("s").DoesNotEndWith("as").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must end with text as"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "asd123";
                s.If().DoesNotEndWith("123").ThenThrow();
            });

        }

        [Fact]
        public void WhenNotWithinThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 5;
                i.If("s").IsNotWithin(1, 4).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be within 1 and 4"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 5;
                i.If().IsNotWithin(3, 6).ThenThrow();
            });

            DateTime dMin = new DateTime(2014, 05, 10);
            DateTime dMax = new DateTime(2014, 05, 23);
            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                DateTime d = new DateTime(2014, 05, 24);
                d.If().IsNotWithin(dMin, dMax).ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                DateTime d = new DateTime(2014, 05, 14);
                d.If().IsNotWithin(dMin, dMax).ThenThrow();
            });


        }

        [Fact]
        public void WhenIsWithinThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                int i = 3;
                i.If("s").IsWithin(1, 4).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must not be within 1 and 4"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                int i = 5;
                i.If().IsWithin(6, 9).ThenThrow();
            });

            DateTime dMin = new DateTime(2014, 05, 10);
            DateTime dMax = new DateTime(2014, 05, 23);
            Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                DateTime d = new DateTime(2014, 05, 15);
                d.If().IsWithin(dMin, dMax).ThenThrow();
            });

            Xunit.Assert.DoesNotThrow(() =>
            {
                DateTime d = new DateTime(2014, 05, 25);
                d.If().IsWithin(dMin, dMax).ThenThrow();
            });

            int i2 = 3;
            Assert.True(i2.If().IsWithin(1, 4).True);

        }

        public class SomeClassWithoutValidate
        {


        }

        public class SomeClassWithValidateB
        {
            public string Name { get; set; }
            public int Number { get; set; }

            public ObjectAssertion IsInvalid()
            {
                return ObjectAssertion.New(
                    Name.If("Name").IsNull, 
                    Number.If("Number").IsEqualTo(4));
            }
        }

        public class SomeClassWithValidate
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public SomeClassWithValidateB Prop { get; set; }

            public SomeClassWithValidate()
            {
                Name = "Some long string";
                Prop = new SomeClassWithValidateB();
            }

            public ObjectAssertion IsInvalid()
            {
                return ObjectAssertion.New(
                    Name.If("Name").IsShorterThan(5),
                    Number.If("Number").IsEqualTo(5)
                    //Prop.If("Prop").IsInvalid
                    );
            }
        }

        [Fact]
        public void WhenCombiningAssertsErrorMessageIsCorrect() {

            double d = 1;
            double d2 = 10;

            var errMsg = d.If("d").IsEqualTo(1).IsLargerThan(0).Combine(d2.If("d2").IsEqualTo(10)).ToString();
            Assert.True(errMsg.StartsWith("d must not be equal to 1 and d must be smaller than 0. d2 must not be equal to 10"));

        }

        [Fact]
        public void WhenDTOIsInValidThenThrow()
        {

            //Xunit.Assert.Throws<MissingMemberException>(() =>
            //{
            //    var x = new SomeClassWithoutValidate();
            //    x.If().IsInvalid.ThenThrow();
            //});

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                var x = new SomeClassWithValidate();
                x.If("s").IsInvalid.ThenThrow();
            });

            /*
             s must be null and Prop must be null and Name is required at IsInvalid, line number 724 in file Asserts.cs  at IsInvalid, line number 743 in file Asserts.cs  at WhenDTOIsInValidThenThrow, line number 761 in file Asserts.cs
             */

            Assert.True(res.Message.StartsWith("s must ..."));
        }

        [Fact]
        public void WhenInOneOfMultipleRangesThenReturnIsValid() {
            double heading = 345;

            var isTrue = heading.If()
                .IsWithin(315, 360) //true
                .Or(heading).IsWithin(0, 45) //another assertion that is false, returns the first one
                .Or(heading).IsWithin(135, 225);

            Assert.True(isTrue.True);

        }

        public class SomeClass
        {

        }

        [Fact]
        public void WhenAssertingMultipleItemsInOneAssertThenYouGetXAssertsEvenIfShortCircuit()
        {
            SomeClass s1 = null;
            SomeClass s2 = null;
            string d = "";

            var errMsg = s1.If("s1") 
                .Or(s2, "s2")
                .Or(d, "s3").IsNull.ToString();

            Xunit.Assert.True(errMsg.Contains("1") && errMsg.Contains("2") && errMsg.Contains("3"));

        }

        [Fact]
        public void AnyAssertWorksForDoubles()
        {
            
            double d1 = 1;
            double d2 = 3;
            double d3 = 4;

            var errMsg = d1.If("d1").Or(d2, "d2").Or(d3, "d3").IsSmallerThan(5).IsLargerThan(0).ToString();

            Xunit.Assert.True(errMsg.StartsWith("d1 must be larger than 5. d2 must be larger than 5. d3 must be larger than 5 and d1 must be smaller than 0. d2 must be smaller than 0. d3 must be smaller than 0"));

        }

        [Fact]
        public void WhenBoolIsFalseThenThrow()
        {

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                (1 == 0).If("expr").IsFalse.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("expr must be true"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                (1 == 1).If("expr").IsFalse.ThenThrow();   
            });

        }

        [Fact]
        public void WhenBoolIsTrueThenThrow()
        {

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                (1 == 1).If("expr").IsTrue.ThenThrow();
            });

            Assert.True(res.Message.StartsWith("expr must be false"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                (1 == 0).If("expr").IsTrue.ThenThrow();
            });

        }

        [Fact]
        public void WhenOnSameDayThenThrow()
        {
            DateTime d = DateTime.Now;
            DateTime d2 = DateTime.Now.Date.AddHours(2);

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                d.If("s").SameDayAs(d2).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must not be on same day as " + d2.ToString("yyyy-MM-dd")));

            Xunit.Assert.DoesNotThrow(() =>
            {
                d.If().SameDayAs(d2.AddDays(1)).ThenThrow();
            });
        }

        [Fact]
        public void WhenNotOnSameDayThenThrow()
        {

            DateTime d = DateTime.Now;
            DateTime d2 = DateTime.Now.Date.AddHours(2).AddDays(1);

            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                d.If("s").NotSameDayAs(d2).ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be on same day as " + d2.ToString("yyyy-MM-dd")));

            Xunit.Assert.DoesNotThrow(() =>
            {
                d.If().NotSameDayAs(d2.AddDays(-1)).ThenThrow();
            });
        }

        [Fact]
        public void WhenDoesNotContainKeyThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                Dictionary<string, int> d = new Dictionary<string, int>();
                d.If("s").DoesNotContainKey("some key").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must contain key some key"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                Dictionary<string, int> d = new Dictionary<string, int>();
                d.Add("some key", 1);
                d.If().DoesNotContainKey("some key").ThenThrow();
            });

        }

        [Fact]
        public void WhenContainKeyThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                Dictionary<string, int> d = new Dictionary<string, int>();
                d.Add("some key", 1);
                d.If("s").ContainsKey("some key").ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must not contain key some key"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                Dictionary<string, int> d = new Dictionary<string, int>();
                d.If().ContainsKey("some key").ThenThrow();
            });

        }

        [Fact]
        public void WhenIsNotValidPersonalNumberThenThrow()
        {
            var res = Xunit.Assert.Throws<ArgumentNullException>(() =>
            {
                string s = "123987";
                s.If("s").IsNotValidPersonalNumber().ThenThrow();
            });

            Assert.True(res.Message.StartsWith("s must be a valid personal number"));

            Xunit.Assert.DoesNotThrow(() =>
            {
                string s = "6708021586";
                s.If().IsNotValidPersonalNumber().ThenThrow();
            });

        }
    }
}

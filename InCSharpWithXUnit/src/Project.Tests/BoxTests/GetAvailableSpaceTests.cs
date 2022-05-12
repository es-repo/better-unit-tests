﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BetterUnitTests.InCSharpWithXUnit.Project.Tests.BoxTests
{
    public static class GetAvailableSpaceTests
    {
        sealed class TestCases : IEnumerable<object[]>
        {
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return EmptyBox_AvailableSpaceEqualToSize();

                // 2.
                yield return NonEmptyBox_AvailableSpaceLessThanSize();
            }

            static object[] EmptyBox_AvailableSpaceEqualToSize()
            {
                var boxSize = 100;

                var actualState = new Box
                {
                    Size = boxSize,
                };

                var expected = boxSize;

                return new object[] { actualState, expected };
            }

            static object[] NonEmptyBox_AvailableSpaceLessThanSize()
            {
                var boxSize = 100;

                var things = new[]
                {
                    new Thing { Size = 10 },
                    new Thing { Size = 20 }
                };

                var thingSizeSum = things.Sum(thing => thing.Size);

                var actualState = new Box(things.ToDictionary(thing => "Label" + thing.Size.ToString(), thing => thing))
                {
                    Size = boxSize,
                };

                var expected = boxSize - thingSizeSum;

                return new object[] { actualState, expected };
            }
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(Box actualState, int expected)
        {
            var actual = actualState.GetAvailableSpace();

            Assert.Equal(expected, actual);
        }
    }
}

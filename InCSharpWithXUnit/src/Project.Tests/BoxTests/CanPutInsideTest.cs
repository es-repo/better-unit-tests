using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace BetterUnitTests.InCSharpWithXUnit.Project.Tests.BoxTests
{
    public static class CanPutInsideTest
    {
        public sealed record Args
        {
            public Thing Thing { get; init; } = null!;
            public string Label { get; init; } = "";
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return OpenBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_TrueExpected();

                // 2.
                yield return ClosedBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_FalseExpected();
            }

            static object[] OpenBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_TrueExpected()
            {
                var stateActual = new Box(
                    new Dictionary<string, Thing>
                    {
                        { "Label1", new Thing { Size = 10 } },
                        { "Label2", new Thing { Size = 20 } },
                    })
                {
                    Size = 100,
                    IsOpen = true
                };

                var args = new Args
                {
                    Thing = new Thing { Size = 30 },
                    Label = "Label3"
                };

                var expected = true;

                return new object[] { stateActual, args, expected };
            }

            static object[] ClosedBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_FalseExpected()
            {
                var stateActual = new Box(
                   new Dictionary<string, Thing>
                   {
                        { "Label1", new Thing { Size = 10 } },
                        { "Label2", new Thing { Size = 20 } },
                   })
                {
                    Size = 100,
                    IsOpen = false
                };

                var args = new Args
                {
                    Thing = new Thing { Size = 30 },
                    Label = "Label3"
                };

                var expected = false;

                return new object[] { stateActual, args, expected };
            }
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Box stateActual,
            Args args,
            bool expected)
        {
            var actual = stateActual.CanPutInside(args.Thing, args.Label);

            Assert.Equal(expected, actual);
        }
    }
}

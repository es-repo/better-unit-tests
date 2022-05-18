using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace BetterUnitTests.InCSharpWithXUnit.Project.Tests.BoxTests
{
    public static class PutInsideTest
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
                yield return OpenBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_ThingAddedIntoBox_TrueExpected();

                // 2.
                yield return ClosedBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_ThingNotAddedIntoBox_FalseExpected();
            }

            static object[] OpenBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_ThingAddedIntoBox_TrueExpected()
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

                var stateExpected = new Box(new Dictionary<string, Thing>
                {
                    { "Label1", new Thing { Size = 10 } },
                    { "Label2", new Thing { Size = 20 } },
                    { "Label3", new Thing { Size = 30 } },
                })
                {
                    Size = 100,
                    IsOpen = true
                };

                var expected = true;

                return new object[] { stateActual, args, stateExpected, expected };
            }

            static object[] ClosedBox_ThingWithSizeLessThanAvailableSpace_UniqueLabel_ThingNotAddedIntoBox_FalseExpected()
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

                var stateExpected = new Box(
                     new Dictionary<string, Thing>
                     {
                        { "Label1", new Thing { Size = 10 } },
                        { "Label2", new Thing { Size = 20 } },
                     })
                {
                    Size = 100,
                    IsOpen = false
                };

                var expected = false;

                return new object[] { stateActual, args, stateExpected, expected };
            }
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Box stateActual,
            Args args,
            Box stateExpected,
            bool expected)
        {
            var actual = stateActual.PutInside(args.Thing, args.Label);

            Assert.Equal(stateExpected, stateActual);
            Assert.Equal(expected, actual);
        }
    }
}

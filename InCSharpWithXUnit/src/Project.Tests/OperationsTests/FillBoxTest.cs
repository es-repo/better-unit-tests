using System.Collections;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace BetterUnitTests.InCSharpWithXUnit.Project.Tests.OperationsTests
{
    public static class FillBoxTest
    {
        public sealed record Args
        {
            public IBox Box { get; init; } = null!;
            public Dictionary<string, Thing> LabelsAndThings { get; init; } = new();
            public WriteLog WriteLog { get; init; } = null!;
        }
        
        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return BoxAndLabelsAndThings_OpenBoxThenPutThingInsideThenCloseBoxAndWriteLogs_ThingsWithLabelEndedWithIgnoreExpected_1();
            }

            static object[] BoxAndLabelsAndThings_OpenBoxThenPutThingInsideThenCloseBoxAndWriteLogs_ThingsWithLabelEndedWithIgnoreExpected_1()
            {
                var writeLogMock = new Mock<WriteLog>(MockBehavior.Strict);
                var boxMock = new Mock<IBox>(MockBehavior.Strict);

                var mockSequence = new MockSequence();

                boxMock.InSequence(mockSequence).Setup(box => box.Open());
                writeLogMock.InSequence(mockSequence).Setup(writeLog => writeLog("The box is opened."));
                boxMock.InSequence(mockSequence).Setup(box => box.PutInside(new Thing { Size = 1 }, "Label 1")).Returns(true);
                boxMock.InSequence(mockSequence).Setup(box => box.PutInside(new Thing { Size = 2 }, "Label 2 Ignore")).Returns(false);
                boxMock.InSequence(mockSequence).Setup(box => box.PutInside(new Thing { Size = 3 }, "Label 3")).Returns(true);
                boxMock.InSequence(mockSequence).Setup(box => box.Close());
                writeLogMock.InSequence(mockSequence).Setup(writeLog => writeLog("The box is closed."));

                var args = new Args
                {
                    Box = boxMock.Object,

                    LabelsAndThings = new Dictionary<string, Thing>
                    {
                        { "Label 1", new Thing { Size = 1 } },
                        { "Label 2 Ignore", new Thing { Size = 2 } },
                        { "Label 3", new Thing { Size = 3 } },
                    },

                    WriteLog = writeLogMock.Object
                };

                var expected = new Dictionary<string, Thing>
                {
                    { "Label 2 Ignore", new Thing { Size = 2 } },
                };

                return new object[] { args, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Args args,
            Dictionary<string, Thing> expected)
        {
            var actual = Operations.FillBox(args.Box, args.LabelsAndThings, args.WriteLog);

            Assert.Equal(expected, actual);
        }
    }
}

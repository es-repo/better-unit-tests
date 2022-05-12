using BetterUnitTests.InCSharpWithXUnit.Project;
using CallTracing;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace BetterUnitTests.InCSharpWithXUnit.ProjectProject.Tests.OperationsTests
{
    public static class FillBoxTests
    {
        public sealed record Args
        {
            public IBox Box { get; init; } = null!;
            public Dictionary<string, Thing> LabelsAndThings { get; init; } = new();
            public WriteLog WriteLog { get; init; } = null!;
        }

        public sealed record BoxMock : IBox
        {
            private readonly CallTrace callTrace;

            public BoxMock(CallTrace callTrace)
            {
                this.callTrace = callTrace;
            }

            public void Open()
            {
                callTrace.Add<IBox>(box => box.Open());
            }

            public void Close()
            {
                callTrace.Add<IBox>(box => box.Close());
            }

            public bool PutInside(Thing thing, string label)
            {
                callTrace.Add<IBox>(box => box.PutInside(thing, label));

                return !label.EndsWith("Ignore");
            }
        }

        public static WriteLog CreateWriteLogMock(CallTrace callTrace)
        {
            return message =>
            {
                callTrace.Add<WriteLog>(writeLog => writeLog(message));
            };
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // 1.
                yield return BoxMock_LabelsAndThings_BoxMockCallsOpenThenPutInsideThenClose_ThingsWithLabelEndWithIngoreExpected();
            }

            static object[] BoxMock_LabelsAndThings_BoxMockCallsOpenThenPutInsideThenClose_ThingsWithLabelEndWithIngoreExpected()
            {
                var actualMockCallTrace = new CallTrace();

                var args = new Args
                {
                    Box = new BoxMock(actualMockCallTrace),

                    LabelsAndThings = new Dictionary<string, Thing>
                    {
                        { "Label 1", new Thing { Size = 1 } },
                        { "Label 2 Ignore", new Thing { Size = 2 } },
                        { "Label 3", new Thing { Size = 3 } },
                    },

                    WriteLog = CreateWriteLogMock(actualMockCallTrace)
                };

                var expectedMockCallTrace = new CallTrace();

                expectedMockCallTrace.Add<IBox>(box => box.Open());
                expectedMockCallTrace.Add<WriteLog>(writeLog => writeLog("The box is opened."));
                expectedMockCallTrace.Add<IBox>(box => box.PutInside(new Thing { Size = 1 }, "Label 1"));
                expectedMockCallTrace.Add<IBox>(box => box.PutInside(new Thing { Size = 2 }, "Label 2 Ignore"));
                expectedMockCallTrace.Add<IBox>(box => box.PutInside(new Thing { Size = 3 }, "Label 3"));
                expectedMockCallTrace.Add<IBox>(box => box.Close());
                expectedMockCallTrace.Add<WriteLog>(writeLog => writeLog("The box is closed."));

                var expected = new Dictionary<string, Thing>
                {
                    { "Label 2 Ignore", new Thing { Size = 2 } },
                };

                return new object[] { args, actualMockCallTrace, expectedMockCallTrace, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(Args args, CallTrace actualMockCallTrace, CallTrace expectedMockCallTrace, Dictionary<string, Thing> expected)
        {
            var actual = Operations.FillBox(args.Box, args.LabelsAndThings, args.WriteLog);

            Assert.Equal(actualMockCallTrace, expectedMockCallTrace);
            Assert.Equal(expected, actual);
        }
    }
}

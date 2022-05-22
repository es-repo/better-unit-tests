using CallTracing;
using System.Collections;
using System.Collections.Generic;
using Xunit;

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

        public sealed record BoxMock : IBox
        {
            private readonly CallTrace callTrace;

            public BoxMock(CallTrace callTrace)
            {
                this.callTrace = callTrace;
            }

            public void Open()
            {
                callTrace.Add((IBox box) => box.Open());
            }

            public void Close()
            {
                callTrace.Add((IBox box) => box.Close());
            }

            public bool PutInside(
                Thing thing,
                string label)
            {
                callTrace.Add((IBox box) => box.PutInside(thing, label));

                return !label.EndsWith("Ignore");
            }
        }

        public static WriteLog CreateWriteLogMock(CallTrace callTrace)
        {
            return message =>
            {
                callTrace.Add((WriteLog writeLog) => writeLog(message));
            };
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return BoxMock_LabelsAndThings_BoxMockCallsOpenThenPutInsideThenClose_ThingsWithLabelEndedWithIgnoreExpected_1();
            }

            static object[] BoxMock_LabelsAndThings_BoxMockCallsOpenThenPutInsideThenClose_ThingsWithLabelEndedWithIgnoreExpected_1()
            {
                var callTraceActual = new CallTrace();

                var args = new Args
                {
                    Box = new BoxMock(callTraceActual),

                    LabelsAndThings = new Dictionary<string, Thing>
                    {
                        { "Label 1", new Thing { Size = 1 } },
                        { "Label 2 Ignore", new Thing { Size = 2 } },
                        { "Label 3", new Thing { Size = 3 } },
                    },

                    WriteLog = CreateWriteLogMock(callTraceActual)
                };

                var callTraceExpected = new CallTrace(
                    (IBox box) => box.Open(),
                    (WriteLog writeLog) => writeLog("The box is opened."),
                    (IBox box) => box.PutInside(new Thing { Size = 1 }, "Label 1"),
                    (IBox box) => box.PutInside(new Thing { Size = 2 }, "Label 2 Ignore"),
                    (IBox box) => box.PutInside(new Thing { Size = 3 }, "Label 3"),
                    (IBox box) => box.Close(),
                    (WriteLog writeLog) => writeLog("The box is closed."));

                var expected = new Dictionary<string, Thing>
                {
                    { "Label 2 Ignore", new Thing { Size = 2 } },
                };

                return new object[] { args, callTraceActual, callTraceExpected, expected };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(
            Args args,
            CallTrace callTraceActual,
            CallTrace callTraceExpected,
            Dictionary<string, Thing> expected)
        {
            var actual = Operations.FillBox(args.Box, args.LabelsAndThings, args.WriteLog);

            Assert.Equal(callTraceActual, callTraceExpected);
            Assert.Equal(expected, actual);
        }
    }
}

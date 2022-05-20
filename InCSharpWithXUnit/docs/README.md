# Proposal for structuring unit test code in C# with xUnit

## Projects

A project under test has a corresponding unit test project. If a project is named `{ProjectName}`, then the unit test project is named `{ProjectName}.Tests`.

To test `internal` code add the following to the project file:

```XML
<ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
        <_Parameter1>$(AssemblyName).Tests</_Parameter1>
    </AssemblyAttribute>
</ItemGroup>
```

## Folders and files

A class (record, struct) under test has a corresponding folder in the unit test project. If a class is named `{ClassName}`, then the folder is named `{ClassName}Tests`.

A method under test has a corresponding test file. If a method is named `{MethodName}`, then the file is named `{MethodName}Test.cs`. If a method has overloading versions then the name should include something which identifies the overloaded version: `{MethodName}{...}Test.cs`.

## Test class

If a method under test named `{MethodName}`, then test class is named `{MethodName}Test`.
A test class is static and has only one test method named `Test` with `[Theory]` and `[ClassData(typeof(TestCases))]` attributes. 
A test class has `TestCases` nested class which yields arguments for the `Test` method for every test case.
A test class has nested `Args` record. `Args` record contains arguments to be passed to method under test.

### Structure

```C#
public static class {MethodName}Test
{
    public sealed record Args {...}        
    
    sealed class TestCases : IEnumerable<object[]>
    {...}
        
    [Theory]
    [ClassData(typeof(TestCases))]
    public static void Test(...)
    {...}
}
```

## Test method

### Parameters

A test method has following parameters:
- `{TypeOfInstance} stateActual` - an instance of a type method of which is under test. If the method is static, then this parameter is omitted. 
- `Args args` - an instance of `Args` which is used to pass arguments to the method under test. If the method does not have parameters, then this parameter is omitted. 
- `CallTrace callTraceActual` - an instance of [CallTrace](https://github.com/es-repo/CallTracing) which contains an actual trace of mock calls. If the test is not supposed to test mock calls then this parameter is omitted.
- `CallTrace callTraceExpected` - an instance of [CallTrace](https://github.com/es-repo/CallTracing) which contains an expected trace of mock calls. If the test is not supposed to test mock calls, then this parameter is omitted.
- `{TypeOfInstance} stateExpected`  - an instance of a type, method of which is under test, with state which is expected to be equal to the state of `stateActual` after the method is called. If the method is static or does not change the state of the object, then this parameter is omitted.
- `{TypeOfReturnValue} expected` - a value which is expected to be equal to the value returned by the method under test. If the method returns nothing, then this parameter is omitted.

### Body

The body of a test method should call the method under test and then verify that actual and expected states, call traces of mocks, and returned values are equal. It should preferably use only `Assert.Equal` assertions to keep code concise.

_NOTE: This puts requirements on types under test to have an implementation of `Equals` method which ensures that two instances are equal if all their property and field values match. In turn implementation of `Equals` method also requires implementation of `GetHashCode`. To reduce the custom implementations using records instead of classes and structs is advised. However, the default `Equals` implementation of a record may not be enough if any of its field or property types have equality by reference implementation. In that case, custom implementation still is required to use `Assert.Equal` only assertion. Sometimes using records may not be possible or such custom `Equals` implementation may cause performance issues. Then the decision of following the proposing guidelines exactly or deviating from them should be taken by a developer meaningfully._

### Structure

```C#
[Theory]
[ClassData(typeof(TestCases))]
public static void Test(
  {TypeOfInstance} stateActual, 
  Args args, 
  CallTrace callTraceActual,
  CallTrace callTraceExpected,
  {TypeOfInstance} stateExpected,
  {ReturnValueType} expected)
{
  var actual = stateActual.{TestedMethod}(args.{...}, args.{...}, ...);
    
  Assert.Equal(callTraceExpected, callTraceActual);
  Assert.Equal(stateExpected, stateActual);
  Assert.Equal(expected, actual);
}
```

## Test cases

For every test case, `TestCases` class has a static method returning an array of objects to be passed as arguments to the `Test` method. The name of a test case method should describe the test case and preferably be in form of `{stateActualDescription}_{argsDescription}_{stareExpectedDescription}_{etc...}_{n}` where `n` is a serial number of a test case. A serial number is required to simplify finding a failed test.

### Structure 

```C#
sealed class TestCases : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return {TestCaseDescription}_1();
        yield return {TestCaseDescription}_2();
        ...
        yield return {TestCaseDescription}_n();
    }
    
    static object[] {TestCaseDescription}_1()
    {
        var stateActual = ...;
        var args = ...;
        var callTraceActual = ...;
        var callTraceExpected = ...;
        var stateExpected = ...;
        var expected = ...;
        
        return new object[] { stateActual, args, stateExpected, callTraceActual, callTraceExpected };
    }

    static object[] {TestCaseDescription}_2()
    { ... }

    ...
    
    static object[] {TestCaseDescription}_n()
    { ... }
}
```

## Examples

Examples can be found [here](https://github.com/es-repo/better-unit-tests/tree/main/InCSharpWithXUnit/src).




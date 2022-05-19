# Proposal for structuring unit test code in C# with xUnit

## Projects

A project under test has a corresponding unit test project. If a project is named `{ProjectName}` then the unit test project is named `{ProjectName}.Tests`.

To test not only `public` but `internal` code add following into project file:

```XML
<ItemGroup>
    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
        <_Parameter1>$(AssemblyName).Tests</_Parameter1>
    </AssemblyAttribute>
</ItemGroup>
```

## Folders and files

A class (record, struct) under test has a corresponding folder in the unit test project. If a class is named `{ClassName}` then the folder is named `{ClassName}Tests`.

A method under test has a corresponding test file. If a method is named `{MethodName}` then the file is named `{MethodName}Test.cs`. If a method has overloading versions then the name should include something which identifies the overloaded version: `{MethodName}{...}Test.cs`.

## Test class

If a method under test named `{MethodName}` then test class is named `{MethodName}Test`.
A test class has only one test method named `Test` with `[Theory]` and `[ClassData(typeof(TestCases))]` attributes. 
A test class has `TestCases` nested class which yield arguments for the `Test` method for every test case.
A test class has nested `Args` record. `Args` record contains arguments to be passed to method under test.

### Structure

```C#
public static class {MethodName}Test
{
    public sealed record Args {...}        
    
    sealed class TestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {...}
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
        
    [Theory]
    [ClassData(typeof(TestCases))]
    public static void Test(...)
    {...}
}
```

## Test method

### Parameters

Test method has following parameters:
- `{TypeOfInstance} stateActual` - an instance of a type method of which is under test. If the method is static then this parameter is omitted. 
- `Args args` - an instance of `Args` which is used to pass arguments to the method under test. If the method does not have parameters then this parameter is omitted. 
- `{TypeOfInstance} stateExpected`  - an instance of a type, method of which is under test, with state which is expected to be equal to the state of `stateActual` after the method is called. If the method is static or does not change the state of the object then this parameter is omitted.
- `CallTrace callTraceActual` - an instance of [CallTrace](https://github.com/es-repo/CallTracing) which contains an actual trace of mock calls. If the test is not supposed to test mock calls then this parameter is omitted.
- `CallTrace callTraceExpected` - an instance of [CallTrace](https://github.com/es-repo/CallTracing) which contains an expected trace of mock calls. If the test is not supposed to test mock calls then this parameter is omitted.
- `{TypeOfReturnValue} expected` - a value which is expected to be equal to the value returned by the method under test. If the method returns nothing then this parameter is omitted.

### Structure

```C#
[Theory]
[ClassData(typeof(TestCases))]
public static void Test(
  {TypeOfInstance} stateActual, 
  Args args, 
  {TypeOfInstance} stateExpected,
  CallTrace callTraceActual,
  CallTrace callTraceExpected,
  {ReturnValueType} expected)
{
  var actual = stateActual.{TestedMethod}(args.{...}, args.{...}, ...);
  
  Assert.Equal(stateExpected, stateActual);
  Assert.Equal(callTraceExpected, callTraceActual);
  Assert.Equal(expected, actual);
}
```




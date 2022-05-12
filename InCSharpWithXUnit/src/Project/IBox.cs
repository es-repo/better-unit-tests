namespace BetterUnitTests.InCSharpWithXUnit.Project
{
    public interface IBox
    {
        void Open();

        void Close();

        bool PutInside(Thing thing, string label);
    }
}

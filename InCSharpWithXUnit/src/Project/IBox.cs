namespace BetterUnitTests.InCSharpWithXUnit.Project
{
    public interface IBox
    {
        public int Size { get; }

        void Open();

        void Close();

        bool PutInside(
            Thing thing,
            string label);
    }
}

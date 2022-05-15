namespace BetterUnitTests.InCSharpWithXUnit.Project
{
    public delegate void WriteLog(string message);

    public static class Operations
    {
        public static Dictionary<string, Thing> FillBox(
            IBox box, 
            IDictionary<string, Thing> labelsAndThings, 
            WriteLog writeLog)
        {
            var rest = new Dictionary<string, Thing>();

            box.Open();

            writeLog("The box is opened.");

            foreach (var (label, thing) in labelsAndThings)
            {
                if (!box.PutInside(thing, label))
                {
                    rest.Add(label, thing);
                }
            }

            box.Close();

            writeLog("The box is closed.");

            return rest;
        }
    }
}

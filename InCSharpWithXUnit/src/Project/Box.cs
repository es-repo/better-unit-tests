namespace BetterUnitTests.InCSharpWithXUnit.Project
{
    public sealed record Box : IBox
    {
        private readonly Dictionary<string, Thing> thingsInside;

        public bool IsOpen { get; set; }

        public int Size { get; init; }

        /// <summary>
        /// Internal constructor to set initial state for test purpose.
        /// </summary>
        internal Box(Dictionary<string, Thing> thingsInside)
        {
            this.thingsInside = thingsInside;
        }

        public Box() : this(new Dictionary<string, Thing>())
        {
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public int GetAvailableSpace()
        {
            return Size - thingsInside.Values.Sum(thing => thing.Size);
        }

        public bool CanPutInside(Thing thing, string label)
        {
            return IsOpen && GetAvailableSpace() - thing.Size >= 0 && !thingsInside.ContainsKey(label);
        }

        public bool PutInside(Thing thing, string label)
        {
            if (!CanPutInside(thing, label))
            {
                return false;
            }

            thingsInside.Add(label, thing);

            return true;
        }

        public bool Equals(Box? other)
        {
            if (other == null)
            {
                return false;
            }

            return IsOpen.Equals(other.IsOpen) &&
                Size == other.Size &&
                thingsInside.SequenceEqual(other.thingsInside);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + IsOpen.GetHashCode();
                hash = (hash * 7) + Size.GetHashCode();
                hash = thingsInside.Aggregate(hash, (acc, o) => (acc * 7) + o.GetHashCode());
                return hash;
            }
        }
    }
}

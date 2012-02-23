using System;

namespace SimpleScriptRunner
{
    [Serializable]
    internal class ScriptVersion : IComparable<ScriptVersion>
    {
        internal ScriptVersion(int releaseNumber, int scriptNumber, DateTime modified)
        {
            ReleaseNumber = releaseNumber;
            ScriptNumber = scriptNumber;
            Modified = modified;
        }

        protected internal int ReleaseNumber { get; private set; }

        internal int ScriptNumber { get; private set; }

        internal DateTime Modified { get; private set; }

        #region IComparable<ScriptVersion> Members

        public int CompareTo(ScriptVersion other)
        {
            var compare = ReleaseNumber.CompareTo(other.ReleaseNumber);
            if (compare != 0)
            {
                return compare;
            }
            
            compare = ScriptNumber.CompareTo(other.ScriptNumber);
            if (compare != 0)
            {
                return compare;
            }
         
            return RoundDateTime(Modified).CompareTo(RoundDateTime(other.Modified));
        }

        #endregion

        public override string ToString()
        {
            return ReleaseNumber + ", " + ScriptNumber + ", " + Modified;
        }

        private DateTime RoundDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}
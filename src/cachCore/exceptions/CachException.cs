using System;

namespace cachCore.exceptions
{
    public class CachException : Exception
    {
        public CachException(string msg) :
            base(msg)
        {
        }
    }
}

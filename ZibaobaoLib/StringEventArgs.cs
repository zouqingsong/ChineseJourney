using System;

namespace ZibaobaoLib
{
    public class StringEventArgs : EventArgs
    {
        public StringEventArgs(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }
}
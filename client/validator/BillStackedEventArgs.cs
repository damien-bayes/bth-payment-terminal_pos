using System;
using System.ComponentModel;

namespace IPTS.Validator
{
    public class BillStackedEventArgs : CancelEventArgs
    {
        public int Value
        {
            get;
            private set;
        }

        #region Constructor
        public BillStackedEventArgs(int value)
        {
            Value = value;
            Cancel = false;
        }
        #endregion
    }
}

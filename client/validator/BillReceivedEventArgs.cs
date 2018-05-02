using System;

namespace IPTS.Validator
{
    public class BillReceivedEventArgs : EventArgs
    {
        public BillRecievedStatus Status
        {
            get;
            private set;
        }

        public int Value
        {
            get;
            private set;
        }

        public string RejectedReason
        {
            get;
            private set;
        }

        #region Constructor
        public BillReceivedEventArgs(BillRecievedStatus billRecievedStatus, int value, string rejectedReason)
        {
            Status = billRecievedStatus;
            Value = value;
            RejectedReason = rejectedReason;
        }
        #endregion
    }
}

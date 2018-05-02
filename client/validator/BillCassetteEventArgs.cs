using System;

namespace IPTS.Validator
{
    public class BillCassetteEventArgs : EventArgs
    {
        public BillCassetteStatus Status
        {
            get;
            private set;
        }

        #region Constructor
        public BillCassetteEventArgs(BillCassetteStatus billCassetteStatus)
        {
            Status = billCassetteStatus;
        }
        #endregion
    }
}

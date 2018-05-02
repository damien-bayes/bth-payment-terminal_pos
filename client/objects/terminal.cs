#region Namespaces
using System;
#endregion

namespace IPTS.Objects
{
    public class TerminalObject
    {
        private Terminal _terminal;

        public TerminalObject(Terminal terminal)
        {
            this._terminal = terminal;
        }

        public string getToken()
        {
            return this._terminal.Token;
        }

        public void setToken(string token)
        {
            this._terminal.Token = token;
        }

        //public string getGuid()
        //{
            //return this._terminal.TerminalGuid.ToString();
        //}
    }
}

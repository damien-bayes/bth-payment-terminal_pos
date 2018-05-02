using System;
using System.Windows.Forms;

namespace IPTS
{
    /**
     *  KeyStrokeMessageFilter _keyStrokeMessageFilter = new KeyStrokeMessageFilter();
     *   Application.AddMessageFilter(_keyStrokeMessageFilter); 
     */
    public class KeyStrokeMessageFilter : IMessageFilter
    {
        public KeyStrokeMessageFilter() { }

        public bool PreFilterMessage(ref Message m)
        {
            /* 0x0100 */
            if (m.Msg == 256)
            {
                switch (((int)m.WParam) | ((int)Control.ModifierKeys))
                {
                    case (int)(Keys.Control | Keys.Alt | Keys.K):
                        MessageBox.Show("You pressed ctrl + alt + k");
                        break;
                    case (int)(Keys.Control | Keys.C):
                        MessageBox.Show("You pressed ctrl+c");
                        break;
                    case (int)(Keys.Control | Keys.V):
                        MessageBox.Show("You pressed ctrl+v");
                        break;
                    case (int)Keys.Up:
                        MessageBox.Show("You pressed up");
                        break;
                }
            }
            return false;
        }
    }
}

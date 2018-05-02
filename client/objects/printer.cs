using System;
using System.Collections.Generic;
using System.Json;
using System.Runtime.InteropServices;

namespace IPTS.Objects
{
    /*
     * CITIZEN CBM1000 Type II
     */
    public class PrinterObject
    {
        class ReceiptSchema
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }

        private string _printerName;

        public PrinterObject(string printerName)
        {
            _printerName = printerName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DOCINFO
        {
            public int cbSize;
            public string lpszDocName;
            public string lpszOutput;
            public string lpszDatatype;
            public int fwType;
        }

        [DllImport(
            "winspool.drv", 
            CharSet = CharSet.Unicode, 
            ExactSpelling = false, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern long OpenPrinter(String pPrinterName, ref IntPtr phPrinter, IntPtr pDefault);

        [DllImport(
            "winspool.drv", 
            CharSet = CharSet.Unicode, 
            ExactSpelling = false, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern long ClosePrinter(IntPtr hPrinter);

        [DllImport(
            "winspool.drv", 
            CharSet = CharSet.Unicode, 
            ExactSpelling = false, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);
        private const int DM_OUT_BUFFER = 0x2;
        private const int DM_PROMPT = 0x4;
        private const int DM_IN_PROMPT = DM_PROMPT;
        private const int DM_IN_BUFFER = 0x8;

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 StartDoc(IntPtr hdc, [MarshalAs(UnmanagedType.Struct)] ref DOCINFO lpdi);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 EndDoc(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 StartPage(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 EndPage(IntPtr hDC);

        [StructLayout(LayoutKind.Sequential)]
        public class LOGFONT
        {
            public const int LF_FACESIZE = 32;
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
            public string lfFaceName;
        }
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT lplf);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr handle);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteObject(IntPtr handle);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private const int LOGPIXELSY = 90;

        public void printEncashment(string jsonString, string facename = "20 cpi")
        {
            if (string.IsNullOrEmpty(jsonString)) return;

            // Convert JSON to the key value pairs
            JsonValue jsonValue = JsonValue.Parse(jsonString);

            if (jsonValue.Count == 0) return;

            IntPtr hPrinter = IntPtr.Zero;
            OpenPrinter(_printerName, ref hPrinter, IntPtr.Zero);

            // Create Printer Device Context
            IntPtr hwnd = IntPtr.Zero;
            int size = DocumentProperties(hwnd, hPrinter, _printerName, IntPtr.Zero, IntPtr.Zero, 0);
            IntPtr pDevmode = Marshal.AllocHGlobal(size);
            int ret = DocumentProperties(hwnd, hPrinter, _printerName, pDevmode, IntPtr.Zero, DM_OUT_BUFFER);
            IntPtr hDC = CreateDC(null, _printerName, null, pDevmode);

            // Make Print-Job
            DOCINFO di = new DOCINFO();
            di.cbSize = Marshal.SizeOf(di);
            di.lpszDocName = "IPTS Receipt";

            // Start printing
            StartDoc(hDC, ref di);
            StartPage(hDC);
            LOGFONT lf = new LOGFONT();
            IntPtr hFont = IntPtr.Zero;
            IntPtr oldFont = IntPtr.Zero;
            string str;
            int fontsize, pointsize;

            // Windows font
            pointsize = 10;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = facename;
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);

            List<ReceiptSchema> list = new List<ReceiptSchema>();
            list.Add(new ReceiptSchema() { Key = 0, Value = "Инкассация произведена успешно" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "----------------------------------------------------" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "№ инкассации" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["encashmentId"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "ФИО инкассатора" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["fullName"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "ИИН/БИН инкассатора" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["taxpayerNumber"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Дата инкассирования" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["created"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Инкассирован с" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["terminalId"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Сумма" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["sum"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "----------------------------------------------------" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Обязательно сохраняйте квитанцию." });
            list.Add(new ReceiptSchema() { Key = 0, Value = "СПАСИБО за использование наших услуг!" });

            int x = 0, y = -1;
            foreach (ReceiptSchema pair in list)
            {
                int handler = pair.Key;
                str = pair.Value;

                if (handler == 1)
                {
                    x = 148;
                }
                else if (handler == 0)
                {
                    x = 0;
                    y++;
                }
                TextOut(hDC, x, y * 50, str, str.Length);
            }


            SelectObject(hDC, oldFont);
            DeleteObject(hFont);

            // Cut paper
            pointsize = 12;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Control";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "";
            TextOut(hDC, 0, y * 50, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);

            // End Printing
            EndPage(hDC);
            EndDoc(hDC);

            // Release Printer Device Context
            DeleteDC(hDC);
            Marshal.FreeHGlobal(pDevmode);
            ClosePrinter(hPrinter);
        }

        public void printReceipt(string jsonString, string facename = "20 cpi")
        {
            if (string.IsNullOrEmpty(jsonString)) return;

            // Convert JSON to the key value pairs
            JsonValue jsonValue = JsonValue.Parse(jsonString);

            if (jsonValue.Count == 0) return;

            IntPtr hPrinter = IntPtr.Zero;
            OpenPrinter(_printerName, ref hPrinter, IntPtr.Zero);

            // Create Printer Device Context
            IntPtr hwnd = IntPtr.Zero;
            int size = DocumentProperties(hwnd, hPrinter, _printerName, IntPtr.Zero, IntPtr.Zero, 0);
            IntPtr pDevmode = Marshal.AllocHGlobal(size);
            int ret = DocumentProperties(hwnd, hPrinter, _printerName, pDevmode, IntPtr.Zero, DM_OUT_BUFFER);
            IntPtr hDC = CreateDC(null, _printerName, null, pDevmode);

            // Make Print-Job
            DOCINFO di = new DOCINFO();
            di.cbSize = Marshal.SizeOf(di);
            di.lpszDocName = "IPTS Receipt";

            // Start printing
            StartDoc(hDC, ref di);
            StartPage(hDC);
            LOGFONT lf = new LOGFONT();
            IntPtr hFont = IntPtr.Zero;
            IntPtr oldFont = IntPtr.Zero;
            string str;
            int fontsize, pointsize;

            // Windows font
            pointsize = 10;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = facename;
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);

            List<ReceiptSchema> list = new List<ReceiptSchema>();
            list.Add(new ReceiptSchema() { Key = 0, Value = "Платеж успешно совершен" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "----------------------------------------------------" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "№ квитанции" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["receiptId"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "ФИО плательщика" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["guestFullname"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "ИИН/БИН плательщика" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["guestTaxpayerIdNumber"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Наименование услуги" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["serviceName"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Дата:" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["created"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Получатель" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["hotelName"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Адрес получателя" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["hotelAddress"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "БИН получателя" });
            list.Add(new ReceiptSchema() { Key = 0, Value = jsonValue["agentBusinessNumber"] });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Оплачено с" });
            list.Add(new ReceiptSchema() { Key = 0, Value = string.Format("IPTS {0}", jsonValue["terminalId"]) });
            list.Add(new ReceiptSchema() { Key = 0, Value = "----------------------------------------------------" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Оплаченная сумма" });
            list.Add(new ReceiptSchema() { Key = 0, Value = string.Format("{0} тенге", jsonValue["payment"]) });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Ожидаемая сумма" });
            list.Add(new ReceiptSchema() { Key = 0, Value = string.Format("{0} тенге", jsonValue["expectedPayment"]) });
            list.Add(new ReceiptSchema() { Key = 0, Value = "----------------------------------------------------" });
            list.Add(new ReceiptSchema() { Key = 0, Value = "Обязательно сохраняйте квитанцию." });
            list.Add(new ReceiptSchema() { Key = 0, Value = "СПАСИБО за использование наших услуг!" });

            int x = 0, y = -1;
            foreach (ReceiptSchema pair in list)
            {
                int handler = pair.Key;
                str = pair.Value;

                if (handler == 1)
                {
                    x = 148;
                }
                else if (handler == 0)
                {
                    x = 0;
                    y++;
                }
                TextOut(hDC, x, y * 50, str, str.Length);
            }


            SelectObject(hDC, oldFont);
            DeleteObject(hFont);

            // Cut paper
            pointsize = 12;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Control";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "";
            TextOut(hDC, 0, y * 50, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);

            // End Printing
            EndPage(hDC);
            EndDoc(hDC);

            // Release Printer Device Context
            DeleteDC(hDC);
            Marshal.FreeHGlobal(pDevmode);
            ClosePrinter(hPrinter);
        }

        public void printerFontTest()
        {
            IntPtr hPrinter = IntPtr.Zero;
            OpenPrinter(_printerName, ref hPrinter, IntPtr.Zero);
            // Create Printer Device Con
            IntPtr hwnd = IntPtr.Zero;
            int size = DocumentProperties(hwnd, hPrinter, _printerName, IntPtr.Zero, IntPtr.Zero, 0);

            IntPtr pDevmode = Marshal.AllocHGlobal(size);
            int ret = DocumentProperties(hwnd, hPrinter, _printerName, pDevmode, IntPtr.Zero,
           DM_OUT_BUFFER);
            IntPtr hDC = CreateDC(null, _printerName, null, pDevmode);

            // Make Print-Job
            DOCINFO di = new DOCINFO();
            di.cbSize = Marshal.SizeOf(di);
            di.lpszDocName = "Driver Test Print";

            // Start printing
            StartDoc(hDC, ref di);
            StartPage(hDC);
            LOGFONT lf = new LOGFONT();
            IntPtr hFont = IntPtr.Zero;
            IntPtr oldFont = IntPtr.Zero;
            string str;
            int fontsize, pointsize;
            // Windows font
            pointsize = 15;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Arial";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "Hello World! Здравствуй мир!";
            TextOut(hDC, 10, 10, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);
            // Printer font
            pointsize = 12;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "15 cpi";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "Hello World! Здравствуй мир!";
            TextOut(hDC, 10, 50, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);
            // Barcode
            pointsize = 42;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Code39";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "ABC123456";
            TextOut(hDC, 10, 90, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);
            // Cash drawer
            pointsize = 12;

            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Control";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "A";
            TextOut(hDC, 10, 130, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);
            // Cut paper
            pointsize = 12;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Control";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "P";
            TextOut(hDC, 10, 170, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);
            // Graphic data from NV memory
            pointsize = 12;
            fontsize = (int)((pointsize * 10) * GetDeviceCaps(hDC, LOGPIXELSY) / 720);
            lf.lfHeight = -fontsize;
            lf.lfFaceName = "Control";
            hFont = CreateFontIndirect(lf);
            oldFont = SelectObject(hDC, hFont);
            str = "G";
            TextOut(hDC, 10, 210, str, str.Length);
            SelectObject(hDC, oldFont);
            DeleteObject(hFont);
            // End Printing
            EndPage(hDC);
            EndDoc(hDC);
            // Release Printer Device Context
            DeleteDC(hDC);
            Marshal.FreeHGlobal(pDevmode);
            ClosePrinter(hPrinter);
        }
    }
}


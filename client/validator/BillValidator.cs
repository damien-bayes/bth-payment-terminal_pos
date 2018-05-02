using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Timers;

namespace IPTS.Validator
{
    #region Global Enums
    /// <summary>
    /// Bill Validator: Baud rate - the rate of bit transfer per second 
    /// </summary>
    public enum BaudRate
    {
        low = 9600,
        high = 19200
    }

    /// <summary>
    /// Bill Validator Commands
    /// </summary>
    public enum BillValidatorCommands
    {
        ACK = 0x00,
        NAK = 0xFF,

        // Command for Bill-to-Bill unit to self-reset
        RESET = 0x30,
        // Request for Bill-to-Bill unit set-up status
        GET_STATUS = 0x31,
        // Sets Bill-to-Bill unit Security Mode. Command is followed by set-up data
        SET_SECURITY = 0x32,
        // Request for Bill-to-Bill unit activity Status
        POLL = 0x33,
        // Indicates Bill Type enable or disable. Command is followed by set-up data
        ENABLE_BILL_TYPES = 0x34,
        // Sent by Controller to stack a bill in escrow into drop cassette or into one of the recycling cassettes
        STACK = 0x35,
        // Sent by Controller to return a bill in escrow
        RETURN = 0x36,
        // Request for Model, Serial Number, Software Version of Bill-to-Bill unit, Country ISO code, Asset Number
        IDENTIFICATION = 0x37,
        // Command for holding the Bill-to-Bill unit in Escrow state
        HOLD = 0x38,

        // Request for Bill-to-Bill unit cassette status
        CASSETE_STATUS = 0x3B,
        // Command to dispense a bill of specific type 
        DISPENSE = 0x3C,
        // Command to unload bills from recycling cassette(s) to drop cassette
        UNLOAD = 0x3D,
        // Request for recycling cassette(s) status 
        ESCROW_CASSETTE_STATUS = 0x3E,
        // Command for routing bills to recycling cassette(s)
        ESCROW_CASSETTE_UNLOAD = 0x3F,
        // Assigns cassettes to bill types 
        SET_CASSETTE_TYPE = 0x40,
        // Request for bill type assignments
        GET_BILL_TABLE = 0x41,
        // Command for transition to download mode.  
        DOWNLOAD = 0x50
    }

    /// <summary>
    /// Bill Validator: Bill Recieved Status - 
    /// </summary>
    public enum BillRecievedStatus
    {
        Accepted,
        Rejected
    };

    /// <summary>
    /// Bill Validator: Bill Cassette Status - 
    /// </summary>
    public enum BillCassetteStatus
    {
        Inplace,
        Removed
    };
    #endregion

    #region Delegates
    public delegate void BillReceivedHandler(object Sender, BillReceivedEventArgs e);

    public delegate void BillCassetteHandler(object Sender, BillCassetteEventArgs e);

    public delegate void BillStackingHandler(object Sender, BillStackedEventArgs e);
    #endregion

    public sealed class BillValidator : IDisposable
    {
        private const int POLL_TIMEOUT = 200;
        private const int WAITING_HANDLER_TIMEOUT = 10000;

        private byte[] ENABLE_BILL_TYPES_WITH_ESCROW = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        private EventWaitHandle _synchronizedSerialPort;
        private List<byte> _receivedBytes;

        private SerialPort _serialPort;
        private System.Timers.Timer _timer;

        private object _locker;

        BillCassetteStatus _cassetteStatus = BillCassetteStatus.Inplace;

        private BillValidatorException _billValidatorException;
        private int _lastException = 0;

        #region Events
        public event BillReceivedHandler BillReceived;

        private void OnBillReceived(BillReceivedEventArgs e)
        {
            if (BillReceived != null)
            {
                BillReceived(this, new BillReceivedEventArgs(e.Status, e.Value, e.RejectedReason));
            }
        }

        public event BillCassetteHandler BillCassetteStatusEvent;

        private void OnBillCassetteStatus(BillCassetteEventArgs e)
        {
            if (BillCassetteStatusEvent != null)
            {
                BillCassetteStatusEvent(this, new BillCassetteEventArgs(e.Status));
            }
        }

        public event BillStackingHandler BillStacking;

        private void OnBillStacking(BillStackedEventArgs e)
        {
            if (BillStacking != null)
            {
                bool cancel = false;
                foreach (BillStackingHandler subscriber in BillStacking.GetInvocationList())
                {
                    subscriber(this, e);

                    if (e.Cancel)
                    {
                        cancel = true;
                        break;
                    }
                }

                _returnBill = cancel;
            }
        }
        #endregion

        #region Properties
        public IDictionary<string, object> ExceptionMessage
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "code", _lastException },
                    { "message", _billValidatorException.ExceptionList[_lastException] }
                };
            }
        }

        private bool _IsEnableBills = false;
        public bool IsEnableBills
        {
            get { return _IsEnableBills; }
        }

        private bool _isPowerUp = false;
        public bool IsPowerUp
        {
            get { return _isPowerUp; }
        }

        private bool _isOpen = false;
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        private bool _isListening = false;
        public bool IsListening
        {
            get { return _isListening; }
        }

        private bool _returnBill;

        public bool ReturnBill
        {
            get { return _returnBill; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        public BillValidator(string portName, int baudRate)
        {
            _billValidatorException = new BillValidatorException();

            /*
             * Data format
             * 
             * Baud Rate: 9600 bps/19200 bps (no negotiation, hardware selectable)
             * Start bit: 1
             * Data bit: 8 (bit 0 = LSB, bit 0 sent first)
             * Parity: Parity none 
             * Stop bit: 1
             */

            _serialPort = new SerialPort()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            _serialPort.DataReceived += _serialPort_DataReceived;

            _receivedBytes = new List<byte>();

            _synchronizedSerialPort = new EventWaitHandle(false, EventResetMode.AutoReset);

            _timer = new System.Timers.Timer();
            _timer.Interval = POLL_TIMEOUT;
            _timer.Enabled = true;
            _timer.Elapsed += _timer_Elapsed;

            _locker = new object();

            _isListening = false;
        }

        /// <summary>
        /// Open the serial port of the bill validator
        /// </summary>
        private int OpenSerialPort()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();

            try
            {
                _serialPort.Open();
                _isOpen = true;
            }
            catch
            {
                _isOpen = false;
                _lastException = 100010;
            }

            return _lastException;
        }

        /// <summary>
        /// Power up the bill validator
        /// </summary>
        public void PowerUp()
        {
            List<byte> bytesResult = null;

            if (OpenSerialPort() == 0)
            {
                bytesResult = SendCommand(BillValidatorCommands.POLL).ToList();

                if (bytesResult.Count() > 0)
                {
                    if (CheckPollOnException(bytesResult.ToArray()))
                        SendCommand(BillValidatorCommands.NAK);

                    SendCommand(BillValidatorCommands.ACK);

                    bytesResult = SendCommand(BillValidatorCommands.RESET).ToList();

                    bytesResult = SendCommand(BillValidatorCommands.POLL).ToList();

                    if (CheckPollOnException(bytesResult.ToArray()))
                        SendCommand(BillValidatorCommands.NAK);

                    SendCommand(BillValidatorCommands.ACK);

                    bytesResult = this.SendCommand(BillValidatorCommands.GET_STATUS).ToList();
                }
                else
                {
                    _lastException = 100030;

                    Logger.Log(Logger.Level.Warning, string.Format("While attempting to connect to the bill validator, the error has occurred. Code: {0}", _lastException));
                }
            }
        }

        public void EnableBillValidator()
        {
            List<byte> bytesResult = null;

            try
            {
                lock (_locker)
                {
                    _IsEnableBills = true;

                    bytesResult = SendCommand(BillValidatorCommands.ENABLE_BILL_TYPES, ENABLE_BILL_TYPES_WITH_ESCROW).ToList();

                    if (bytesResult[3] != 0x00)
                    {

                    }

                    bytesResult = this.SendCommand(BillValidatorCommands.POLL).ToList();
                    SendCommand(BillValidatorCommands.ACK);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
        }

        /// <summary>
        /// Timer for accepting payments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            try
            {
                lock (_locker)
                {
                    List<byte> bytesResult = null;

                    bytesResult = SendCommand(BillValidatorCommands.POLL).ToList();

                    if (bytesResult[3] != 0x14)
                    {
                        switch (bytesResult[3])
                        {
                            // Accepting 
                            case 0x15:
                                SendCommand(BillValidatorCommands.ACK);
                                break;
                            // Escrow position - Rejecting
                            case 0x1C:
                                SendCommand(BillValidatorCommands.ACK);
                                OnBillReceived(new BillReceivedEventArgs(BillRecievedStatus.Rejected, 0, ""));
                                break;
                            // Escrow position
                            case 0x80:
                                SendCommand(BillValidatorCommands.ACK);
                                OnBillStacking(new BillStackedEventArgs(CashCodeTable(bytesResult[4])));

                                if (this._returnBill)
                                {
                                    bytesResult = this.SendCommand(BillValidatorCommands.RETURN).ToList();
                                    _returnBill = false;
                                }
                                else
                                {
                                    bytesResult = this.SendCommand(BillValidatorCommands.STACK).ToList();
                                }

                                break;
                            // Stacking
                            case 0x17:
                                SendCommand(BillValidatorCommands.ACK);
                                break;
                            // Bill stacked
                            case 0x81:
                                SendCommand(BillValidatorCommands.ACK);

                                OnBillReceived(new BillReceivedEventArgs(BillRecievedStatus.Accepted, CashCodeTable(bytesResult[4]), ""));

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(string.Format("Banknote - {0} was obtained.", CashCodeTable(bytesResult[4])));
                                Console.ResetColor();
                                break;
                            // Returning
                            case 0x18:
                                SendCommand(BillValidatorCommands.ACK);

                                break;
                            // Bill returning
                            case 0x82:
                                SendCommand(BillValidatorCommands.ACK);

                                break;
                            // Drop cassette out of position
                            case 0x42:
                                if (_cassetteStatus != BillCassetteStatus.Removed)
                                {
                                    _cassetteStatus = BillCassetteStatus.Removed;
                                    OnBillCassetteStatus(new BillCassetteEventArgs(_cassetteStatus));
                                }
                                break;
                            // Initialize
                            case 0x13:
                                if (_cassetteStatus == BillCassetteStatus.Removed)
                                {
                                    _cassetteStatus = BillCassetteStatus.Inplace;
                                    OnBillCassetteStatus(new BillCassetteEventArgs(_cassetteStatus));
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
            finally
            {
                if (!_timer.Enabled && _isListening)
                {
                    _timer.Start();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartListening()
        {
            _isListening = true;
            _timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopListening()
        {
            _isListening = false;
            _timer.Stop();
        }

        public void DisableBillValidator()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytesResult"></param>
        /// <returns></returns>
        private bool CheckPollOnException(byte[] bytesResult)
        {
            bool isException = false;

            switch (bytesResult[3])
            {
                // Illegal command
                case 0x30:
                    isException = true;
                    break;
                // Drop cassette full
                case 0x41:
                    isException = true;
                    break;
                // Drop cassette out of position
                case 0x42:
                    isException = true;
                    break;
                // Validator jammed
                case 0x43:
                    isException = true;
                    break;
                // Drop cassette jammed
                case 0x44:
                    isException = true;
                    break;
                // Cheated
                case 0x45:
                    isException = true;
                    break;
                // Pause
                case 0x46:
                    isException = true;
                    break;
                // Generic failure codes
                case 0x47:
                    switch (bytesResult[4])
                    {
                        // Stack motor failure
                        case 0x50:
                            break;
                        // Transport motor speed failure
                        case 0x51:
                            break;
                        // Transport motor failure
                        case 0x52:
                            break;
                        // Aligning motor failure
                        case 0x53:
                            break;
                        // Initial cassette status failure
                        case 0x54:
                            break;
                        // Optic canal failure
                        case 0x55:
                            break;
                        // Magnetic canal failure
                        case 0x56:
                            break;
                        // Capacitance canal failure
                        case 0x5F:
                            break;
                    }
                    isException = true;
                    break;
            }

            return isException;
        }

        /// <summary>
        /// Send relevant commands to the bill validator
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] SendCommand(BillValidatorCommands command, byte[] data = null)
        {
            if (command == BillValidatorCommands.ACK || command == BillValidatorCommands.NAK)
            {
                byte[] bytes = null;

                switch (command)
                {
                    case BillValidatorCommands.ACK: bytes = BillValidatorPackage.CreateResponse(BillValidatorPackage.ResponseType.ACK); break;
                    case BillValidatorCommands.NAK: bytes = BillValidatorPackage.CreateResponse(BillValidatorPackage.ResponseType.NAK); break;
                }

                if (bytes != null)
                    _serialPort.Write(bytes, 0, bytes.Length);

                return null;
            }
            else
            {
                BillValidatorPackage package = new BillValidatorPackage();
                package.Command = (byte)command;

                if (data != null)
                    package.Data = data;

                byte[] bytesCommand = package.GetBytes();
                _serialPort.Write(bytesCommand, 0, bytesCommand.Length);

                _synchronizedSerialPort.WaitOne(WAITING_HANDLER_TIMEOUT);
                _synchronizedSerialPort.Reset();

                byte[] result = _receivedBytes.ToArray();

                // Add CheckCRC
                if (result.Length == 0)
                {
                    Console.WriteLine();
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private int CashCodeTable(byte code)
        {
            /*
             * Alleged bytes: 02, 03, 06, 41, 4F, D1
             * 
             * 0 = 0
             * 1 = 200 KZ
             * 2 = 500 KZ
             * 3 = 1000 KZ
             * 4 = 2000 KZ
             * 5 = 5000 KZ
             * 6 = 10000 KZ
             * 7-22 = 0
             * 23 = 1-BAR
             */
            int result = 0;

            switch (code)
            {
                case 0x01: result = 200; break; // 200 KZ
                case 0x02: result = 500; break; // 500 KZ
                case 0x03: result = 1000; break; // 1000 KZ
                case 0x04: result = 2000; break; // 2000 KZ
                case 0x05: result = 5000; break; // 5000 KZ
                case 0x06: result = 10000; break; // 10000 KZ
            }
            return result;
        }

        /// <summary>
        /// Event of receiving data (Defect)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(string.Format("Working function name: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));

            Thread.Sleep(100);
            _receivedBytes.Clear();

            // Start reading bytes
            while(_serialPort.BytesToRead > 0)
            {
                _receivedBytes.Add((byte)_serialPort.ReadByte());
            }

            _synchronizedSerialPort.Set();
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(50);
                _receivedBytes.Clear();

                int bytes = _serialPort.BytesToRead;
                byte[] comBuffer = new byte[bytes];
                _serialPort.Read(comBuffer, 0, bytes);

                for (int i = 0; i < comBuffer.Length; i++)
                {
                    _receivedBytes.Add(comBuffer[i]);
                    Console.Write(string.Format("{0}. {1};", i, comBuffer[i]));
                }

                _synchronizedSerialPort.Set();
            }
            catch (System.Exception exception)
            {
                Logger.Log(exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }
    }
}

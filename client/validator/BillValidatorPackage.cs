using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTS.Validator
{
    public sealed class BillValidatorPackage
    {
        public enum ResponseType
        {
            ACK,
            NAK
        };

        // For calculating CRC
        private const int _polynomial = 0x08408;
        // Fixed synchronization bit
        private const byte _synchronization = 0x02;
        // Peripheral address
        private const byte _address = 0x03;

        #region Properties
        private byte _command;
        public byte Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (value.Length + 5 > 250) { }
                else
                {
                    // In doubt
                    _data = new byte[value.Length];
                    _data = value;
                }
            }
        }
        #endregion

        #region Constructor
        public BillValidatorPackage() { }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] CreateResponse(ResponseType type)
        {
            List<byte> buffer = new List<byte>();
            // 1. The synchronization flag is 0x02 (2) as default
            buffer.Add(_synchronization);

            // 2. The device address is 0x03 (3) as default
            buffer.Add(_address);

            // 3. The package length is 0x06 (6) as default
            buffer.Add(0x06);

            // 4. Data
            if (type == ResponseType.ACK)
                buffer.Add(0x00);
            else if (type == ResponseType.NAK)
                buffer.Add(0xFF);

            // 5. CRC
            byte[] CRC = GetCRC(buffer.ToArray());

            byte[] package = new byte[buffer.Count + CRC.Length];
            buffer.ToArray().CopyTo(package, 0);
            CRC.CopyTo(package, buffer.Count);

            return package;
        }

        public byte[] GetBytes()
        {
            List<byte> buffer = new List<byte>();

            // 1. synchronization 2. address 3. length 4. command
            buffer.Add(_synchronization);
            buffer.Add(_address);

            int length = GetLength();

            if (length > 250)
                buffer.Add(0);
            else
                buffer.Add(Convert.ToByte(length));

            buffer.Add(_command);

            if (_data != null)
            {
                for (int i = 0; i < _data.Length; i++)
                    buffer.Add(_data[i]);
            }

            byte[] CRC = GetCRC(buffer.ToArray());

            byte[] result = new byte[buffer.Count + CRC.Length];
            buffer.ToArray().CopyTo(result, 0);
            CRC.CopyTo(result, buffer.Count);

            return result;
        }

        /// <summary>
        /// Package length
        /// </summary>
        /// <returns></returns>
        public int GetLength()
        {
            return (_data == null ? 0 : _data.Length) + 6;
        }

        /// <summary>
        /// Estimated amount calculation (Defect)
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        /*
        private static int GetCRC16(byte[] dataBuffer, int dataSize)
        {
            Console.WriteLine(string.Format("Working function name: {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));

            int temporaryCRC, CRC = 0;

            for (int i = 0; i < dataSize; i++)
            {
                temporaryCRC = CRC ^ dataBuffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((temporaryCRC & 0x0001) != 0)
                    {
                        temporaryCRC >>= 1;
                        temporaryCRC ^= _polynomial;
                    }
                    else
                        temporaryCRC >>= 1;
                }
                CRC = temporaryCRC;
            }
            return CRC;
        }*/

        private static byte[] GetCRC(byte[] paquete)
        {
            const ushort POLYNOMIAL = 0x08408;
            byte[] arrayCRC = new byte[4];
            int CRC = 0;
            for (int i = 0; i < paquete.Length; ++i)
            {
                CRC ^= paquete[i];

                for (int j = 0; j < 8; ++j)
                {
                    if ((CRC & 0x0001) != 0)
                    {
                        CRC >>= 1;
                        CRC ^= POLYNOMIAL;
                    }
                    else
                        CRC >>= 1;
                }
            }
            arrayCRC = BitConverter.GetBytes(CRC);

            // Los dos primeros bytes corresponden al CRC
            return new byte[] { arrayCRC[0], arrayCRC[1] };
        }
    }
}

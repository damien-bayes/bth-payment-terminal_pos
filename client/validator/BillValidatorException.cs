using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTS.Validator
{
    public sealed class BillValidatorException
    {
        public Dictionary<int, string> ExceptionList { get; private set; }

        public BillValidatorException()
        {
            ExceptionList = new Dictionary<int, string>();

            ExceptionList.Add(0, string.Empty);

            ExceptionList.Add(100000, "Неизвестная ошибка");
            
            ExceptionList.Add(100010, "The serial port is in an invalid state or an attempt to set the state of the underlying port failed.");
            ExceptionList.Add(100020, "Com-порт не открыт");
            ExceptionList.Add(100030, "The banknote acceptor is in an invalid state. An attempt to send the command to the banknote acceptor failed.");
            ExceptionList.Add(100040, "Ошибка отпраки команды включения купюроприемника. От купюроприемника не получена команда POWER UP.");
            ExceptionList.Add(100050, "Ошибка отпраки команды включения купюроприемника. От купюроприемника не получена команда ACK.");
            ExceptionList.Add(100060, "Ошибка отпраки команды включения купюроприемника. От купюроприемника не получена команда INITIALIZE.");
            ExceptionList.Add(100070, "Ошибка проверки статуса купюроприемника. Cтекер снят.");
            ExceptionList.Add(100080, "Ошибка проверки статуса купюроприемника. Стекер переполнен.");
            ExceptionList.Add(100090, "Ошибка проверки статуса купюроприемника. В валидаторе застряла купюра.");
            ExceptionList.Add(100100, "Ошибка проверки статуса купюроприемника. В стекере застряла купюра.");
            ExceptionList.Add(100110, "Ошибка проверки статуса купюроприемника. Фальшивая купюра.");
            ExceptionList.Add(100120, "Ошибка проверки статуса купюроприемника. Предыдущая купюра еще не попала в стек и находится в механизме распознавания.");

            ExceptionList.Add(100130, "Ошибка работы купюроприемника. Сбой при работе механизма стекера.");
            ExceptionList.Add(100140, "Ошибка работы купюроприемника. Сбой в скорости передачи купюры в стекер.");
            ExceptionList.Add(100150, "Ошибка работы купюроприемника. Сбой передачи купюры в стекер.");
            ExceptionList.Add(100160, "Ошибка работы купюроприемника. Сбой механизма выравнивания купюр.");
            ExceptionList.Add(100170, "Ошибка работы купюроприемника. Сбой в работе стекера.");
            ExceptionList.Add(100180, "Ошибка работы купюроприемника. Сбой в работе оптических сенсоров.");
            ExceptionList.Add(100190, "Ошибка работы купюроприемника. Сбой работы канала индуктивности.");
            ExceptionList.Add(100200, "Ошибка работы купюроприемника. Сбой в работе канала проверки заполняемости стекера.");

            // Ошибки распознования купюры
            ExceptionList.Add(0x60, "Rejecting due to Insertion");
            ExceptionList.Add(0x61, "Rejecting due to Magnetic");
            ExceptionList.Add(0x62, "Rejecting due to Remained bill in head");
            ExceptionList.Add(0x63, "Rejecting due to Multiplying");
            ExceptionList.Add(0x64, "Rejecting due to Conveying");
            ExceptionList.Add(0x65, "Rejecting due to Identification1");
            ExceptionList.Add(0x66, "Rejecting due to Verification");
            ExceptionList.Add(0x67, "Rejecting due to Optic");
            ExceptionList.Add(0x68, "Rejecting due to Inhibit");
            ExceptionList.Add(0x69, "Rejecting due to Capacity");
            ExceptionList.Add(0x6A, "Rejecting due to Operation");
            ExceptionList.Add(0x6C, "Rejecting due to Length");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class CheckDate
    {
        public bool IsValidDate(DateOnly date)
        {
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;
            if (year < 1)
            {
                return false;
            }

            // Kiểm tra tháng có hợp lệ không (1 - 12)
            if (month < 1 || month > 12)
            {
                return false;
            }

            // Xác định số ngày tối đa trong tháng
            int maxDaysInMonth;
            switch (month)
            {
                case 2: // Tháng Hai
                    maxDaysInMonth = DateTime.IsLeapYear(year) ? 29 : 28;
                    break;
                case 4: // Tháng Tư
                case 6: // Tháng Sáu
                case 9: // Tháng Chín
                case 11: // Tháng Mười Một
                    maxDaysInMonth = 30;
                    break;
                default:
                    maxDaysInMonth = 31;
                    break;
            }

            // Kiểm tra ngày có hợp lệ không (1 đến maxDaysInMonth)
            if (day < 1 || day > maxDaysInMonth)
            {
                return false;
            }

            return true;
        }

    }
}

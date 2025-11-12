using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseFarm_BackEnd.Test.EnumTest
{
    public class StatusTest
    {
        // --- SUCCESS ---
        public const int SUCCESS_CREATE_CODE = 1;
        public const int SUCCESS_READ_CODE = 1;
        public const int SUCCESS_UPDATE_CODE = 1;

        public const string SUCCESS_CREATE_MSG = "Thêm thành công.";
        public const string SUCCESS_READ_MSG = "Đọc dữ liệu thành công.";
        public const string SUCCESS_UPDATE_MSG = "Cập nhật thành công.";

        // --- FAIL ---
        public const int FAIL_CREATE_CODE = 0;
        public const int FAIL_READ_CODE = 0;

        public const string FAIL_CREATE_MSG = "Thêm thất bại.";
        public const string FAIL_READ_MSG = "Đọc dữ liệu thất bại.";

        // --- ERROR ---
        public const int ERROR_EXCEPTION = -1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Const
    {
        #region Error Codes
        public static int ERROR_EXCEPTION = -4;
        #endregion

        #region Success Codes
        public static int SUCCESS_CREATE_CODE = 1;
        public static string SUCCESS_CREATE_MSG = "Create Data Success";
        public static int SUCCESS_READ_CODE = 1;
        public static string SUCCESS_READ_MSG = "Get Data Success";
        public static int SUCCESS_UPDATE_CODE = 1;
        public static string SUCCESS_UPDATE_MSG = "Update Data Success";
        public static int SUCCESS_DELETE_CODE = 1;
        public static string SUCCESS_DELETE_MSG = "Delete Data Success";
        #endregion

        #region Fail Codes
        public static int FAIL_CREATE_CODE = -1;
        public static string FAIL_CREATE_MSG = "Create Data Fail";
        public static int FAIL_READ_CODE = -1;
        public static string FAIL_READ_MSG = "Get Data Fail";
        public static int FAIL_UPDATE_CODE = -1;
        public static string FAIL_UPDATE_MSG = "Update Data Fail";
        public static int FAIL_DELETE_CODE = -1;
        public static string FAIL_DELETE_MSG = "Delete Data Fail";
        #endregion

        #region Warning Codes
        public static int WARNING_NO_DATA_CODE = 4;
        public static string WARNING_NO_DATA_MSG = "No Data";
        #endregion
    }
}

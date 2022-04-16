using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    public class ReturnData
    {
        public Boolean isSuccess { get; set; }
        public String errorText { get; set; }
        public Object data { get; set; }

        public ReturnData(Boolean isSuccess, Object data, String errorText)
        {
            this.isSuccess = isSuccess;
            this.errorText = errorText;
            this.data = data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Admin.Contracts.Rights
{
    public class CreateOrUpdateRightRequest
    {
        public string RightName { get; set; }
        public string RightDescription { get; set; }
    }
}

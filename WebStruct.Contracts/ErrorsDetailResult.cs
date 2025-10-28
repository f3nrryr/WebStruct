using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Contracts
{
    public class ErrorsDetailResult
    {
        public string Title { get; set; } = "Возникла ошибка";
        public string[] Errors { get; set; }
    }
}

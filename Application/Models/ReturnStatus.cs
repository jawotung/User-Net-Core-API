using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ReturnStatus
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class ReturnStatusData<T>(T data) : ReturnStatus
    {
        public T Data { get; set; } = data;
    }
    public class ReturnStatusList<T>(List<T> data) : ReturnStatus
    {
        public List<T> Data { get; set; } = data;
    }
}

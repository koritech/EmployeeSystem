using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeConsumerService.Model
{
    public class KafkaConsumerSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string AutoOffsetReset { get; set; } = "Latest";
        public bool EnableAutoCommit { get; set; } = true;
        public int SessionTimeoutMs { get; set; } = 10000;

        public string Topic { get; set; } = string.Empty;
    }

}

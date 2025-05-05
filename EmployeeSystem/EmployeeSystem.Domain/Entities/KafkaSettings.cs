using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSystem.Domain.Entities
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string Acks { get; set; } 
        public bool EnableIdempotence { get; set; } = true;

        public string EmployeeTopic { get; set; }
    }

}

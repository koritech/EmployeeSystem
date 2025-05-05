using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSystem.Domain.Entities
{
    namespace EmployeeSystem.Services.Validation
    {
        public class ValidationResult
        {
            public bool IsValid => !Errors.Any();
            public List<string> Errors { get; } = new();
        }
    }

}

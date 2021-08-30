using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class CRONExpression
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string CronExpression { get; set; }
        public int IsActive { get; set; }
        public string CronResult { get; set; }
        public DateTime LastExecution { get; set; }
    }
}

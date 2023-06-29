using Finance.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.BaseLibrary
{
    public class ProcessMaintenanceDto : ResultDto
    {
        public long Id { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public string ProcessNumber { get; set; }

        public string ProcessName { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

        public decimal? ProcessHoursenterId { get; set; }
    }
}

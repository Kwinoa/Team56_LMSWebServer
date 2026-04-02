using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public short Score { get; set; }
        public string SubmissionContents { get; set; } = null!;
        public DateTime Time { get; set; }
        public string UId { get; set; } = null!;
        public uint AssignmentId { get; set; }
        public uint SubmissionId { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}

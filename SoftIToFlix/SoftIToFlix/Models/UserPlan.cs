using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftIToFlix.Models
{
	public class UserPlan
	{
		
		public long UserId { get; set; }
		public short PlanId { get; set; }
		[Column(TypeName="date")]
		public DateTime StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        [ForeignKey("UserId")]
		public SoftIToFlixUser? SoftIToFlixUser { get; set; }
        [ForeignKey("PlanId")]
        public Plan? Plan { get; set; }

    }
}


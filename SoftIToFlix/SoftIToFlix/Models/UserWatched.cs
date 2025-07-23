using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftIToFlix.Models
{
	public class UserWatched
	{
		public long UserId { get; set; }
		public long EpisodeId { get; set; }

		[ForeignKey("UserId")]
		public SoftIToFlixUser? SoftIToFlixUser { get; set; }
        [ForeignKey("EpisodeId")]
        public Episode? Episode { get; set; }
    }
}


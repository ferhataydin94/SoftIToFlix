using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftIToFlix.Models
{
	public class UserFavorite
	{
        public long UserId { get; set; }
        public int MediaId { get; set; }
        [ForeignKey("UserId")]
        public SoftIToFlixUser? SoftIToFlixUser { get; set; }
        [ForeignKey("MediaId")]
        public Media? Media { get; set; }
    }
}


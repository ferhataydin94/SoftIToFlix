using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SoftIToFlix.Models
{
	public class Director : Person
	{
        public bool Passive { get; set; }
    }
}


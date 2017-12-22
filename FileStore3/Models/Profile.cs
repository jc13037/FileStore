using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStore3.Models
{
    public class Profile
    {
        public int ProfileID { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(60, MinimumLength = 1)]
        public string ProfileName { get; set; }

        public string ApplicationUserID { get; set; }

        public IList<File> Files { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStore3.Models
{
    public class File
    {
        public int FileID { get; set; }
         
        [Required]
        [Display(Name = "Title")]
        [StringLength(150, MinimumLength = 1)]
        public string FileTitle { get; set; }

        public string FilePath { get; set; }

        [Display(Name = "Date Uploaded")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Range(typeof(DateTime), "1/1/1970", "1/1/2049",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
        [DataType(DataType.Date)]
        public DateTime FileUploadDate { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1/1/1970", "1/1/2049",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
        [DataType(DataType.Date)]
        public DateTime FileCreateDate { get; set; }

        [Display(Name = "File Size (KB)")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public long FileSize { get; set; }

        public int ProfileID { get; set; }
    }
}

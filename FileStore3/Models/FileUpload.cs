using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace FileStore3.Models
{
    public class FileUpload
    {
        [Required]
        [Display(Name = "Title")]
        [StringLength(60, MinimumLength = 3)]
        public string FileUploadTitle { get; set; }

        [Required]
        [Display(Name = "Upload File")]
        public IFormFile FileUploadFile { get; set; }

        public int ProfileID { get; set; }

        [Display(Name = "Date Created (UTC)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FileUploadCreateDate { get; set; }
    }
}

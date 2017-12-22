using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FileStore3.Data;
using FileStore3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.Extensions.Configuration;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.WindowsAzure.Storage.Table;

namespace FileStore3.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<UploadModel> _logger;

        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public UploadModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<UploadModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public FileUpload FileUpload { get; set; }

        [BindProperty]
        public Profile Profile { get; set; }

        public IList<Profile> ProfileList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProfileList = await _context.Profile.Where(p => p.ApplicationUserID == _userManager.GetUserId(User)).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUploadFileAsync()
        {
            if (FileUpload.FileUploadFile.Length > 1000000)
            {
                return RedirectToPage("./Profiles");
            }
            else
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = new CloudStorageAccount(
                    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                        "filestoragejc13037",
                        "G6JLPODZD2XOQaM2Z6Ptd8PLPkluAsjYZAOJ87Xy19wx9P6deJiflPlIdnsANkKinAOUgEr7Cu9eHEYQqylESA=="), true);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference(_userManager.GetUserId(User));

                // Create the container if it doesn't already exist.
                await container.CreateIfNotExistsAsync();

                // Retrieve reference to a blob named "myblob".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileUpload.FileUploadTitle + " (" + DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss") + ")" + "." + FileUpload.FileUploadFile.FileName.Substring(FileUpload.FileUploadFile.FileName.LastIndexOf('.')));

                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fileStream = new BinaryReader(FileUpload.FileUploadFile.OpenReadStream()))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream.BaseStream);
                }

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Get a reference to a table named "fileTable"
                CloudTable table = tableClient.GetTableReference("fileTable");

                // Create the CloudTable if it does not exist
                await table.CreateIfNotExistsAsync();

                //Save File data in the database
                var file = new Models.File()
                {
                    FileTitle = FileUpload.FileUploadTitle,
                    FilePath = blockBlob.Uri.ToString(),
                    FileSize = (FileUpload.FileUploadFile.Length / 1000),
                    FileUploadDate = DateTime.UtcNow,
                    FileCreateDate = FileUpload.FileUploadCreateDate,
                    ProfileID = FileUpload.ProfileID
                };

                _context.File.Add(file);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Profiles");
            }
        }

        public async Task<IActionResult> OnPostCreateProfileAsync()
        {
            Profile.ApplicationUserID = _userManager.GetUserId(User);

            _context.Profile.Add(Profile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Upload");
        }
    }
}
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FileStore3.Data;
using FileStore3.Models;
using FileStore3.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Http.Internal;
using System.Web;

namespace FileStore3.Pages
{
    public class EditFileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public EditFileModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.File EditFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int fileID)
        {
            EditFile = await _context.File.Where(p => p.FileID == fileID).SingleAsync();

            return Page();
        }

        [HttpGet]
        public async Task<FileResult> OnGetDownloadFileAsync(int fileID)
        {
            var downloadFile = await _context.File.Where(p => p.FileID == fileID).SingleAsync();

            string[] fileURI = downloadFile.FilePath.Split('/');
            string blobFileName = fileURI[fileURI.Length - 1];
            string blobContainerName = fileURI[fileURI.Length - 2];

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    "filestoragejc13037",
                    "G6JLPODZD2XOQaM2Z6Ptd8PLPkluAsjYZAOJ87Xy19wx9P6deJiflPlIdnsANkKinAOUgEr7Cu9eHEYQqylESA=="), true);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFileName);

            // Save the blob contents to a file named "myfile".

            return File(await blockBlob.OpenReadAsync(), "application/octet-stream", blobFileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteFileAsync(int fileID)
        {
            EditFile = await _context.File.Where(p => p.FileID == fileID).SingleAsync();

            _context.File.Remove(EditFile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profiles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostEditFileAsync()
        {
            _context.Attach(EditFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(EditFile.FileID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Profiles");
        }

        private bool FileExists(int fileID)
        {
            return _context.File.Any(p => p.FileID == fileID);
        }
    }
}
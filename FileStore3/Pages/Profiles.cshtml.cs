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
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Http.Internal;
using System.Web;

namespace FileStore3.Pages
{
    public class ProfilesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Profile Profile { get; set; }

        public IList<Profile> ProfileList { get; set; }

        [BindProperty]
        public Models.File EditFile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProfileList = await _context.Profile.Where(p => p.ApplicationUserID == _userManager.GetUserId(User)).Include(p => p.Files).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateProfileAsync()
        {
            Profile.ApplicationUserID = _userManager.GetUserId(User);

            _context.Profile.Add(Profile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profiles");
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
            var deleteFile = await _context.File.Where(p => p.FileID == fileID).SingleAsync();

            string[] fileURI = deleteFile.FilePath.Split('/');
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

            // Delete the blob.
            await blockBlob.DeleteAsync();

            EditFile = await _context.File.Where(p => p.FileID == fileID).SingleAsync();

            _context.File.Remove(EditFile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profiles");
        }
    }
}
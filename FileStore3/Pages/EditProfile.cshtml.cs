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

namespace FileStore3.Pages
{
    public class EditProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public EditProfileModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Profile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync(int profileID)
        {
            Profile = await _context.Profile.Where(p => p.ProfileID == profileID).Include(p => p.Files).SingleAsync();

            return Page();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteProfileAsync(int profileID)
        {
            Profile = await _context.Profile.Where(p => p.ProfileID == profileID).SingleAsync();

            _context.Profile.Remove(Profile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profiles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostEditProfileAsync()
        {
            _context.Attach(Profile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(Profile.ProfileID))
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

        private bool ProfileExists(int profileID)
        {
            return _context.Profile.Any(p => p.ProfileID == profileID);
        }
    }
}
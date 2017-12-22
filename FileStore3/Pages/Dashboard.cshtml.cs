using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileStore3.Data;
using FileStore3.Models;

namespace FileStore3.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly FileStore3.Data.ApplicationDbContext _context;

        public DashboardModel(FileStore3.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Profile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile = await _context.Profile.SingleOrDefaultAsync(m => m.ProfileID == id);

            if (Profile == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSD605ClassProject.Data;
using DSD605ClassProject.Models;

namespace DSD605ClassProject.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly DSD605ClassProject.Data.ApplicationDbContext _context;

        public IndexModel(DSD605ClassProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Movie != null)
            {
                Movie = await _context.Movie.ToListAsync();
            }
        }
    }
}

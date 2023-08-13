using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DSD605ClassProject.Data;
using DSD605ClassProject.Models;

namespace DSD605ClassProject.Pages.Casts
{
    public class IndexModel : PageModel
    {
        private readonly DSD605ClassProject.Data.ApplicationDbContext _context;

        public IndexModel(DSD605ClassProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cast> Cast { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Cast != null)
            {
                Cast = await _context.Cast
                .Include(c => c.Movie).ToListAsync();
            }
        }
    }
}

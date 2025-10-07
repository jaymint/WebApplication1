using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BestReceiversModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public BestReceiversModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public class ReceiverStats
    {
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public int ShipmentCount { get; set; }
        public decimal TotalPrice { get; set; }
    }

    [BindProperty(SupportsGet = true)]
    public string Timeline { get; set; } = "today";

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }


    public List<ReceiverStats> BestReceivers { get; set; }

    public async Task OnGetAsync()
    {
        DateTime fromDate = Timeline switch
        {
            "today" => DateTime.Today,
            "week" => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),
            "month" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            _ => DateTime.MinValue
        };

        var baseQuery = _context.Shipments
            .Where(s => s.ShipmentDateTime >= fromDate)
            .GroupBy(s => new { s.ReceiverId, s.Receiver.Name })
            .Select(g => new ReceiverStats
            {
                ReceiverId = g.Key.ReceiverId,
                ReceiverName = g.Key.Name,
                ShipmentCount = g.Count(),
                TotalPrice = g.Sum(x => x.Price)
            });

        TotalCount = await baseQuery.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        BestReceivers = await baseQuery
            .OrderByDescending(r => r.ShipmentCount)
            .ThenBy(r => r.ReceiverName)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}

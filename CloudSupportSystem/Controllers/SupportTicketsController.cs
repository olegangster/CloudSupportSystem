
using CloudSupportSystem.Data;
using CloudSupportSystem.Models;
using CloudSupportSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudSupportSystem.Controllers
{
    public class SupportTicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SqsService _sqsService;
        private readonly ILogger<SupportTicketsController> _logger;

        public SupportTicketsController(
            ApplicationDbContext context,
            SqsService sqsService,
            ILogger<SupportTicketsController> logger)
        {
            _context = context;
            _sqsService = sqsService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _context.SupportTickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.SupportTickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportTicket ticket)
        {
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            ticket.Id = 0;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.Status = "New";

            await _context.SupportTickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            try
            {
                await _sqsService.SendTicketCreatedAsync(
                    ticket.Id,
                    ticket.Subject,
                    ticket.Priority);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ticket {TicketId} was saved, but sending its SQS message failed.",
                    ticket.Id);

                TempData["SqsWarning"] =
                    "Ticket was saved, but the notification could not be sent to AWS SQS.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(
            int id,
            string status)
        {
            var allowedStatuses = new[]
            {
                "New",
                "In Progress",
                "Resolved"
            };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest();
            }

            var ticket = await _context.SupportTickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = status;

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = ticket.Id });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Черга.Data;
using Черга.Models;

public class QueueController : Controller
{
    private readonly ApplicationDbContext _context;

    public QueueController(ApplicationDbContext context)
    {
        _context = context;
    }
    //[Authorize]
    public IActionResult Details(int id)
    {
        var queue = _context.Queues
            .Include(q => q.Entries)
            .ThenInclude(e => e.User)
            .FirstOrDefault(q => q.Id == id);
        return View(queue);
    }

    //[HttpPost]
    //[Authorize]
    public IActionResult Create(/*Queue queue*/)
    {
        //ViewBag.CreatorId = _context.Groups.Where(gm => gm.Id == queue.GroupId).Select(gm => gm.CreatorId);
        //// Check if the queue type is Random
        //if (queue.Type == QueueType.Random)
        //{
        //    // Get all users in the group
        //    var groupMembers = _context.GroupMemberships
        //        .Where(gm => gm.GroupId == queue.GroupId)
        //        .Select(gm => gm.User)
        //        .ToList();

        //    // Shuffle the members
        //    var random = new Random();
        //    var shuffledMembers = groupMembers.OrderBy(x => random.Next()).ToList();

        //    // Add the queue
        //    queue.Entries = shuffledMembers.Select(member => new QueueEntry
        //    {
        //        UserId = member.Id,
        //        JoinDateTime = DateTime.UtcNow
        //    }).ToList();

        //    _context.Queues.Add(queue);
        //    _context.SaveChanges();
        //}
        //else
        //{
        //    // Handle other queue types
        //    _context.Queues.Add(queue);
        //    _context.SaveChanges();
        //}

        //return RedirectToAction("Details", new { id = queue.Id });
        return View();
    }
    //[Authorize]
    public IActionResult Index()
    {
        var queues = _context.Queues
            .Include(q => q.Group) // Ensure Group is included
            .ToList();
        return View(queues);
    }
    //[HttpPost]
    //[Authorize]
    public IActionResult Join(int queueId)
    {
        var entry = new QueueEntry
        {
            QueueId = queueId,
            UserId = User.Identity.Name,
            JoinDateTime = DateTime.UtcNow
        };
        _context.QueueEntries.Add(entry);
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = queueId });
    }
    //[Authorize]
    public IActionResult ShuffleQueue(int queueId)
    {
        var queue = _context.Queues.Include(q => q.Entries).FirstOrDefault(q => q.Id == queueId);
        if (queue != null && queue.Type == QueueType.Random)
        {
            var entries = queue.Entries.ToList();
            var random = new Random();
            entries = entries.OrderBy(x => random.Next()).ToList();
            queue.Entries = entries;
            _context.SaveChanges();
        }
        return RedirectToAction("Details", new { id = queueId });
    }
}

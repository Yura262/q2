using System;
using System.Collections;
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
            .ThenInclude(e => e.User).Include(q=>q.Group)
            .FirstOrDefault(q => q.Id == id);
        return View(queue);
        //var queues = _context.Queues
        //    .Include(q => q.Group) // Ensure Group is included
        //    .ToList();
    }

    //[HttpPost]
    //[Authorize]

    public IActionResult Create()
    {
        //ViewBag.CreatorId = _context.Groups.Where(gm => gm.Id == queue.GroupId).Select(gm => gm.CreatorId);
        ViewBag.GroupId = _context.Groups.Select(x=>new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name.ToString()

        });

        //ViewBag.QueueType = E QueueType.Self.ToSelectList();
        return View();
    }
    [HttpPost]

    //public async Task<IActionResult> Create(string Name, DateTime EnabledDate, QueueType Type, int GroupId, Черга.Models.Queue queue)
    //{
    //    queue.Name = Name;
    //    queue.EnabledDate = EnabledDate;
    //    queue.Type = Type;
    //    queue.GroupId = GroupId;
    //    queue.Group =
    public async Task<IActionResult> Create([Bind("Name, EnabledDate, Type, GroupId")] Черга.Models.Queue queue)
    {
        //ViewBag.CreatorId = _context.Groups.Where(gm => gm.Id == queue.GroupId).Select(gm => gm.CreatorId);
        //// Check if the queue type is Random
        if (queue.Type == QueueType.Random)
        {
            // Get all users in the group
            var groupMembers = _context.GroupMemberships
                .Where(gm => gm.GroupId == queue.GroupId)
                .Select(gm => gm.User)
                .ToList();

            // Shuffle the members
            var random = new Random();
            var shuffledMembers = groupMembers.OrderBy(x => random.Next()).ToList();

            // Add the queue
            queue.Entries = shuffledMembers.Select(member => new QueueEntry
            {
                UserId = member.Id,
                JoinDateTime = DateTime.UtcNow
            }).ToList();

            _context.Queues.Add(queue);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        else
        {
            _context.Queues.Add(queue);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        ////queue.Group = await _context.Groups.()
        //////return RedirectToAction("Details", new { id = queue.Id });
        ////if (ModelState.IsValid)
        ////{
        ////    _context.Queues.Add(queue);
        ////    await _context.SaveChangesAsync();
        ////    return RedirectToAction("Index");
        ////}
        //return RedirectToAction("Index");
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
    public IActionResult Edit(int queueId,Черга.Models.Queue queue)
    {
        ViewBag.GroupId = _context.Groups.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name.ToString()

        });
        var ques = _context.Queues.FirstOrDefault(x => x == queue);
        return View(ques);
    }
    [HttpPost]
    public async Task<IActionResult> Edit([Bind("Id, Name, EnabledDate, Type, GroupId")] Черга.Models.Queue queue)
    {
        //if (ModelState.IsValid)
        //{
            _context.Queues.Update(queue);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        //}
        //var ques = await _context.Queues.FirstOrDefaultAsync(x => x == queue);
        //return View(ques);
    }
    public async Task<IActionResult> Delete(Черга.Models.Queue queue)
    {
        ViewBag.GroupId = _context.Groups.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name.ToString()

        });
        var ques = await _context.Queues.FirstOrDefaultAsync(x => x == queue);
        return View(ques);
    }
    [HttpPost,ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Черга.Models.Queue queue)
    {
        //var ques = await _context.Queues.FirstOrDefaultAsync(x => x == queue);
        if (queue != null)
        {
            _context.Queues.Remove(queue);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}

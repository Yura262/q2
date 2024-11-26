using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Черга.Data;
using Черга.Models;

public class GroupController : Controller
{
    private readonly ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public GroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;

    }
    //[Authorize]
    public IActionResult Index()
    {
        // Get the currently logged-in user's ID
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //if (userId == null)
        //{
        //    return Unauthorized();
        //}


        var groups = _context.Groups
            .Include(g => g.Members) // Include navigation property if needed
            //.Where(g => g.Members.Any(m => m.UserId == userId)) // Filter by user's membership
            .Include(cr=>cr.Creator)
            .ToList();

        return View(groups);
    }


    //[HttpPost]
    //[Authorize]
    public IActionResult Create()
    {
        //group.CreatorId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming username is the user ID
        //group.Name = 
        //_context.Groups.Add(group);
        //_context.SaveChanges();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name")] Group group)
    {
        //UserManager<ApplicationUser> _userManager;
        group.CreatorId = "c8571d80-46e9-427a-8956-deba10d68061";
        group.Creator = await _userManager.FindByIdAsync("c8571d80-46e9-427a-8956-deba10d68061");
        if (ModelState.IsValid)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(group);
    }
    public IActionResult Details(int id)
    {
        var group = _context.Groups
            .Include(q => q.Members)
            .ThenInclude(e => e.User).Include(cr=>cr.Creator)
            .FirstOrDefault(q => q.Id == id);
        return View(group);
    }

    //[HttpPost]
    //[Authorize]
    public IActionResult Join(int groupId)
    {
        var membership = new GroupMembership(User.Identity != null ? User.Identity.Name : "none", groupId)
        {
            UserId = User.Identity.Name,
            //User=new User(),
            //User = _context.Users.Where(u => u.Id == User.Identity.Name).DefaultIfEmpty(new Черга.Models.User()).First(),
            GroupId = groupId,
            Group = _context.Groups.Single()
        };
        _context.GroupMemberships.Add(membership);
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = groupId });
    }
}

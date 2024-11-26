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

public class GroupController : Controller
{
    private readonly ApplicationDbContext _context;

    public GroupController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var groups = _context.Groups.Include(g => g.Members).ToList();
        return View(groups);
    }

    //[HttpPost]
    //[Authorize]
    public IActionResult Create(Group group)
    {
        group.CreatorId = User.Identity.Name; // Assuming username is the user ID
        _context.Groups.Add(group);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    //[HttpPost]
    //[Authorize]
    public IActionResult Join(int groupId)
    {
        var membership = new GroupMembership(User.Identity != null ? User.Identity.Name : "none", groupId)
        {
            UserId = User.Identity.Name,
            //User=new User(),
            User = _context.Users.Where(u => u.Id == User.Identity.Name).DefaultIfEmpty(new Черга.Models.User()).First(),
            GroupId = groupId,
            Group = _context.Groups.Single()
        };
        _context.GroupMemberships.Add(membership);
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = groupId });
    }
}

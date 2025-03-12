using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListProj.Data;
using ToDoListProj.Models;
using Microsoft.EntityFrameworkCore;
using ToDoListProj.ViewModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Azure.Core;
using System.Net.Mail;
using System.Net;
using NuGet.Protocol.Plugins;
using System.Threading.Tasks;
using ToDoListProj.Areas.Identity.Pages.Account;



namespace ToDoListProj.Controllers
{
    [Authorize] 
    public class ToDoListController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        public ToDoListController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tasks = await _applicationDbContext.TaskItems
                .Where(t => t.UserId == user.Id && t.ProjectId == null && t.GroupTask == null)
                .ToListAsync();

            ViewBag.Tasks = tasks;
            ViewBag.ShowSidebar = true;

            return View();
        }


        public async Task<IActionResult> Today()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tasks = await _applicationDbContext.TaskItems
                .Where(t => t.UserId == user.Id && t.ProjectId == null && t.GroupTask == null && t.DueDate == DateTime.Now.Date)
                .ToListAsync();

            ViewBag.TodayTasks = tasks;
            ViewBag.ShowSidebar = true;

            return View();

        }
    
        public async Task<IActionResult> Projects()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var projects = await _applicationDbContext.Projects
                .Where(t => t.UserId == user.Id)
                .ToListAsync();
            var projectstask = await _applicationDbContext.TaskItems
                .Where(t => t.UserId == user.Id && t.ProjectId != null)
                .ToListAsync();

            var model = new ProjectsViewModel
            {
                Projects = projects,
                ProjectTasks = projectstask
            };
            ViewBag.ShowSidebar = true;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFriendship(int friendshipId)
        {
            var friendship = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f => f.Id == friendshipId);

            if (friendship == null)
            {
                return Json("Friendship not found.");
            }

            _applicationDbContext.Friendships.Remove(friendship);
            await _applicationDbContext.SaveChangesAsync();

            return Json("Friendship deleted successfully.");
        }

        public async Task<IActionResult> FriendsPage()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized("User not authenticated.");

           
                var friendships = _applicationDbContext.Friendships
                    .Where(f => (f.UserId == user.Id || f.FriendId == user.Id) && f.Status == "Accepted")
                    .ToList();

                var friends = new List<FriendsViewModel>();

                foreach (var friendship in friendships)
                {
                    var friendId = friendship.UserId == user.Id ? friendship.FriendId : friendship.UserId;
                    var friend = await _applicationDbContext.Users
                        .Where(u => u.Id == friendId)
                        .FirstOrDefaultAsync();

                    if (friend != null)
                    {
                    friends.Add(new FriendsViewModel
                    {
                        Id = friend.Id,
                        FullName = friend.FullName,
                        UserId = friend.Id,
                        FriendshipId = friendship.Id,
                        AvatarUrl = friend.AvatarUrl
                    });
                    }
                }

          
                var model = new FriendsPageViewModel
                {
                    Friends = friends
                };

           
                ViewBag.ShowSidebar = true;
                return View(model);
            }




        public async Task<IActionResult> UpcomingTasks(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Now.Date;

             var tasks = await _applicationDbContext.TaskItems
                .Where(t => t.UserId == user.Id &&
                            t.ProjectId == null &&
                            t.GroupTask == null &&
                            t.DueDate.Date >= today)
                .ToListAsync();

             if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                tasks = tasks.Where(t => t.DueDate.Date >= start).ToList();
                ViewData["StartDate"] = start.ToString("yyyy-MM-dd");
            }

             if (endDate.HasValue)
            {
                var end = endDate.Value.Date;
                tasks = tasks.Where(t => t.DueDate.Date <= end).ToList();
                ViewData["EndDate"] = end.ToString("yyyy-MM-dd");
            }

             tasks = tasks.OrderBy(t => t.DueDate).ToList();

            ViewBag.ShowSidebar = true;

            return View(tasks);
        }



        public async Task<IActionResult> GroupTasks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var groupmembers = await _applicationDbContext.GroupMembers
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            var groupIds = groupmembers.Select(gm => gm.GroupTaskId).Distinct().ToList();

            var groups = await _applicationDbContext.GroupTasks
                .Where(t => groupIds.Contains(t.Id))
                .ToListAsync();

            var groupMembers = await _applicationDbContext.GroupMembers
                .Where(gm => groupIds.Contains(gm.GroupTaskId))
                .Include(gm => gm.User)  
                .ToListAsync();

            var model = new GroupViewModel
            {
                Group = groups,  
                GroupMembers = groupMembers 
            };

            ViewBag.ShowSidebar = true;
            return View(model);
        }


        public IActionResult Notification()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Support()
        {
            ViewBag.ShowSidebar = false;
            return View();
        }

        [AllowAnonymous] 
        public IActionResult LandingPage()
        {
             ViewBag.ShowSidebar = false;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask(string title, string? description, DateTime dueDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            if(title == null)
            {
                return View();
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                IsCompleted = false,
                UserId = user.Id
            };

            _applicationDbContext.TaskItems.Add(task);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroupTask(string title, string? description, DateTime dueDate, int groupTaskId, int groupId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); 
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title cannot be empty.");
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                IsCompleted = false,
                GroupTaskId = groupTaskId
            };

            _applicationDbContext.TaskItems.Add(task);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("GroupDetails", "ToDoList", new { groupId = groupId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodayTask(string title, string? description, DateTime dueDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (title == null)
            {
                return View();
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                IsCompleted = false,
                UserId = user.Id
            };

            _applicationDbContext.TaskItems.Add(task);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Today");
        }
        [HttpPost]
        public async Task<IActionResult> CreateUpcomingTask(string title, string? description, DateTime dueDate, DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (title == null)
            {
                return View();
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                IsCompleted = false,
                UserId = user.Id
            };

            _applicationDbContext.TaskItems.Add(task);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("UpcomingTasks", new { startDate = startDate, endDate = endDate });
        }



        [HttpPost]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, bool isCompleted)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _applicationDbContext.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);

            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = isCompleted;
            await _applicationDbContext.SaveChangesAsync();

            return Ok(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _applicationDbContext.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);

            if (task == null)
            {
                return NotFound();
            }

            _applicationDbContext.TaskItems.Remove(task);
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true, taskId = taskId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGroupTask([FromBody] TaskCompletionDeleteModel request)
        {
               var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _applicationDbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == request.TaskId);
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            var groupMember = await _applicationDbContext.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == user.Id && gm.GroupTaskId == task.GroupTaskId);

            if (groupMember == null)
            {
                return Forbid(); 
            }

            _applicationDbContext.TaskItems.Remove(task);
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true, taskId = request.TaskId });
        }
        public class TaskCompletionDeleteModel
        {
            public int TaskId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var projecttasks = await _applicationDbContext.TaskItems
                .Where(t => t.ProjectId == projectId && t.UserId == user.Id)
                .ToListAsync();

            if (projecttasks.Any()) 
            {
                _applicationDbContext.TaskItems.RemoveRange(projecttasks);
            }

      
            var project = await _applicationDbContext.Projects
                .FirstOrDefaultAsync(t => t.Id == projectId && t.UserId == user.Id);

            if (project == null)
            {
                return NotFound();
            }

            _applicationDbContext.Projects.Remove(project);
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true, projectId = projectId });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateTaskTitle(int taskId, string newTitle)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _applicationDbContext.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);

            if (task == null)
            {
                return NotFound();
            }

            task.Title = newTitle;
            await _applicationDbContext.SaveChangesAsync();

            return Ok(new { success = true });
        }
     


        [HttpPost]
        public async Task<IActionResult> UpdateProjectTitle(string name, int projectId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Project name cannot be empty.");
            }

            var project = await _applicationDbContext.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            project.Name = name;
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true });
        }


        public class UpdateProjectModel
        {
            public int ProjectId { get; set; }
            public string NewTitle { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(string name, string? description, DateTime dueDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (name == null)
            {
                return View();
            }

            var project = new Project
            {
                Name = name,
                Description = description,
                DueDate = dueDate,
                UserId = user.Id
            };

            _applicationDbContext.Projects.Add(project);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Projects");
        }
        [HttpPost]
        public async Task<IActionResult> CreateProjectTask(ProjectTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid task data.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var task = new TaskItem
            {
                Title = model.Name,
                DueDate = model.DueDate,
                ProjectId = model.ProjectId,
                UserId = user.Id
            };

            _applicationDbContext.TaskItems.Add(task);
            await _applicationDbContext.SaveChangesAsync();

            return Json(new
            {
                id = task.Id,
                title = task.Title,
                dueDate = task.DueDate.ToString("dd.MM.yyyy"),
                projectId = task.ProjectId
            });
        }
   
        private List<string> sampleData = new List<string>
        {
            "Apple", "Banana", "Grapes", "Orange", "Peach", "Pineapple", "Strawberry", "Watermelon"
        };

        public async Task<JsonResult> GetSuggestions(string term)
        {
            var user = await _userManager.GetUserAsync(User); 

            if (user != null && term != null && term.Length >= 3)
            {
       
                var friends = await _applicationDbContext.Friendships
                    .Where(f => f.UserId == user.Id || f.FriendId == user.Id)
                    .Select(f => new
                    {
                        FullName = f.UserId == user.Id ? f.Friend.FullName : f.User.FullName,
                        FriendId = f.UserId == user.Id ? f.FriendId : f.UserId,
                        AvatarUrl = f.UserId == user.Id ? f.Friend.AvatarUrl : f.User.AvatarUrl 
                    })
                    .ToListAsync();

                var suggestions = friends
                    .Where(f => f.FullName.ToLower().Contains(term.ToLower()))
                    .Select(f => new
                    {
                        f.FullName,
                        f.FriendId,
                        AvatarUrl = string.IsNullOrEmpty(f.AvatarUrl) ? "/media/3d-illustration-white-man-with-glasses-bow-tie.jpg" : f.AvatarUrl 
                    })
                    .ToList();

                return Json(suggestions);
            }
            else
            {
                var suggestions = new List<object>(); 
                return Json(suggestions);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddUsersToGroup(string friendId, int groupId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var group = await _applicationDbContext.GroupTasks.FindAsync(groupId);

            if (group == null)
            {
                return NotFound("Group not found");
            }

            var friend = await _applicationDbContext.Users.FindAsync(friendId);

            if (friend == null)
            {
                return NotFound("User not found");
            }

             var existingMember = await _applicationDbContext.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupTaskId == groupId && gm.UserId == friendId);

            if (existingMember != null)
            {
                return BadRequest("User is already in the group");
            }

            var groupMember = new GroupMember
            {
                GroupTaskId = groupId,
                UserId = friendId
            };

            _applicationDbContext.GroupMembers.Add(groupMember);
            await _applicationDbContext.SaveChangesAsync();

            return Ok("User added to group");
        }

        public JsonResult GetTaskCounts(string userId)
        {

            var tasksCount = _applicationDbContext.TaskItems.Count(t => t.UserId == userId);

            var completedTasks = _applicationDbContext.TaskItems.Count(t => t.UserId == userId && t.IsCompleted);

            return Json(new { totalTasks = tasksCount, completedTasks = completedTasks });
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreateModel model)
        {
            if (model == null)
            {
                return BadRequest("Incorrect data!");
            }

            Console.WriteLine($"Group Name: {model.Name}");
            Console.WriteLine($"Group Description: {model.Description}");
            Console.WriteLine($"Tasks count: {model.GroupTasks?.Count}");

            var newGroup = new GroupTask
            {
                Name = model.Name,
                Description = model.Description,
                CreatedDate = DateTime.Now
            };

            _applicationDbContext.GroupTasks.Add(newGroup);
            await _applicationDbContext.SaveChangesAsync();

            var groupTasks = model.GroupTasks.Select(task => new TaskItem
            {
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate.HasValue ? task.DueDate.Value : DateTime.Now,
                IsCompleted = task.IsCompleted,
                UserId = task.UserId,
                ProjectId = task.ProjectId,
                GroupTaskId = newGroup.Id 
            }).ToList();

            _applicationDbContext.TaskItems.AddRange(groupTasks);
            await _applicationDbContext.SaveChangesAsync(); 

          
            var groupMembers = model.GroupMembers.Select(member => new GroupMember
            {
                UserId = member.UserId,
                GroupTaskId = newGroup.Id
            }).ToList();

            _applicationDbContext.GroupMembers.AddRange(groupMembers);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(new { message = "Group successfully created!", groupId = newGroup.Id });
        }


        [HttpPost]
        public ActionResult GetGroupDetails(int groupId)
        {
            var group = _applicationDbContext.GroupTasks
                .Include(g => g.Members)
                .Include(g => g.Tasks)
                .FirstOrDefault(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound();
            }

         
            return RedirectToAction("GroupDetails", new { groupId = groupId });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveUserFromGroup([FromBody] RemoveUserFromGroupRequest request)
        {
            if (request == null || request.UserId == null || request.GroupId == 0)
            {
                return Json(new { success = false, message = "Invalid request data." });
            }

            var groupMember = await _applicationDbContext.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == request.UserId && gm.GroupTaskId == request.GroupId);

            if (groupMember == null)
            {
                return Json(new { success = false, message = "User not found in the group." });
            }

            _applicationDbContext.GroupMembers.Remove(groupMember);
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> LeaveGroup([FromBody] LeaveGroupRequest request)
        {
            var group = await _applicationDbContext.GroupTasks
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId);

            if (group == null)
            {
                return Json(new { success = false, message = "Group not found." });
            }

            var member = group.Members.FirstOrDefault(m => m.UserId == request.UserId);
            if (member == null)
            {
                return Json(new { success = false, message = "User not in group." });
            }

            // Remove all tasks related to the group
         

            // Remove the member from the group
            _applicationDbContext.GroupMembers.Remove(member);
            await _applicationDbContext.SaveChangesAsync();

            // Check if the group is empty after removing the member
            bool isGroupEmpty = !group.Members.Any();
            if (isGroupEmpty)
            {
                var tasksToRemove = await _applicationDbContext.TaskItems
                    .Where(t => t.GroupTaskId == request.GroupId)
                    .ToListAsync();

                if (tasksToRemove.Any())
                {
                    _applicationDbContext.TaskItems.RemoveRange(tasksToRemove);
                }
                _applicationDbContext.GroupTasks.Remove(group);
                await _applicationDbContext.SaveChangesAsync();
            }

            return Json(new { success = true, isGroupEmpty });
        }



        public class LeaveGroupRequest
        {
            public string UserId { get; set; }
            public int GroupId { get; set; }
        }

        public class RemoveUserFromGroupRequest
        {
            public string UserId { get; set; }
            public int GroupId { get; set; }
        }

        [Authorize]
        public async Task<IActionResult> GroupDetails(int groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var group = await _applicationDbContext.GroupTasks
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Include(g => g.Tasks)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound();
            }

         
            bool isMember = group.Members.Any(m => m.User.Id == userId);

            if (!isMember)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            var viewModel = new GroupDetailsViewModel
            {
                Group = group,
                Members = group.Members.Select(m => m.User).ToList(),
                Tasks = group.Tasks.ToList()
            };
            ViewBag.ShowSidebar = true;

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGroupTaskTitle([FromBody] UpdateTaskGroupRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _applicationDbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == request.TaskId);
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            var groupMember = await _applicationDbContext.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == user.Id && gm.GroupTaskId == task.GroupTaskId);

            if (groupMember == null)
            {
                return Forbid();
            }

            task.Title = request.NewTitle;
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true });
        }
        public async Task<IActionResult> LogoutAndRedirectToForgotPassword()
        {
           

            return Json(new { redirectUrl = "/Identity/Account/ForgotPassword" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "File not selected" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { success = false, message = "Invalid file type. Only image files are allowed." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder); 

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

     
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

        
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { success = false, message = "User not authenticated" });

       
            user.AvatarUrl = $"/uploads/{fileName}";
            await _userManager.UpdateAsync(user);

      
            return Json(new { success = true, avatarUrl = user.AvatarUrl });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var group = await _applicationDbContext.GroupTasks
               .Include(g => g.Members)
               .FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return Json(new { success = false, message = "Group not found." });
            }
            var tasksToRemove = await _applicationDbContext.TaskItems
                   .Where(t => t.GroupTaskId == groupId)
                   .ToListAsync();

            if (tasksToRemove.Any())
            {
                _applicationDbContext.TaskItems.RemoveRange(tasksToRemove);
            }
            _applicationDbContext.GroupTasks.Remove(group);
            await _applicationDbContext.SaveChangesAsync();
            return Json(new { success = true});
        }
       

        [HttpPost]
        public async Task<IActionResult> UploadGroupFile(IFormFile file, int groupId)
        {
            var group = await _applicationDbContext.GroupTasks.FindAsync(groupId);
            if (group == null)
                return NotFound(new { success = false, message = "Group not found" });

           
            if (file == null || file.Length == 0)
            {
                 
                group.GroupImageUrl = "/media/joanna-kosinska-spAkZnUleVw-unsplash.jpg";
                _applicationDbContext.GroupTasks.Update(group);
                await _applicationDbContext.SaveChangesAsync();
                return Json(new { success = true, imageUrl = group.GroupImageUrl });
            }
             
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { success = false, message = "Invalid file format. Only images are allowed." });

         
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

 
            group.GroupImageUrl = $"/uploads/{fileName}";
            _applicationDbContext.GroupTasks.Update(group);
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { success = true, imageUrl = group.GroupImageUrl });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAvatar()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized(new { success = false, message = "User not authenticated" });

            return Json(new { avatarUrl = user.AvatarUrl });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateFullName(string fullName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return Json(new { success = false, message = "Full name cannot be empty" });
            }
            var userfullname = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            userfullname.FullName = fullName;
            await _applicationDbContext.SaveChangesAsync();
          

            return Json(new { success = true, message = "Full name updated successfully" });
        }

        public class UpdateTaskGroupRequest
        {
            public int TaskId { get; set; }
            public string NewTitle { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGroupTaskCompletion([FromBody] TaskCompletionUpdateModel model)
        {
            if (model == null || model.TaskId <= 0)
            {
                return Json(new { result = "error", message = "Invalid task ID" });
            }

            var task = await _applicationDbContext.TaskItems.FindAsync(model.TaskId);
            if (task == null)
            {
                return Json(new { result = "error", message = "Task not found" });
            }

            task.IsCompleted = model.IsCompleted;

        
            await _applicationDbContext.SaveChangesAsync();

            return Json(new { result = "success", taskId = model.TaskId });
        }
        public class TaskCompletionUpdateModel
        {
            public int TaskId { get; set; }
            public bool IsCompleted { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> SendSupportMessage(string name, string email, string message)
        {
            var fromMail = "todolisttodolist43@gmail.com";
            var fromPassword = "xmeo nmwe kxae pawr";
            var toMail = "todolisttodolist43@gmail.com";

            try
            {
                var fromMailAddress = new MailAddress(fromMail);
                var toMailAddress = new MailAddress(toMail);

                var mailMessage = new MailMessage
                {
                    From = fromMailAddress,
                    Subject = "ToDoList Support",
                    IsBodyHtml = true,
                    Body = $"<html><body>" +
                           $"<h2>New Support Request</h2>" +
                           $"<p><strong>Name:</strong> {name}</p>" +
                           $"<p><strong>Email:</strong> {email}</p>" +
                           $"<p><strong>Message:</strong></p>" +
                           $"<p>{message}</p>" +
                           $"</body></html>"
                };

                mailMessage.To.Add(toMailAddress);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;

                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine("✅ Message sent successfully!");

                    return Json(new { success = true, message = "Message sent successfully!" });
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"❌ SMTP Error: {smtpEx.Message}");
                return Json(new { success = false, error = smtpEx.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ General Error: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddFriend(string userId, string friendId)
        {
            if (userId == friendId)
            {
             
                return BadRequest("You cannot add yourself as a friend.");
            }

      
            var existingFriendship = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));

            if (existingFriendship != null)
            {
              
                return BadRequest("This is already your friend.");
            }

            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

       
            _applicationDbContext.Friendships.Add(friendship);
            await _applicationDbContext.SaveChangesAsync();

            return Ok("Friend added successfully.");
        }
        [HttpGet]
        public IActionResult CheckUser(string friendId)
        {

            var user = _applicationDbContext.Users.FirstOrDefault(u => u.Id == friendId);
            
            return Json(new { exists = user != null });
        }
        [HttpPost]
        public async Task<IActionResult> SendRequest(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Id == friendId)
                return BadRequest("Invalid request");

            var existingRequest = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.FriendId == friendId);

            if (existingRequest != null)
                return BadRequest("Request already sent");

            var friendship = new Friendship
            {
                UserId = user.Id,
                FriendId = friendId,
                Status = "Pending"
            };

            _applicationDbContext.Friendships.Add(friendship);
            await _applicationDbContext.SaveChangesAsync();

     
            var notification = new Notification
            {
                UserId = friendId,
                Message = $"{user.UserName} sent you a friend request.",
                Date = DateTime.UtcNow,
                SenderId = user.Id, 
                IsRead = false
            };

            _applicationDbContext.Notifications.Add(notification);
            await _applicationDbContext.SaveChangesAsync();

            return Ok("Request sent successfully");
        }


        [HttpPost]
        public async Task<IActionResult> AcceptRequest(string senderId, int notId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

             var friendship = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f => f.UserId == senderId && f.FriendId == user.Id);

            if (friendship == null)
                return BadRequest("Friend request does not exist.");

             var notificationsToRemove = await _applicationDbContext.Notifications
                .Where(n => (n.UserId == user.Id && n.SenderId == senderId)
                         || (n.UserId == senderId && n.SenderId == user.Id))
                .ToListAsync();

            if (!notificationsToRemove.Any())
                return NotFound("Notification not found");

             var reverseFriendship = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.FriendId == senderId);

             if (reverseFriendship != null)
            {
                _applicationDbContext.Friendships.Remove(reverseFriendship);
            }

             friendship.Status = "Accepted";

             _applicationDbContext.Notifications.RemoveRange(notificationsToRemove);

            await _applicationDbContext.SaveChangesAsync();

            return Ok("Friend request accepted.");
        }



        [HttpPost]
        public async Task<IActionResult> DeclineRequest(string senderId, int notId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not authenticated.");

             var friendship = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f => (f.UserId == senderId && f.FriendId == user.Id) ||
                                          (f.UserId == user.Id && f.FriendId == senderId));

            if (friendship == null)
                return NotFound("Friend request not found.");

             if (friendship.Status == "Accepted")
                return BadRequest("You are already friends.");

             _applicationDbContext.Friendships.Remove(friendship);

             var notificationsToRemove = await _applicationDbContext.Notifications
                .Where(n => (n.UserId == user.Id && n.SenderId == senderId)
                         || (n.UserId == senderId && n.SenderId == user.Id))
                .ToListAsync();

            if (notificationsToRemove.Any())
            {
                _applicationDbContext.Notifications.RemoveRange(notificationsToRemove);
            }

            await _applicationDbContext.SaveChangesAsync();

            return Ok("Friend request declined.");
        }


        [HttpGet]
        public IActionResult GetUnreadNotifications()
            {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

             var notifications = _applicationDbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.Date)
            .Select(n => new
            {
                n.Id,
                n.Message,
                n.Date,
                n.SenderId 
            }).ToList();


            return Ok(notifications);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(int id)
        {
             var notification = await _applicationDbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

       
            _applicationDbContext.Notifications.Remove(notification);

             var friendship = await _applicationDbContext.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.UserId == notification.UserId && f.FriendId == notification.SenderId ||
                     f.UserId == notification.SenderId && f.FriendId == notification.UserId) &&
                    f.Status == "Pending");

            if (friendship != null)
            {
                _applicationDbContext.Friendships.Remove(friendship);
            }

             await _applicationDbContext.SaveChangesAsync();

            return Ok("Notification deleted if applicable.");
        }




    }


}


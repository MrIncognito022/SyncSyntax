using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SyncSyntax.Data;
using SyncSyntax.Models.ViewModels;
using System.Net;

namespace SyncSyntax.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", ".png" };
        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Posts/Create
        [HttpGet]
        public IActionResult Create()
        {
            //ViewData["Category"] = new SelectList(_context.Categories, "Id", "Name");
            var postViewModel = new PostViewModel
            {
                Categories = new SelectList(_context.Categories, "Id", "Name"),
            };
            return View(postViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostViewModel postViewModel)
        {
            if(ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed =  _allowedExtension.Contains(inputFileExtension);
                if (!isAllowed) {
                    ModelState.AddModelError("Image", "Invalid image format. Allowed formats are .jpg, .jpeg, .png");
                    return View(postViewModel);
                }

                postViewModel.Post.FeatureImagePath =  UploadFileToFolder(postViewModel.FeatureImage);
                postViewModel.Post.GenerateSlug();
                _context.Posts.Add(postViewModel.Post);
                _context.SaveChanges();
            }


            return View(postViewModel);
        }

        private string UploadFileToFolder(IFormFile file)
        {
           var inputFileExtension = Path.GetExtension(file.FileName);
           var fileName = Guid.NewGuid().ToString() + inputFileExtension;
           var wwwRootPath = _webHostEnvironment.WebRootPath;
           var imagesFolderPath = Path.Combine(wwwRootPath,"images");
           var filePath = Path.Combine(imagesFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }

    }
}

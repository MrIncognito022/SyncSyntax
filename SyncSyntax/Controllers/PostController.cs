﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncSyntax.Data;
using SyncSyntax.Models.ViewModels;
using System.Net;
using System.Text.RegularExpressions;

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
            //ViewData["Category"] = new SelectList(_context.Categori
            //es, "Id", "Name");
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

        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            var postQuery = _context.Posts.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue) 
            {
                postQuery =  postQuery.Where(p=>p.CategoryId == categoryId);
            }
            var posts = postQuery.ToList();

            ViewData["Categories"] = _context.Categories.ToList();

            return View(posts);
        }

        public IActionResult Detail(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _context.Posts.Include(p=>p.Category).Include(p=>p.Comments)
                .FirstOrDefault(p=>p.Id == id);

            if (post == null) 
            {
                return NotFound();
            }
            return View(post);

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

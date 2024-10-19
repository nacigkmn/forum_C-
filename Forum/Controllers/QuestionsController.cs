using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Forum.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(ApplicationDbContext context, ILogger<QuestionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            try
            {
                var questions = await _context.Questions
                    .Include(q => q.User)
                    .Include(q => q.Answers)
                    .ToListAsync();
                return View(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index method");
                return View("Error");
            }
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var question = await _context.Questions
                    .Include(q => q.User)
                    .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (question == null)
                {
                    return NotFound();
                }

                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Details method");
                return View("Error");
            }
        }

        // GET: Questions/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IdentityUserId")] Question question)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(question);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                _logger.LogWarning("[QuestionsController] Question creation failed {@question}", question);
                return View("Error");
            }

            return View(question);
        }

        // GET: Questions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var question = await _context.Questions.FindAsync(id);

                if (question == null)
                {
                    return NotFound();
                }

                if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }

                ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", question.IdentityUserId);
                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Edit method");
                return View("Error");
            }
        }

        // POST: Questions/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IdentityUserId")] Question question)
        {
            try
            {
                if (id != question.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(question);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!QuestionExists(question.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Edit method");
                return View("Error");
            }

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", question.IdentityUserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var question = await _context.Questions
                    .Include(q => q.User)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (question == null)
                {
                    return NotFound();
                }

                if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }

                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete method");
                return View("Error");
            }
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var question = await _context.Questions.FindAsync(id);

                if (question == null)
                {
                    return NotFound();
                }

                if (question.IdentityUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Unauthorized();
                }

                var answersToDelete = await _context.Answers.Where(a => a.QuestionId == question.Id).ToListAsync();
                _context.Answers.RemoveRange(answersToDelete);

                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteConfirmed method");
                return View("Error");
            }
        }

        // GET: Questions/AddAnswer/5
        [Authorize]
        public async Task<IActionResult> AddAnswer(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var question = await _context.Questions
                    .Include(q => q.User)
                    .Include(a => a.Answers)
                    .ThenInclude(q => q.User)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                {
                    return NotFound();
                }

                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAnswer method");
                return View("Error");
            }
        }

        // POST: Questions/AddAnswer
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswer([Bind("Id,Content,QuestionId,IdentityUserId")] Answer answer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(answer);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", new { id = answer.QuestionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAnswer POST method");
                return View("Error");
            }
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }

    }

}
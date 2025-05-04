using Microsoft.AspNetCore.Mvc;
using Supabase;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoController : Controller
    {
        private readonly Supabase.Client _supabase;

        public ToDoController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IActionResult> Index(string? category, string? priority, string? sortBy)
        {
            var query = _supabase.From<Todo>();

            if (!string.IsNullOrWhiteSpace(category))
                query.Filter("category", Postgrest.Constants.Operator.Equals, category);

            if (!string.IsNullOrWhiteSpace(priority))
            {
                int priorityValue = priority.ToLower() switch
                {
                    "low" => 1,
                    "medium" => 2,
                    "high" => 3,
                    _ => 0
                };

                if (priorityValue > 0)
                    query.Filter("priority", Postgrest.Constants.Operator.Equals, priorityValue);
            }


            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("category", StringComparison.OrdinalIgnoreCase))
                    query.Order("category", Postgrest.Constants.Ordering.Ascending);
                else if (sortBy.Equals("priority", StringComparison.OrdinalIgnoreCase))
                    query.Order("priority", Postgrest.Constants.Ordering.Ascending);
            }

            var result = await query.Get();
            var todos = result.Models;

            return View(todos);
        }


        // CREATE ToDo HELL YEAH?
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Todo todo)
        {
            if (ModelState.IsValid)
            {
                await _supabase.From<Todo>().Insert(todo);
                return RedirectToAction("Index");
            }

            return View(todo);
        }
        // CREATE END

        // EDIT ToDo HELL YEAH?
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _supabase
                .From<Todo>()
                .Where(x => x.Id == id)
                .Get();

            var todo = response.Models.FirstOrDefault();
            if (todo == null)
                return NotFound();

            return View(todo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Todo updatedTodo)
        {
            if (!ModelState.IsValid)
                return View(updatedTodo);

            updatedTodo.Id = id;
            var response = await _supabase.From<Todo>().Update(updatedTodo);

            return RedirectToAction(nameof(Index));
        }
        // EDIT END

        // DELETE ToDo HELL YEAH?
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _supabase
                .From<Todo>()
                .Where(x => x.Id == id)
                .Get();

            var todo = response.Models.FirstOrDefault();
            if (todo == null)
                return NotFound();

            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _supabase
                .From<Todo>()
                .Where(x => x.Id == id)
                .Get();

            var todo = response.Models.FirstOrDefault();
            if (todo == null)
                return NotFound();

            await _supabase.From<Todo>().Delete(todo);

            return RedirectToAction(nameof(Index));
        }
        // DELETE END
    }
}

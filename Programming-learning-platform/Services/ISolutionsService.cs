using lab2.Models;
using lab2.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace lab2.Services
{
    public interface ISolutionsService
    {
        JsonResult GetAllSolutions(int? task, int? user);
        Task PostmoderateSolution(PostmoderationDTO model, int id);
        bool IsSolutionExist(int? id);
    }
    public class SolutionsService : ISolutionsService
    {
        private readonly DatabaseContext _context;

        public SolutionsService(DatabaseContext context)
        {
            _context = context;
        }

        public JsonResult GetAllSolutions(int? task, int? user)
        {
            Solution[]? solutions = null;
            if (task != null && user != null)
            {
                solutions = _context.Solutions.Where(x => x.taskId == task && x.authorId == user).ToArray();
            }
            else if (task == null && user != null)
            {
                solutions = _context.Solutions.Where(x => x.authorId == user).ToArray();
            }
            else if (task != null && user == null)
            {
                solutions = _context.Solutions.Where(x => x.taskId == task).ToArray();
            }
            else
            {
                solutions = _context.Solutions.ToArray();
            }

            if (solutions == null)
            {
                return null;
            }

            return new JsonResult(solutions);
        }

        public async Task PostmoderateSolution(PostmoderationDTO model, int id)
        {
            var solution = _context.Solutions.FirstOrDefault(x => x.id == id);
            solution.verdict = model.verdict;
            _context.Solutions.Update(solution);
            await _context.SaveChangesAsync();
            
        }

        public bool IsSolutionExist(int? id)
        {
            if (id == null)
            {
                return false;
            }

            var solution = _context.Solutions.FirstOrDefault(x => x.id == id);

            if (solution == null)
            {
                return false;
            }

            return true;
        }
    }
}

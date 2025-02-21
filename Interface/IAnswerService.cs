using FormsWebApplication.Models;

namespace FormsWebApplication.Interface
{
    public interface IAnswerService
    {
        Task<List<Answer>> GetUserAnswersAsync(string userId);
        Task<List<Answer>> GetAnswerByTemplateIdAsync(int templateId, string userId, bool isAdmin);
        Task<Answer?> GetTemplateAnswersAsync(int answerId, string userId, bool isAdmin);
        Task<bool> DeleteAnswerAsync(int id, string userId, bool isAdmin);
        Task<Dictionary<string, List<int>>> GetCustomIntAnswersAsync(int templateId, string userId, bool isAdmin);
        double CalculateAverage(IEnumerable<int> numbers);
    }

}

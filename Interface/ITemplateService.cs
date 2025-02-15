using FormsWebApplication.Models;

namespace FormsWebApplication.Interface
{
    public interface ITemplateService
    {
        Task<List<Template>> GetUserTemplatesAsync(string userId);
        Task<Template?> GetTemplateByIdAsync(int templateId);
        Task<int> CreateTemplateAsync(Template template, string userId);
        Task<List<Template>> GetLatestTemplatesAsync(int skip, int take);

        Task<bool> SubmitResponseAsync(Answer answer, string userId);
        //Task<Template?> GetTemplateForResponseAsync(int templateId);
    }

}

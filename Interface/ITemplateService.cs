using FormsWebApplication.Models;
using FormsWebApplication.Models.FormsWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormsWebApplication.Interface
{
    public interface ITemplateService
    {
        Task<List<Template>> GetUserTemplatesAsync(string userId);
        Task<Template?> GetTemplateByIdAsync(int templateId, string userId, bool isAdmin);
        Task<int> CreateTemplateAsync(Template template, string userId);
        Task<List<Template>> GetLatestTemplatesAsync(int skip, int take);
        Task<bool> SubmitResponseAsync(Answer answer, string userId);
        Task<bool> LikeTemplateAsync(int templateId, string userId);
        Task<int> GetLikeCountAsync(int templateId);
        Task<bool> AddCommentAsync(int templateId, string userId, string content);
        Task<List<Comment>> GetCommentsAsync(int templateId);
        Task<Template?> GetTemplateForEditAsync(int templateId);
        Task<bool> UpdateTemplateAsync(int templateId, Template updatedTemplate);
        Task<bool> DeleteTemplateAsync(int templateId);
        int GetAnswerCount(int templateId);
        void CallReIndex();
        Task<List<string>> SearchTagsAsync(string query);
        Task SetTagsForTemplateAsync(Template template, List<string>? tagNames);

        Task<List<Template>> GetTemplatesByTagAsync(string tagName, int skip, int take);
        Task<List<Tag>> GetPopularTagsAsync(int count=20);
    }
}

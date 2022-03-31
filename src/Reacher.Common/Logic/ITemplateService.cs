namespace Reacher.Common.Logic;

public interface ITemplateService
{
    Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model) where T : class;
}

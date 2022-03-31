using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Reacher.App.Services;
public class TemplateService : ITemplateService
{
    private IRazorViewEngine _razorViewEngine;
    private IServiceProvider _serviceProvider;
    private ITempDataProvider _tempDataProvider;

    public TemplateService(
        IRazorViewEngine engine,
        IServiceProvider serviceProvider,
        ITempDataProvider tempDataProvider)
    {
        this._razorViewEngine = engine;
        this._serviceProvider = serviceProvider;
        this._tempDataProvider = tempDataProvider;
    }

    public async Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model) where T : class
    {
        var httpContext = new DefaultHttpContext() { RequestServices = _serviceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using (StringWriter sw = new StringWriter())
        {
            var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                return string.Empty;
            }

            var metadataProvider = new EmptyModelMetadataProvider();
            var msDictionary = new ModelStateDictionary();
            var viewDataDictionary = new ViewDataDictionary(metadataProvider, msDictionary);

            viewDataDictionary.Model = model;

            var tempDictionary = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDataDictionary,
                tempDictionary,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}

using RazorEngine;
using RazorEngine.Templating;
using Reacher.Data.Enums;

namespace Reacher.App.Services
{
    public class EmailContentProvider : IEmailContentProvider
    {
        public EmailContentProvider()
        {
            var assembly = this.GetType().Assembly;
            var resourceName = "Reacher.App.Templates.Main.cshtml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string template = reader.ReadToEnd();
                Engine.Razor.Compile(template, "main", typeof(RazorReacherEmail));
            }
        }

        public string RunTemplate(ReacherEmail email)
        {
            var razorEmail = new RazorReacherEmail(email);
            return Engine.Razor.Run("main", typeof(RazorReacherEmail), razorEmail);
        }
    }
}

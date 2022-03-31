using Reacher.Common.Models;

namespace Reacher.Common.Logic;
public interface IEmailContentProvider
{
    string RunTemplate(ReacherEmail email);
}


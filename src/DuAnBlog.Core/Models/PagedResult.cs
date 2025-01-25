namespace DuAnBlog.Core.Models;
public class PagedResult<T> : PagedResultBase where T : class
{
    public PagedResult()
    {
        Results = new List<T>();
    }
    public List<T> Results { get; set; }

}
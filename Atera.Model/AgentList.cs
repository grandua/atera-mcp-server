namespace Atera.Model;

public class AgentList
{
    public List<Agent> Items { get; set; }
    public int Page { get; set; }
    public int ItemsInPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
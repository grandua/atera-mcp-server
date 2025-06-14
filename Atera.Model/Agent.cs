namespace Atera.Model;

public class Agent
{
    public int AgentID { get; set; }
    public int CustomerID { get; set; }
    public string CustomerName { get; set; }
    public string MachineName { get; set; }
    public string DeviceType { get; set; }
    public string DomainName { get; set; }
    public bool Online { get; set; }
    public DateTime? LastRebootTime { get; set; }
    public string LastLoginUser { get; set; }
    public DateTime LastSeenTime { get; set; }
    public string OSType { get; set; }
    public string OSVersion { get; set; }
}

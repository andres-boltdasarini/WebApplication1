// Services/IAgentService.cs
using BookingAgentApp.Models;

namespace BookingAgentApp.Services
{
    public interface IAgentService
    {
        List<BookingAgent> GetAllAgents();
        BookingAgent? GetAgentById(int id);
        bool BookAgent(int agentId, string userName);
        bool ReleaseAgent(int agentId);
    }

    // Services/AgentService.cs
    public class AgentService : IAgentService
    {
        private static List<BookingAgent> _agents = new()
        {
            new BookingAgent { Id = 1, Name = "Агент 1", Status = "Свободен" },
            new BookingAgent { Id = 2, Name = "Агент 2", Status = "Свободен" },
            new BookingAgent { Id = 3, Name = "Агент 3", Status = "Занят", BookedBy = "Иван Иванов", BookingTime = DateTime.Now.AddHours(-2) },
            new BookingAgent { Id = 4, Name = "Агент 4", Status = "Свободен" },
            new BookingAgent { Id = 5, Name = "Агент 5", Status = "Занят", BookedBy = "Петр Петров", BookingTime = DateTime.Now.AddHours(-1) }
        };

        public List<BookingAgent> GetAllAgents()
        {
            return _agents;
        }

        public BookingAgent? GetAgentById(int id)
        {
            return _agents.FirstOrDefault(a => a.Id == id);
        }

        public bool BookAgent(int agentId, string userName)
        {
            var agent = GetAgentById(agentId);
            if (agent != null && agent.Status == "Свободен")
            {
                agent.Status = "Занят";
                agent.BookedBy = userName;
                agent.BookingTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool ReleaseAgent(int agentId)
        {
            var agent = GetAgentById(agentId);
            if (agent != null && agent.Status == "Занят")
            {
                agent.Status = "Свободен";
                agent.BookedBy = null;
                agent.BookingTime = null;
                return true;
            }
            return false;
        }
    }
}
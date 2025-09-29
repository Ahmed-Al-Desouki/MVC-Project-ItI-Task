using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models
{
    public class Department
    {

        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string ManagerName { get; set; }
    }
}

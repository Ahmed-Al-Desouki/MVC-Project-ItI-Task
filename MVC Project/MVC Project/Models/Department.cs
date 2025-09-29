using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Project.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        [ForeignKey("Manager")]
        public int ManagerId { get; set; }
        public Instractor Manager { get; set; } = default!;
    }
}

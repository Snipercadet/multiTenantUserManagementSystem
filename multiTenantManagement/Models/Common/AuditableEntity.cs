namespace multiTenantManagement.Models.Common
{
    public class AuditableEntity : IAuditableEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get ; set ; }
       
        
    }


    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime LastModifiedAt { get; set; }
        string ModifiedBy { get; set; }
      
    }
}

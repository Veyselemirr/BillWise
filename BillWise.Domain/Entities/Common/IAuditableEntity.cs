namespace BillWise.Domain.Entities.Common
{
    //Bu class kim oluşturdu kim değiştirdi bilgileri için kalıtımv verebilir
    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }
       string? UpdatedBy { get; set; }
    }
}

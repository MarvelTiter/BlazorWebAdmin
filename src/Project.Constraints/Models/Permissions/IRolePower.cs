namespace Project.Constraints.Models.Permissions
{
    public interface IRolePower
    {
        string RoleId { get; set; }
        string PowerId { get; set; }
    }
    [LightTable(Name = "ROLE_POWER")]
    public class RolePower: IRolePower
    {
        [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
        [LightColumn(Name = "POWER_ID", PrimaryKey = true)]
        public string PowerId { get; set; }
    }
}

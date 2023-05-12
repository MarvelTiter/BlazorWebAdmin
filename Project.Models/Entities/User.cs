using MDbEntity.Attributes;
using Project.Common.Attributes;
using Project.Models.Permissions;

namespace Project.Models.Entities
{
	[Table(Name = "USER")]
	public class User : IUser
	{
		[ColumnDefinition]
		[Column(Name = "USER_ID")]
		public string UserId { get; set; }
		[ColumnDefinition]
		[Column(Name = "USER_NAME")]
		public string UserName { get; set; }
		[ColumnDefinition]
		[Column(Name = "PASSWORD")]
		public string Password { get; set; }
		[ColumnDefinition]
		[Column(Name = "AGE")]
		public int? Age { get; set; }
		[ColumnDefinition]
		[Column(Name = "SIGN")]
		public string Sign { get; set; }
		[ColumnDefinition]
		[Column(Name = "LAST_LOGIN")]
		public DateTime? LastLogin { get; set; }
	}
}

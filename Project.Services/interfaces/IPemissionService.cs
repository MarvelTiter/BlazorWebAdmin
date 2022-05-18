using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services.interfaces
{
    public interface IPermissionService
	{
		Task<IQueryCollectionResult<Power>> GetPowerListAsync(GeneralReq<Power> req);
		Task<IQueryCollectionResult<Power>> GetPowerListAsync();
		Task<IQueryCollectionResult<Role>> GetRoleListAsync(GeneralReq<Role> req);
		Task<IQueryCollectionResult<Role>> GetRoleListAsync();
		Task<IQueryCollectionResult<Power>> GetPowerListByUserIdAsync(string usrId);
		Task<IQueryCollectionResult<Power>> GetPowerListByRoleIdAsync(string roleId);
		Task<IQueryCollectionResult<Role>> GetUserRolesAsync(string usrId);
		Task<bool> SaveUserRole(string usrId, params string[] roles);
		Task<bool> SaveRolePower(string roleId, params string[] powers);
		Task<int> UpdatePowerAsync(Power power);
		Task<Power> InsertPowerAsync(Power power);
		Task<int> UpdateRoleAsync(Role role);
		Task<Role> InsertRoleAsync(Role role);
	}
}

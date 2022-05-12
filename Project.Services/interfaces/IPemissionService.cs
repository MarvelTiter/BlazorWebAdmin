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
    public interface IPemissionService
	{
		Task<QueryResult<PagingResult<Power>>> GetPowerListAsync(GeneralReq<Power> req);
		Task<IEnumerable<Power>> GetPowerListAsync();
		Task<QueryResult<PagingResult<Role>>> GetRoleListAsync(GeneralReq<Role> req);
		Task<IEnumerable<Role>> GetRoleListAsync();
		Task<QueryResult<IEnumerable<Power>>> GetPowerListByUserIdAsync(string usrId);
		Task<QueryResult<IEnumerable<Power>>> GetPowerListByRoleIdAsync(string roleId);
		Task<bool> SaveUserRole(string usrId, params string[] roles);
		Task<bool> SaveRolePower(string roleId, params string[] powers);
		Task<int> UpdatePower(Power power);
	}
}

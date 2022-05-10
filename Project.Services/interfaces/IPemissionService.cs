using Project.Models;
using Project.Models.Entities;
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
	}
}

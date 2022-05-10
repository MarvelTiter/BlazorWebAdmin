using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using Project.Repositories.interfaces;
using Project.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
	public class PemissionService : IPemissionService
	{
		private readonly IRepository repository;

		public PemissionService(IRepository repository)
		{
			this.repository = repository;
		}
		public async Task<QueryResult<PagingResult<Power>>> GetPowerListAsync(GeneralReq<Power> req)
		{
			var count = await repository.Table<Power>().GetCountAsync(req.Expression);
			var list = await repository.Table<Power>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
			return QueryResult<Power>.PagingResult(list, count);
		}
	}
}

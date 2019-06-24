using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
	public class CityInfoRepository : ICityInfoRepository
	{
		private CityInfoContext _context;

		public CityInfoRepository(CityInfoContext context)
		{
			_context = context;
		}
		public IEnumerable<City> GetCities()
		{
			return _context.Cities.OrderBy(c => c.Name).ToList();
		}

		public City GetCity(int cityId, bool includePointsOfInterest)
		{
			if (includePointsOfInterest)
			{
				return _context.Cities.Include(c => c.PointsOfInterest)
					.Where(c => c.Id == cityId).FirstOrDefault();
			}

			return _context.Cities.Where(c => c.Id == cityId).FirstOrDefault();
		}		

		public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterstId)
		{
			return _context.PointsOfInterest
				.Where(p => p.CityId == cityId && p.Id == pointOfInterstId).FirstOrDefault();
		}
		public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
		{
			return _context.PointsOfInterest
				.Where(p => p.CityId == cityId).ToList();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace FilterAndRank
{
    public static class Ranking
    {        
        public static IList<RankedResult> FilterByCountryWithRank(
            IList<Person> people,
            IList<CountryRanking> rankingData,
            IList<string> countryFilter,
            int minRank, int maxRank,
            int maxCount)
        {
            // TODO: write your solution here.  Do not change the method signature in any way, or validation of your solution will fail.
            var peopleQuery = people
                .Join(rankingData, p => p.Id, r => r.PersonId, (p, r) => new {p, r})
                .AsQueryable();
            
            if (countryFilter.Any())
            {
                peopleQuery = peopleQuery.Where(p => countryFilter.Select(c => c.ToLower()).Contains(p.r.Country.ToLower()));
            }

            peopleQuery = peopleQuery.Where(p => p.r.Rank >= minRank && p.r.Rank <= maxRank);

            if (peopleQuery.GroupBy(p => p.r.Rank).Any(g => g.Count() > 1) == false)
            {
                peopleQuery = peopleQuery.Take(maxCount);
            }

            return peopleQuery
                .OrderBy(p => p.r.Rank)
                .ThenBy(p => countryFilter.Select(c => c.ToLower()).ToList().IndexOf(p.r.Country.ToLower()))
                .ThenBy(p => p.p.Name.ToLower())
                .Select(p => new RankedResult(p.p.Id, rankingData.First(r => r.PersonId == p.p.Id).Rank))
                .ToList();
        }
    }
}

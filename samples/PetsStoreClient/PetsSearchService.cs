using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PetsStoreClient.Nswag;

namespace PetsStoreClient
{
    public class PetsSearchService : IPetsSearchService
    {
        private readonly PetstoreClient _petsApiClient;
        private readonly IMapper _mapper;

        public PetsSearchService(IMapper mapper, PetstoreClient petsApiClient) 
        {
            _petsApiClient = petsApiClient;
            _mapper = mapper;
        }

        public async Task<Pet> LoadByIdAsync(long id)
        {
            var pet = await _petsApiClient.GetPetByIdAsync(id);
            if(pet != null)
            {
                return _mapper.Map<Pet>(pet);
            }
            return null;
        }

        public async Task<SearchPetsResult> SearchPetsAsync(SearchPetsQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            //TechDept: Since the public petstore API doesn't support pagination on the server side, we must load all the pets and do this on the client side in memory.            
            var pets = await _petsApiClient.FindPetsByStatusAsync(query.Statuses.Select(x=> (Anonymous)x));

            var result = new SearchPetsResult
            {
                TotalCount = pets.Count
            };
            if (query.Keyword != null)
            {
                pets = pets.Where(x => x.Name?.Contains(query.Keyword) ?? false).ToList();
            }

            //Use automapper for map  dto's types from remote  service into our domains types
            result.Pets = pets.Skip(query.Skip).Take(query.Take).Select(x => _mapper.Map<Pet>(x));

            return result;
        }
    }
}

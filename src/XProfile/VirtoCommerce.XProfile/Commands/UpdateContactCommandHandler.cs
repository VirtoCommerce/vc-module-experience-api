using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IMemberAggregateFactory _memberAggregateFactory;
        private readonly IMapper _mapper;

        public UpdateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, IMemberAggregateFactory factory, IMapper mapper)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _memberAggregateFactory = factory;
            _mapper = mapper;

        }
        public virtual async Task<ContactAggregate> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            //var sourceAggregate = _memberAggregateFactory.Create<ContactAggregate>(request);

            // Step 1. Validate Required Command Attributes

            // Step 2. Load Initial Contact
            var contactAggregate = await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<ContactAggregate>(request.Id);

            // Step 3. Partial Update (Required, Optional)
            _mapper.Map(request, contactAggregate.Contact, opt => opt.Items["OriginalInput"] = request.OriginalInput);

            //DemoParialtUpdateFromDict(contactAggregate.Contact, request.OriginalInput);

            // Step 4. Save
            await _contactAggregateRepository.SaveAsync(contactAggregate);
            //await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }

        public static object DemoParialtUpdateFromDict(object someObject, IDictionary<string, object> source)
        {
            var someObjectType = someObject.GetType();
            foreach (var item in source)
                someObjectType.GetProperty(item.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(someObject, item.Value, null);

            return someObject;
        }
    }
}

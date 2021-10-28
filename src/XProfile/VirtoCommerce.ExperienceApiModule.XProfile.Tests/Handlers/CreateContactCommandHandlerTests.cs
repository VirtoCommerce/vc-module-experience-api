using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Tests.Handlers
{
    public class CreateContactCommandHandlerTests : MoqHelper
    {
        [Fact]
        public async Task Handle_RequestWithDynamicProperties_UpdateDynamicPropertyCalled()
        {
            // Arragne
            var aggregateRepositoryMock = new Mock<IContactAggregateRepository>();
            var aggregateFactoryMock = new Mock<IMemberAggregateFactory>();
            var dynamicPropertyUpdaterServiceMock = new Mock<IDynamicPropertyUpdaterService>();
            var mapperMock = new Mock<IMapper>();
            var validator = new NewContactValidator();

            var contact = _fixture.Create<Contact>();
            var contactAggregae = new ContactAggregate { Member = contact };
            aggregateFactoryMock
                .Setup(x => x.Create<ContactAggregate>(It.IsAny<Contact>()))
                .Returns(contactAggregae);

            var handler = new CreateContactCommandHandler(aggregateRepositoryMock.Object,
                aggregateFactoryMock.Object,
                dynamicPropertyUpdaterServiceMock.Object,
                mapperMock.Object,
                validator);

            var command = _fixture.Create<CreateContactCommand>();

            // Act
            var aggregate = await handler.Handle(command, CancellationToken.None);

            // Assert
            dynamicPropertyUpdaterServiceMock.Verify(x => x.UpdateDynamicPropertyValues(It.Is<Contact>(x => x == contact),
                It.Is<IList<DynamicPropertyValue>>(x => x == command.DynamicProperties)), Times.Once);
        }
    }
}

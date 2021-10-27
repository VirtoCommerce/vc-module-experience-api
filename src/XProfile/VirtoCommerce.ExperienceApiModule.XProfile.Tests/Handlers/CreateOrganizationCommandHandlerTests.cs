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
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Tests.Handlers
{
    public class CreateOrganizationCommandHandlerTests : MoqHelper
    {
        [Fact]
        public async Task Handle_RequestWithDynamicProperties_UpdateDynamicPropertyCalled()
        {
            // Arragne
            var aggregateRepositoryMock = new Mock<IOrganizationAggregateRepository>();
            var aggregateFactoryMock = new Mock<IMemberAggregateFactory>();
            var dynamicPropertyUpdaterServiceMock = new Mock<IDynamicPropertyUpdaterService>();
            var mapperMock = new Mock<IMapper>();

            var organization = _fixture.Create<Organization>();
            var organizatnoAggregate = new OrganizationAggregate { Member = organization };
            aggregateFactoryMock
                .Setup(x => x.Create<OrganizationAggregate>(It.IsAny<Organization>()))
                .Returns(organizatnoAggregate);

            var handler = new CreateOrganizationCommandHandler(
                mapperMock.Object,
                aggregateRepositoryMock.Object,
                aggregateFactoryMock.Object,
                dynamicPropertyUpdaterServiceMock.Object);

            var command = _fixture.Create<CreateOrganizationCommand>();

            // Act
            var aggregate = await handler.Handle(command, CancellationToken.None);

            // Assert
            dynamicPropertyUpdaterServiceMock.Verify(x => x.UpdateDynamicPropertyValues(It.Is<Organization>(x => x == organization),
                It.Is<IList<DynamicPropertyValue>>(x => x == command.DynamicProperties)), Times.Once);
        }
    }
}

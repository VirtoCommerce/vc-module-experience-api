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
    public class UpdateContactCommandHandlerTests : MoqHelper
    {
        [Fact]
        public async Task Handle_RequestWithDynamicProperties_UpdateDynamicPropertyCalled()
        {
            // Arragne
            var aggregateRepositoryMock = new Mock<IContactAggregateRepository>();
            var dynamicPropertyUpdaterServiceMock = new Mock<IDynamicPropertyUpdaterService>();
            var mapperMock = new Mock<IMapper>();

            var contact = _fixture.Create<Contact>();
            var contactAggregae = new ContactAggregate { Member = contact };

            aggregateRepositoryMock
                .Setup(x => x.GetMemberAggregateRootByIdAsync<ContactAggregate>(It.IsAny<string>()))
                .ReturnsAsync(contactAggregae);

            var handler = new UpdateContactCommandHandler(aggregateRepositoryMock.Object,
                dynamicPropertyUpdaterServiceMock.Object,
                mapperMock.Object);

            var command = _fixture.Create<UpdateContactCommand>();

            // Act
            var aggregate = await handler.Handle(command, CancellationToken.None);

            // Assert
            dynamicPropertyUpdaterServiceMock.Verify(x => x.UpdateDynamicPropertyValues(It.Is<Contact>(x => x == contact),
                It.Is<IList<DynamicPropertyValue>>(x => x == command.DynamicProperties)), Times.Once);
        }
    }
}

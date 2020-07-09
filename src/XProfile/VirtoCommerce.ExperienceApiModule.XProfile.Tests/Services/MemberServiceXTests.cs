using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Tests.Services
{
    public class MemberServiceXTests
    {
        private readonly Mock<IMemberService> _memberServiceMock;
        private readonly Mock<IMemberSearchService> _memberSearchServiceMock;
        private readonly Mock<IServiceProvider> _servicesMock;
        private readonly Mock<IMapper> _mapperMock;

        public MemberServiceXTests()
        {
            _memberSearchServiceMock = new Mock<IMemberSearchService>();
            _memberServiceMock = new Mock<IMemberService>();
            _servicesMock = new Mock<IServiceProvider>();
            _mapperMock = new Mock<IMapper>();
        }

        //[Fact]
        //public async Task UpdateContactAddressesAsync_UpdatedAddress()
        //{
        //    //Arrange
        //    var contactId = Guid.NewGuid().ToString();
        //    var address = new Address { City = "Los Angeles", CountryCode = "USA", CountryName = "United States", PostalCode = "34535", RegionId = "CA", Line1 = "20945 Devonshire St Suite 102" };
        //    var contact = new Contact() { Id = contactId, SecurityAccounts = new List<ApplicationUser>() };
        //    _memberServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //        .ReturnsAsync(contact);

        //    var service = GetMemberServiceX();

        //    //Act
        //    var result = await service.UpdateContactAddressesAsync(contactId, new List<Address> { address });

        //    //Assert
        //    result.Addresses.FirstOrDefault().Should().Be(address);
        //}

        //[Fact]
        //public async Task UpdateContactAsync_Updated()
        //{
        //    //Arrange
        //    var contactId = Guid.NewGuid().ToString();
        //    var contact = new Contact() { Id = contactId, SecurityAccounts = new List<ApplicationUser>() };
        //    _memberServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //        .ReturnsAsync(contact);
        //    var userUpdateInfo = new UserUpdateInfo { Id = contactId, FullName = "some name" };
        //    _memberServiceMock.Setup(x => x.SaveChangesAsync(It.IsAny<Member[]>()))
        //        .Callback(() =>
        //        {
        //            contact.FullName = userUpdateInfo.FullName;
        //        });

        //    var service = GetMemberServiceX();

        //    //Act
        //    var result = await service.UpdateContactAsync(userUpdateInfo);

        //    //Assert
        //    result.Contact.Id.Should().Be(contactId);
        //    result.Contact.FullName.Should().Be(contact.FullName);
        //}

        //[Fact]
        //public async Task UpdateOrganizationAsync()
        //{
        //    //Arrange
        //    var organizationId = Guid.NewGuid().ToString();
        //    var address = new Address { City = "Los Angeles", CountryCode = "USA", CountryName = "United States", PostalCode = "34535", RegionId = "CA", Line1 = "20945 Devonshire St Suite 102" };
        //    var organization = new Organization() { Id = organizationId };
        //    _memberServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //        .ReturnsAsync(organization);
        //    var organizationUpdateInfo = new OrganizationUpdateInfo { Id = organizationId, Name = "some name", Addresses = new List<Address> { address } };
        //    var user = new ApplicationUser { MemberId = organizationId };
        //    _memberServiceMock.Setup(x => x.SaveChangesAsync(It.IsAny<Member[]>()))
        //        .Callback(() =>
        //        {
        //            organization.Name = organizationUpdateInfo.Name;
        //            organization.Addresses = new List<Address> { address };
        //        });

        //    var service = GetMemberServiceX();

        //    //Act
        //    var result = await service.UpdateOrganizationAsync(organizationUpdateInfo);

        //    //Assert
        //    result.Id.Should().Be(organizationId);
        //    result.Name.Should().Be(organization.Name);
        //    result.Addresses.First().Should().Be(address);
        //}

        //[Fact]
        //public async Task SearchOrganizationContactsAsync_ReturnProfileSearchResult()
        //{
        //    //Arrange
        //    var user = new ApplicationUser { MemberId = Guid.NewGuid().ToString() };
            
        //    var contact = new Contact() { Id = user.MemberId, SecurityAccounts = new List<ApplicationUser> { user } };
        //    var membersSearchCriteria = AbstractTypeFactory<MembersSearchCriteria>.TryCreateInstance();
        //    _memberSearchServiceMock.Setup(x => x.SearchMembersAsync(membersSearchCriteria))
        //        .ReturnsAsync(
        //        new MemberSearchResult
        //        {
        //            Results = new List<Member> { contact }
        //        });
        //    var service = GetMemberServiceX();
        //    _memberServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //        .ReturnsAsync(contact);

        //    //Act
        //    var result = await service.SearchOrganizationContactsAsync(membersSearchCriteria);

        //    //Assert
        //    result.Should().NotBeNull();
        //    result.Results.Should().NotBeEmpty();
        //    result.Results.FirstOrDefault().Id.Should().Be(user.MemberId);
        //}


        //private MemberServiceX GetMemberServiceX()
        //{
        //    var serviceScope = new Mock<IServiceScope>();
        //    serviceScope.Setup(x => x.ServiceProvider).Returns(_servicesMock.Object);
        //    var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        //    serviceScopeFactory
        //        .Setup(x => x.CreateScope())
        //        .Returns(serviceScope.Object);
        //    _servicesMock
        //        .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
        //        .Returns(serviceScopeFactory.Object);

        //    return new MemberServiceX(_memberServiceMock.Object,
        //        _memberSearchServiceMock.Object,
        //        _servicesMock.Object,
        //        _mapperMock.Object);
        //}
    }
}

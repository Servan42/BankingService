using AutoMapper;
using BankingService.Core.API.MapperProfile;

namespace BankingService.Tests
{
    internal class MapperTests
    {
        [Test]
        public void Should_have_a_valid_Core_Api_Mapper_Profile()
        {
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CoreApiProfile>();
            }).AssertConfigurationIsValid();
        }

        [Test]
        public void Should_have_a_valid_Core_Spi_Mapper_Profile()
        {
            new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CoreApiProfile>();
            }).AssertConfigurationIsValid();
        }
    }
}

using AutoMapper;
using AutoMapper.Mappers;
using WebApi.Hal.Web;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class WebTests
    {
        [Fact]
        public void automapper_test()
        {
            var store = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());
            AutomapperMappings.RegisterMaps(store);

            store.AssertConfigurationIsValid();
        }
    }
}
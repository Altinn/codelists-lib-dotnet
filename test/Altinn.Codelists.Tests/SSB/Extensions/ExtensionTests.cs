using Altinn.App.Core.Features;
using Altinn.Codelists.SSB;
using Altinn.Codelists.SSB.Extensions;
using Altinn.Codelists.SSB.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Altinn.Codelists.Tests.SSB.Extensions;

public class ExtensionTests
{
    [Fact]
    public void AddSSBClassifications()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSSBClassificationCodelistProvider("sivilstand", Classification.MaritalStatus);
        services.AddSSBClassificationCodelistProvider("yrker", Classification.Occupations);

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        IEnumerable<IClassificationsClient> classificationsClients =
            serviceProvider.GetServices<IClassificationsClient>();

        classificationsClients.Should().HaveCount(1);

        var optionsProviders = serviceProvider.GetServices<IAppOptionsProvider>();
        optionsProviders.Should().HaveCount(2);
    }
}

using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries;

public abstract class LocalizedSettingQueryHandler<TQuery> : IQueryHandler<TQuery, LocalizedSettingResponse>
    where TQuery : LocalizedSettingQuery
{
    private readonly ILocalizableSettingService _localizableSettingService;

    protected LocalizedSettingQueryHandler(ILocalizableSettingService localizableSettingService)
    {
        _localizableSettingService = localizableSettingService;
    }

    protected abstract SettingDescriptor Setting { get; }

    public async Task<LocalizedSettingResponse> Handle(TQuery request, CancellationToken cancellationToken)
    {
        var result = AbstractTypeFactory<LocalizedSettingResponse>.TryCreateInstance();
        result.Items = await _localizableSettingService.GetValuesAsync(Setting.Name, request.CultureName);

        return result;
    }
}

using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.Settings;
using OrderSettings = VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries;

public class PaymentStatusesQueryHandler : LocalizedSettingQueryHandler<PaymentStatusesQuery>
{
    public PaymentStatusesQueryHandler(ILocalizableSettingService localizableSettingService)
        : base(localizableSettingService)
    {
    }

    protected override SettingDescriptor Setting => OrderSettings.PaymentInStatus;
}

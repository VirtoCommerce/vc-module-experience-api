using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.XOrder.Core.Queries;
using OrderSettings = VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.XOrder.Data.Queries;

public class OrderStatusesQueryHandler : LocalizedSettingQueryHandler<OrderStatusesQuery>
{
    public OrderStatusesQueryHandler(ILocalizableSettingService localizableSettingService)
        : base(localizableSettingService)
    {
    }

    protected override SettingDescriptor Setting => OrderSettings.OrderStatus;
}

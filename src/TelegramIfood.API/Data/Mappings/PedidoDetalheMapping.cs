using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Data.Mappings;
public class PedidoDetalheMapping : IEntityTypeConfiguration<IfoodPedidoDetalhe>
{
    public void Configure(EntityTypeBuilder<IfoodPedidoDetalhe> builder)
    {
        builder.Ignore(x => x.merchant);
        builder.Ignore(x => x.additionalInfo);
        builder.Ignore(x => x.total);
        builder.Ignore(x => x.payments);
        builder.Ignore(x => x.customer);
        builder.Ignore(x => x.delivery);

        builder.Property<decimal>("totalPedido").HasPrecision(18, 2);
    }
}

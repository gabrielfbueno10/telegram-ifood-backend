namespace TelegramIfood.Events.Models.Ifood;

public class IfoodPedidoDetalhe
{
    public Guid id { get; set; }
    public Delivery delivery { get; set; }
    public string orderType { get; set; }
    public string orderTiming { get; set; }
    public string displayId { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime preparationStartDateTime { get; set; }
    public bool isTest { get; set; }
    public Merchant merchant { get; set; }
    public Customer customer { get; set; }
    public IEnumerable<Item> items { get; set; }
    public string salesChannel { get; set; }
    public Total total { get; set; }
    public Payments payments { get; set; }
    public Additionalinfo additionalInfo { get; set; }
    public decimal totalPedido { get; set; }
    public string pedidoStatus { get; set; }
    public Guid merchantId { get; set; }
    public void PreencherValoresPedido()
    {
        pedidoStatus = "PLC";
        totalPedido = total?.orderAmount ?? 0;
        merchantId = merchant?.id ?? Guid.Empty;
    }
}

public class Delivery
{
    public string mode { get; set; }
    public string deliveredBy { get; set; }
    public DateTime deliveryDateTime { get; set; }
    public string observations { get; set; }
    public Deliveryaddress deliveryAddress { get; set; }
    public string pickupCode { get; set; }
}

public class Deliveryaddress
{
    public string streetName { get; set; }
    public string streetNumber { get; set; }
    public string formattedAddress { get; set; }
    public string neighborhood { get; set; }
    public string complement { get; set; }
    public string postalCode { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public string reference { get; set; }
    public Coordinates coordinates { get; set; }
}

public class Coordinates
{
    public float latitude { get; set; }
    public float longitude { get; set; }
}

public class Merchant
{
    public Guid id { get; set; }
    public string name { get; set; }
}

public class Customer
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string documentNumber { get; set; }
    public Phone phone { get; set; }
    public int ordersCountOnMerchant { get; set; }
    public string segmentation { get; set; }
}

public class Phone
{
    public string number { get; set; }
    public string localizer { get; set; }
    public DateTime localizerExpiration { get; set; }
}

public class Total
{
    public float subTotal { get; set; }
    public decimal deliveryFee { get; set; }
    public int benefits { get; set; }
    public decimal orderAmount { get; set; }
    public decimal additionalFees { get; set; }
}

public class Payments
{
    public decimal prepaid { get; set; }
    public int pending { get; set; }
    public Method[] methods { get; set; }
}

public class Method
{
    public decimal value { get; set; }
    public string currency { get; set; }
    public string method { get; set; }
    public string type { get; set; }
    public Card card { get; set; }
    public bool prepaid { get; set; }
}

public class Card
{
    public string brand { get; set; }
}

public class Additionalinfo
{
    public Metadata metadata { get; set; }
}

public class Metadata
{
    public string customerEmail { get; set; }
    public string developerEmail { get; set; }
    public string developerId { get; set; }
}

public class Item
{
    public Guid id { get; set; }
    public int index { get; set; }
    public string uniqueId { get; set; }
    public string name { get; set; }
    public string unit { get; set; }
    public int quantity { get; set; }
    public decimal unitPrice { get; set; }
    public int optionsPrice { get; set; }
    public decimal totalPrice { get; set; }
    public decimal price { get; set; }
}

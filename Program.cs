using System;
using System.Security.Cryptography.X509Certificates;

namespace FunctionalDeliveryCalcuator 
{
    public enum DeliveryType 
    {
        
        Pickup,
        Courier,
        DoorToDoor

    }
    public enum DeliveryZone 
    {
        
        City,
        OutsideCity,
        Remote

    }

    public static class DeliveryPricing 
    {

        public static decimal ApplyRule(decimal price, Func<decimal, decimal> rule) => rule(price);

        public static decimal RoundTwoDecimals(decimal price) => 
            Math.Round(price, 2, MidpointRounding.AwayFromZero);

        public static Func<decimal, decimal> GetItemsRule(int itemCount) 
        {
            if (itemCount > 8)
                return price => price * 1.20m;
            if (itemCount >= 4)
                return price => price * 1.10m;
            return price => price;
        }

        public static Func<decimal, decimal> GetDeliveryTypeRule(DeliveryType type) => type switch
        {

            DeliveryType.Pickup => price => price * 0.80m,
            DeliveryType.Courier => price => price,
            DeliveryType.DoorToDoor => price => price * 1.15m,
            _ => price => price 

        };

        public static Func<decimal, decimal> GetZoneRule(DeliveryZone zone) => zone switch
        {

            DeliveryZone.OutsideCity => price => price * 1.25m,
            DeliveryZone.City => price => price ,
            DeliveryZone.Remote => price => price,
            _ => price => price

        };
        public static readonly Func<decimal, bool, decimal> ExpressRule =
            (price, isExpress) => isExpress ? price * 1.30m : price;

        public static decimal CalculateFinalPrice(
            decimal basePrice,
            int itemCount,
            DeliveryType type,
            DeliveryZone zone,
            bool isExpress)
        {
            Func<decimal, decimal> itemsRule = GetItemsRule(itemCount);
            Func<decimal, decimal> typeRule = GetDeliveryTypeRule(type);
            Func<decimal, decimal> zoneRule = GetZoneRule(zone);

            decimal price = basePrice;
            price = ApplyRule(price, itemsRule);
            price = ApplyRule(price, typeRule);
            price = ApplyRule(price, zoneRule);
            price = ExpressRule(price, isExpress);

            return RoundTwoDecimals(price);
        }
    
    }

    public static class Program 
    {
        public static void Main() 
        {
            Console.WriteLine("=== Delivery Cost Calculator ===");

            decimal? basePrice = ReadBasePrice();
            if (basePrice is null ) return;

            int? itemCount = ReadItemCount();
            if (itemCount is null) return;

            DeliveryType? deliveryType = ReadDeliveryType();
            if (deliveryType is null) return;

            DeliveryZone? deliveryZone = ReadDeliveryZone();
            if (deliveryZone is null) return;

            bool? isExpress = ReadExpressStatus();
            if (isExpress is null) return;

            decimal finalPrice = DeliveryPricing.CalculateFinalPrice(
                basePrice.Value, itemCount.Value, deliveryType.Value, deliveryZone.Value, isExpress.Value);

            Console.WriteLine($"Final delivery price:{finalPrice:0.00}");
        }

        private static decimal ? ReadBasePrice()
        {
            Console.Write("Enter base delivery price: ");
            string? input = Console.ReadLine();

            if(!decimal.TryParse(input,out decimal price))
            {
                Console.WriteLine("Error:base price must be a valid decimakl number.Ex:49.9).");
                return null;
            }

            if(price < 0)
            {
                Console.WriteLine("Error: base price cannot be negative.");
                return null;
            }

            return price;
        }
        private static int? ReadItemCount()
        {
            Console.Write("Enter number of items: ");
            string? input = Console.ReadLine();

            if(!int.TryParse(input, out int items))
            {
                Console.WriteLine("Error:number of items must be a whole number.");
                return null;
            }

            return items;

        }

        private static DeliveryType? ReadDeliveryType()
        {
            Console.Write("Enter delivery type (Pickup, Courier, DoorToDoor): ");
            string? input = Console.ReadLine();

            if (!Enum.TryParse(input, ignoreCase: true, out DeliveryType type) ||
                !Enum.IsDefined(typeof(DeliveryType), type))
            {
                Console.WriteLine("Error: delivery type must be one of Pickup, Courier, DoorToDoor.");
                return null;
            }

            return type;
        }

        private static DeliveryZone? ReadDeliveryZone()
        {
            Console.Write("Enter delivery zone (City, OutsideCity, Remote): ");
            string? input = Console.ReadLine();

            if (!Enum.TryParse(input, ignoreCase: true, out DeliveryZone zone) ||
                !Enum.IsDefined(typeof(DeliveryZone), zone))
            {
                Console.WriteLine("Error: delivery zone must be one of City, OutsideCity, Remote.");
                return null;
            }

            return zone;
        }

        private static bool? ReadExpressStatus()
        {
            Console.Write("Express delivery? (true/false): ");
            string? input = Console.ReadLine();

            if (!bool.TryParse(input, out bool isExpress))
            {
                Console.WriteLine("Error: express delivery status must be 'true' or 'false'.");
                return null;
            }

            return isExpress;
        }

    }


}
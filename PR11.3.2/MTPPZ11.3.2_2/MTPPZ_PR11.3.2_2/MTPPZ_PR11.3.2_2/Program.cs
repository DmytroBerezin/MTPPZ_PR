using System;

namespace RestaurantTemplateMethodModule
{
    public abstract class DishPreparationTemplate
    {

        public void PrepareAndServeOrder(int tableNumber)
        {
            AcceptOrder(tableNumber);
            PrepareIngredients();
            CookDish();
            PlateAndGarnish();
            NotifyWaiter(tableNumber);
            Console.WriteLine(new string('-', 55));
        }

        protected void AcceptOrder(int tableNumber)
        {
            Console.WriteLine($"[Кухня | Столик №{tableNumber}] Отримано чек-замовлення у роботу.");
        }


        protected abstract void PrepareIngredients();
        protected abstract void CookDish();
        protected abstract void PlateAndGarnish();

        protected void NotifyWaiter(int tableNumber)
        {
            Console.WriteLine($"[Видача | Столик №{tableNumber}] Страва готова! Сповіщення відправлено офіціанту.");
        }
    }


    public class PastaPreparation : DishPreparationTemplate
    {
        protected override void PrepareIngredients()
        {
            Console.WriteLine("  1. Підготовка спагеті з твердих сортів пшениці, гуанчале, яєчних жовтків та сиру Пекоріно.");
        }

        protected override void CookDish()
        {
            Console.WriteLine("  2. Відварювання пасти до стану al dente (8 хв) та обсмажування гуанчале до хрусткої скоринки.");
        }

        protected override void PlateAndGarnish()
        {
            Console.WriteLine("  3. Викладання пасти у підігріту глибоку тарілку, посипання тертим сиром та свіжомеленим перцем.");
        }
    }

    public class SteakPreparation : DishPreparationTemplate
    {
        private readonly string _doneness;

        public SteakPreparation(string doneness = "Medium Rare")
        {
            _doneness = doneness;
        }

        protected override void PrepareIngredients()
        {
            Console.WriteLine("  1. Зачистка мармурової яловичини Рібай, маринування в морській солі, розмарині та часнику.");
        }

        protected override void CookDish()
        {
            Console.WriteLine($"  2. Обсмажування стейка на грилі до ступеня просмаження '{_doneness}' та відпочинок м'яса 5 хв.");
        }

        protected override void PlateAndGarnish()
        {
            Console.WriteLine("  3. Подача на дерев'яній дошці з овочами гриль та фірмовим соусом Деміглас.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== АВТОМАТИЗАЦІЯ КУХНІ РЕСТОРАНУ (TEMPLATE METHOD) ===\n");

            DishPreparationTemplate pastaOrder = new PastaPreparation();
            pastaOrder.PrepareAndServeOrder(tableNumber: 4);

            DishPreparationTemplate steakOrder = new SteakPreparation("Medium");
            steakOrder.PrepareAndServeOrder(tableNumber: 9);
        }
    }
}
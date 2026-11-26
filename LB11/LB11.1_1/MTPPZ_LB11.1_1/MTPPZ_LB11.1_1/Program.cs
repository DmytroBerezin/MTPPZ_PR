using System;
using System.Collections.Generic;

namespace Lab11_Builder
{
    public class Sandwich
    {
        public string Name { get; set; }
        public List<string> Ingredients { get; } = new List<string>();

        public Sandwich(string name)
        {
            Name = name;
        }

        public void AddIngredient(string ingredient)
        {
            Ingredients.Add(ingredient);
        }

        public void ShowInfo()
        {
            Console.WriteLine($"=== {Name} ===");
            Console.WriteLine("Склад: " + (Ingredients.Count > 0 ? string.Join(", ", Ingredients) : "порожньо"));
            Console.WriteLine();
        }
    }

    public interface ISandwichBuilder
    {
        void AddBread();
        void AddMeat();
        void AddVegetables();
        void AddSauce();
        Sandwich GetSandwich();
    }

    public class VeggieSandwichBuilder : ISandwichBuilder
    {
        private Sandwich _sandwich = new Sandwich("Вегетаріанський сендвіч-ролл (Veggie)");

        public void AddBread()
        {
            // Для дієтичного овочевого варіанту хліб не додається
        }

        public void AddMeat()
        {
            // М'ясо не додається
        }

        public void AddVegetables()
        {
            _sandwich.AddIngredient("Листя салату Айсберг");
            _sandwich.AddIngredient("Свіжі томати");
            _sandwich.AddIngredient("Огірок");
            _sandwich.AddIngredient("Болгарський перець");
        }

        public void AddSauce()
        {
            _sandwich.AddIngredient("Оливковий соус песто");
        }

        public Sandwich GetSandwich()
        {
            Sandwich result = _sandwich;
            _sandwich = new Sandwich("Вегетаріанський сендвіч-ролл (Veggie)");
            return result;
        }
    }

    public class MeatLoversBuilder : ISandwichBuilder
    {
        private Sandwich _sandwich = new Sandwich("Сендвіч 'М'ясний мікс' (Meat Lovers)");

        public void AddBread()
        {
            _sandwich.AddIngredient("Хрустка чіабата з висівками");
        }

        public void AddMeat()
        {
            _sandwich.AddIngredient("Куряче філе гриль");
            _sandwich.AddIngredient("Бекон смажений");
            _sandwich.AddIngredient("Шинка");
        }

        public void AddVegetables()
        {
            _sandwich.AddIngredient("Мариновані огірки");
            _sandwich.AddIngredient("Червона цибуля");
        }

        public void AddSauce()
        {
            _sandwich.AddIngredient("Фірмовий соус барбекю (BBQ)");
        }

        public Sandwich GetSandwich()
        {
            Sandwich result = _sandwich;
            _sandwich = new Sandwich("Сендвіч 'М'ясний мікс' (Meat Lovers)");
            return result;
        }
    }

    public class Director
    {
        public void BuildSandwich(ISandwichBuilder builder)
        {
            builder.AddBread();
            builder.AddMeat();
            builder.AddVegetables();
            builder.AddSauce();
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var director = new Director();

            ISandwichBuilder veggieBuilder = new VeggieSandwichBuilder();
            director.BuildSandwich(veggieBuilder);
            Sandwich veggieSandwich = veggieBuilder.GetSandwich();
            veggieSandwich.ShowInfo();

            ISandwichBuilder meatBuilder = new MeatLoversBuilder();
            director.BuildSandwich(meatBuilder);
            Sandwich meatSandwich = meatBuilder.GetSandwich();
            meatSandwich.ShowInfo();
        }
    }
}
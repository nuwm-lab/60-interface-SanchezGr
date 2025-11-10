using System;
using System.Globalization;

namespace LabWork6
{
    //Інтерфейси
    public interface IPrintable
    {
        string RoleName { get; }
        void Show();
    }

    public interface IWorker
    {
        void Experience(out int years, out int months, out int days);
    }

    //11
    static class ConsoleHelper
    {
        public static string ReadLetters(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim();
                if (s.Length == 0) { Console.WriteLine("Введіть дані"); continue; }
                bool ok = true;
                foreach (char c in s)
                    if (!(char.IsLetter(c) || c == ' ' || c == '-' || c == '\'' || c == '’')) { ok = false; break; }
                if (ok) return s;
                Console.WriteLine("Помилка: дозволені лише літери, пробіл, дефіс, апостроф.");
            }
        }

        public static DateTime ReadDate(string prompt)
        {
            string[] formats = { "yyyy-MM-dd", "dd.MM.yyyy" };
            while (true)
            {
                Console.Write(prompt);
                string s = (Console.ReadLine() ?? "").Trim();
                if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var dt))
                    return dt.Date;
                Console.WriteLine("Неправильний формат. Приклади: 2023-09-01 або 01.09.2023");
            }
        }

        public static bool IsPalindrome(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var buf = new System.Text.StringBuilder(s.Length);
            foreach (char c in s) if (char.IsLetter(c)) buf.Append(char.ToLowerInvariant(c));
            int l = 0, r = buf.Length - 1;
            while (l < r) { if (buf[l] != buf[r]) return false; l++; r--; }
            return buf.Length > 0;
        }
    }

    // Абстрактний базовий клас 
    abstract class Person : IPrintable
    {
        // Інкапсуляція через властивості 
        public string LastName  { get; protected set; } = "";
        public string FirstName { get; protected set; } = "";
        public string University{ get; protected set; } = "";

        protected Person() { }
        protected Person(string ln, string fn, string un)
        {
            LastName = ln;
            FirstName = fn;
            University = un;
        }

        ~Person() { } // деструктор для вимог

        public abstract string RoleName { get; }

        // Віртуальні методи
        public virtual void InputMain()
        {
            LastName   = ConsoleHelper.ReadLetters("Прізвище практиканта: ");
            FirstName  = ConsoleHelper.ReadLetters("Ім'я практиканта: ");
            University = ConsoleHelper.ReadLetters("ВНЗ: ");
        }

        public virtual void Show()
        {
            Console.WriteLine($"\nПрактикант: {FirstName} {LastName}");
            Console.WriteLine($"ВНЗ: {University}");
            Console.WriteLine($"Симетричне прізвище: {(ConsoleHelper.IsPalindrome(LastName) ? "Так" : "Ні")}");
        }
    }

    //  Похідний клас Практикант 
    class Praktykant : Person
    {
        public Praktykant() : base() { }
        public Praktykant(string ln, string fn, string un) : base(ln, fn, un) { }
        ~Praktykant() { }

        public override string RoleName => "Практикант";
        
    }

    //  Похідний клас Працівник фірми
    class PracivnykFirmy : Person, IWorker
    {
        public string School   { get; private set; } = "";
        public string Position { get; private set; } = "";
        public DateTime HireDate { get; private set; }

        public PracivnykFirmy() : base() { }
        public PracivnykFirmy(string ln, string fn, string un, string school, string pos, DateTime hd)
            : base(ln, fn, un)
        {
            School = school;
            Position = pos;
            HireDate = hd.Date;
        }
        ~PracivnykFirmy() { }

        public override string RoleName => "Працівник фірми";

        public override void InputMain()
        {
            LastName   = ConsoleHelper.ReadLetters("Прізвище працівника: ");
            FirstName  = ConsoleHelper.ReadLetters("Ім'я працівника: ");
            University = ConsoleHelper.ReadLetters("ВНЗ: ");
            School     = ConsoleHelper.ReadLetters("Заклад, який закінчив: ");
            Position   = ConsoleHelper.ReadLetters("Посада: ");
            HireDate   = ConsoleHelper.ReadDate("Дата прийому (yyyy-MM-dd або dd.MM.yyyy): ");
        }

        public void Experience(out int years, out int months, out int days)
        {
            var now = DateTime.Today;
            if (now < HireDate) { years = months = days = 0; return; }
            years  = now.Year  - HireDate.Year;
            months = now.Month - HireDate.Month;
            days   = now.Day   - HireDate.Day;
            if (days < 0)   { var prev = now.AddMonths(-1); days += DateTime.DaysInMonth(prev.Year, prev.Month); months--; }
            if (months < 0) { months += 12; years--; }
        }

        public override void Show()
        {
            Console.WriteLine($"\nПрацівник фірми: {FirstName} {LastName}");
            Console.WriteLine($"Посада: {Position}");
            Console.WriteLine($"ВНЗ: {University}");
            Console.WriteLine($"Закінчив: {School}");
            Console.WriteLine($"Дата прийому: {HireDate:yyyy-MM-dd}");
            Experience(out int y, out int m, out int d);
            Console.WriteLine($"Стаж роботи: {y} р. {m} міс. {d} дн.");
            Console.WriteLine($"Симетричне прізвище: {(ConsoleHelper.IsPalindrome(LastName) ? "Так" : "Ні")}");
        }
    }

  
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.Write("Оберіть режим (1 — працівник, 2 — практикант): ");
            string choice = (Console.ReadLine() ?? "").Trim();

            //  одночасно базовий тип + інтерфейс
            Person person;
            if (choice == "1") person = new PracivnykFirmy();
            else if (choice == "2") person = new Praktykant();
            else { Console.WriteLine("Невірний вибір."); return; }

            IPrintable printable = (IPrintable)person; // робота через інтерфейс

            person.InputMain();  // виклик віртуального методу через базовий тип (динамічний поліморфізм)
            printable.Show();    // виклик через інтерфейс

            if (person is IWorker w)
            {
                w.Experience(out int y, out int m, out int d);
               
            }
        }
    }
}

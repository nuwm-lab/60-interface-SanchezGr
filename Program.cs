
﻿using System;

using System;

using System.Globalization;

namespace LabWork6
{
    abstract class Person
    {
        protected string lastName;
        protected string firstName;
        protected string university;

        protected Person() { }
        protected Person(string ln, string fn, string u)
        {
            lastName = ln; firstName = fn; university = u;
        }

        ~Person() { }

        public abstract string RoleName { get; }

        protected static string ReadLetters(string prompt)
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
                Console.WriteLine("Помилка вводу.");
            }
        }

        protected static DateTime ReadDate(string prompt)
        {
            string[] formats = { "yyyy-MM-dd", "dd.MM.yyyy" };
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var dt))
                    return dt.Date;
                Console.WriteLine("Неправильний формат");
            }
        }

        protected static bool IsPalindrome(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var buf = new System.Text.StringBuilder(s.Length);
            foreach (char c in s) if (char.IsLetter(c)) buf.Append(char.ToLowerInvariant(c));
            if (buf.Length == 0) return false;
            int l = 0, r = buf.Length - 1;
            while (l < r) { if (buf[l] != buf[r]) return false; l++; r--; }
            return true;
        }

        public virtual void InputMain()
        {
            lastName = ReadLetters("Прізвище практиканта: ");
            firstName = ReadLetters("Ім'я практиканта: ");
            university = ReadLetters("ВНЗ: ");
        }

        public virtual void Show()
        {
            Console.WriteLine($"\nПрактикант: {firstName} {lastName}");
            Console.WriteLine($"ВНЗ: {university}");
            Console.WriteLine($"Симетричне прізвище: {(IsPalindrome(lastName) ? "Так" : "Ні")}");
        }
    }

    class Praktykant : Person
    {
        public Praktykant() : base() { }
        public Praktykant(string ln, string fn, string u) : base(ln, fn, u) { }
        ~Praktykant() { }

        public override string RoleName => "Практикант";

        public override void InputMain()
        {
            lastName = ReadLetters("Прізвище практиканта: ");
            firstName = ReadLetters("Ім'я практиканта: ");
            university = ReadLetters("ВНЗ: ");
        }

        public override void Show()
        {
            Console.WriteLine($"\nПрактикант: {firstName} {lastName}");
            Console.WriteLine($"ВНЗ: {university}");
            Console.WriteLine($"Симетричне прізвище: {(IsPalindrome(lastName) ? "Так" : "Ні")}");
        }
    }

    class PracivnykFirmy : Person
    {
        private string school;
        private string position;
        private DateTime hireDate;

        public PracivnykFirmy() : base() { }
        public PracivnykFirmy(string ln, string fn, string u, string sch, string pos, DateTime hd)
            : base(ln, fn, u)
        {
            school = sch; position = pos; hireDate = hd.Date;
        }
        ~PracivnykFirmy() { }

        public override string RoleName => "Працівник фірми";

        public override void InputMain()
        {
            lastName = ReadLetters("Прізвище працівника: ");
            firstName = ReadLetters("Ім'я працівника: ");
            university = ReadLetters("ВНЗ: ");
            school = ReadLetters("Заклад, який закінчив: ");
            position = ReadLetters("Посада: ");
            hireDate = ReadDate("Дата прийому (yyyy-MM-dd або dd.MM.yyyy): ");
        }

        public void Experience(out int years, out int months, out int days)
        {
            var now = DateTime.Today;
            if (now < hireDate) { years = months = days = 0; return; }
            years = now.Year - hireDate.Year;
            months = now.Month - hireDate.Month;
            days = now.Day - hireDate.Day;
            if (days < 0) { var prev = now.AddMonths(-1); days += DateTime.DaysInMonth(prev.Year, prev.Month); months--; }
            if (months < 0) { months += 12; years--; }
        }

        public override void Show()
        {
            Console.WriteLine($"\nПрацівник фірми: {firstName} {lastName}");
            Console.WriteLine($"Посада: {position}");
            Console.WriteLine($"ВНЗ: {university}");
            Console.WriteLine($"Закінчив: {school}");
            Console.WriteLine($"Дата прийому: {hireDate:yyyy-MM-dd}");
            Experience(out int y, out int m, out int d);
            Console.WriteLine($"Стаж роботи: {y} р. {m} міс. {d} дн.");
            Console.WriteLine($"Симетричне прізвище: {(IsPalindrome(lastName) ? "Так" : "Ні")}");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("Оберіть режим (1 — працівник, 2 — практикант): ");
            string choose = (Console.ReadLine() ?? "").Trim();

            Person p;
            if (choose == "1") p = new PracivnykFirmy();
            else if (choose == "2") p = new Praktykant();
            else { Console.WriteLine("Невірний вибір."); return; }

            p.InputMain();
            p.Show();
        }
    }
}

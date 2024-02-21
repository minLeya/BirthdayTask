using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{

    static List<Birthday> birthdays = new List<Birthday>();
    static string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "birthdays.txt");


    static void Main()
    {
        Console.WriteLine("Поздравлятор\n");
        LoadDataFromFile();
        ShowUpcomingBirthdays();

        while (true)
        {
            Console.WriteLine("\n");
            Console.WriteLine("1. Показать все дни рождения");
            Console.WriteLine("2. Показать сегодняшние и ближайшие дни рождения");
            Console.WriteLine("3. Добавить день рождения");
            Console.WriteLine("4. Удалить день рождения");
            Console.WriteLine("5. Редактировать день рождения");
            Console.WriteLine("0. Выход");

            Console.Write("\nВыберите действие: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("\n");
                    ShowAllBirthdays();
                    break;
                case "2":
                    Console.Write("\n");
                    ShowUpcomingBirthdays();
                    break;
                case "3":
                    AddBirthday();
                    break;
                case "4":
                    Console.Write("\n");
                    RemoveBirthday();
                    break;
                case "5":
                    Console.Write("\n");
                    EditBirthday();
                    break;
                case "0":
                    Console.Write("\n");
                    SaveDataToFile();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\nНеверный выбор. Пожалуйста, повторите.");
                    break;
            }
        }
    }

    static void ShowAllBirthdays()
    {
        var allBirthdays = birthdays
            .OrderBy(b => b.DateOfBirth.Month)
            .ThenBy(b => b.DateOfBirth.Day)
            .ToList();

        Console.WriteLine("Все дни рождения:");

        foreach (var birthday in allBirthdays)
        {
            Console.WriteLine($"{birthday.Name} ({birthday.DateOfBirth.ToString("dd.MM.yyyy")})");
        }
    }


    static void ShowUpcomingBirthdays()
    {
        var today = DateTime.Today;
        var upcomingBirthdays = birthdays
            .Where(b => (b.DateOfBirth.Month == today.Month && b.DateOfBirth.Day >= today.Day) ||
                        (b.DateOfBirth.Month == today.AddMonths(1).Month && b.DateOfBirth.Day <= today.AddMonths(1).Day))
            .OrderBy(b => b.DateOfBirth.Month)
            .ThenBy(b => b.DateOfBirth.Day)
            .ToList();

        Console.WriteLine("Сегодняшние и ближайшие дни рождения:");

        foreach (var birthday in upcomingBirthdays)
        {
            Console.WriteLine($"{birthday.Name} ({birthday.DateOfBirth.ToString("dd.MM.yyyy")})");
        }
    }



    static void AddBirthday()
    {
        Console.Write("Введите имя: ");
        string name = Console.ReadLine();

        DateTime dateOfBirth;

        do
        {
            Console.Write("Введите дату рождения (в формате ДД.ММ.ГГГГ): ");
            string inputDate = Console.ReadLine();

            if (DateTime.TryParseExact(inputDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth))
            {
                if (dateOfBirth.Date <= DateTime.Today)
                {
                    break; // Выход из цикла, если введена корректная дата, и она не позднее сегодняшнего дня
                }
                else
                {
                    Console.WriteLine("Нельзя добавить день рождения, который будет позже сегодняшнего дня. Пожалуйста, повторите ввод.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат даты. Пожалуйста, повторите ввод.");
            }

        } while (true);

        birthdays.Add(new Birthday { Name = name, DateOfBirth = dateOfBirth });
        Console.WriteLine("День рождения успешно добавлен.");
    }



    static void RemoveBirthday()
    {
        ShowAllBirthdays();
        Console.Write("\nВведите имя для удаления: ");
        string nameToRemove = Console.ReadLine();

        var birthdayToRemove = birthdays.FirstOrDefault(b => b.Name.Equals(nameToRemove, StringComparison.OrdinalIgnoreCase));

        if (birthdayToRemove != null)
        {
            birthdays.Remove(birthdayToRemove);
            Console.WriteLine($"День рождения для {nameToRemove} успешно удален.");
        }
        else
        {
            Console.WriteLine($"День рождения для {nameToRemove} не найден.");
        }
    }


    static void EditBirthday()
    {
        ShowAllBirthdays();
        Console.Write("\nВведите имя дня рождения для редактирования: ");
        string nameToEdit = Console.ReadLine();

        var birthdayToEdit = birthdays.FirstOrDefault(b => b.Name.Equals(nameToEdit, StringComparison.OrdinalIgnoreCase));

        if (birthdayToEdit != null)
        {
            Console.Write("Введите новое имя: ");
            string newName = Console.ReadLine();

            Console.Write("Введите новую дату рождения (в формате ДД.ММ.ГГГГ): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDateOfBirth))
            {
                birthdayToEdit.Name = newName;
                birthdayToEdit.DateOfBirth = newDateOfBirth;
                Console.WriteLine($"День рождения для {nameToEdit} успешно отредактирован.");
            }
            else
            {
                Console.WriteLine("Неверный формат даты. Редактирование невозможно.");
            }
        }
        else
        {
            Console.WriteLine($"День рождения для {nameToEdit} не найден.");
        }
    }


    static void DisplayBirthdays(List<Birthday> birthdayList, string title)
    {
        Console.WriteLine(title);
        if (birthdayList.Count == 0)
        {
            Console.WriteLine("Список пуст.");
        }
        else
        {
            for (int i = 0; i < birthdayList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {birthdayList[i].Name} - {birthdayList[i].DateOfBirth.ToShortDateString()}");
            }
        }
    }

    static void LoadDataFromFile()
    {
        try
        {
            if (File.Exists(dataFilePath))
            {
                string[] lines = File.ReadAllLines(dataFilePath);

                foreach (var line in lines)
                {
                    string[] parts = line.Split('|');

                    if (parts.Length == 2)
                    {
                        string name = parts[0];
                        if (DateTime.TryParseExact(parts[1], "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                        {
                            birthdays.Add(new Birthday { Name = name, DateOfBirth = dateOfBirth });
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка при чтении даты рождения для {name}. Пропускаем эту запись.");
                        }
                    }

                    else
                    {
                        Console.WriteLine($"Ошибка при разборе строки: {line}. Пропускаем эту запись.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Файл с данными не найден. Создан новый файл.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных из файла: {ex.Message}");
        }
    }


    static void SaveDataToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(dataFilePath))
            {
                foreach (var birthday in birthdays)
                {
                    writer.WriteLine($"{birthday.Name}|{birthday.DateOfBirth.ToString("dd.MM.yyyy")}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении данных в файл: {ex.Message}");
        }
    }
}

class Birthday
{
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
}

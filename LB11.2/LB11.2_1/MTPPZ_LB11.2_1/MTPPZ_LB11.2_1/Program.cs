using System;

namespace Lab11_2_Proxy
{

    public interface IMedicalRecord
    {
        void Display();
    }

    public class RealMedicalRecord : IMedicalRecord
    {
        private readonly string _patientName;
        private readonly int _age;
        private readonly string _diagnosis;
        private readonly string _prescriptions;
        private readonly string _labResults;

        public RealMedicalRecord(
            string patientName,
            int age,
            string diagnosis,
            string prescriptions,
            string labResults)
        {
            _patientName = patientName;
            _age = age;
            _diagnosis = diagnosis;
            _prescriptions = prescriptions;
            _labResults = labResults;
        }

        public void Display()
        {
            Console.WriteLine($"[ПОВНА МЕДИЧНА КАРТКА]");
            Console.WriteLine($"  Пацієнт: {_patientName}, Вік: {_age} років");
            Console.WriteLine($"  Діагноз: {_diagnosis}");
            Console.WriteLine($"  Призначення: {_prescriptions}");
            Console.WriteLine($"  Аналізи: {_labResults}");
        }

        public void DisplayGeneralInfo()
        {
            Console.WriteLine($"[ЗАГАЛЬНА ІНФОРМАЦІЯ КАРТКИ]");
            Console.WriteLine($"  Пацієнт: {_patientName}, Вік: {_age} років");
            Console.WriteLine($"  (Доступ до діагнозу та результатів аналізів обмежено політикою безпеки)");
        }
    }

    public class MedicalRecordProxy : IMedicalRecord
    {
        private readonly RealMedicalRecord _realRecord;
        private readonly string _userRole;

        public MedicalRecordProxy(RealMedicalRecord realRecord, string userRole)
        {
            _realRecord = realRecord;
            _userRole = userRole;
        }

        public void Display()
        {
            Console.WriteLine($"--- Запит доступу від користувача з роллю: '{_userRole}' ---");

            if (_userRole.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
            {
                _realRecord.Display();
            }
            else if (_userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                _realRecord.DisplayGeneralInfo();
            }
            else
            {
                Console.WriteLine("Помилка доступу: недостатньо прав для перегляду медичної картки.");
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var patientRecord = new RealMedicalRecord(
                patientName: "Ковальчук Олександр Петрович",
                age: 34,
                diagnosis: "Гострий бронхіт",
                prescriptions: "Інгаляції, вітамін C, рясне пиття",
                labResults: "Загальний аналіз крові: лейкоцити 11.2×10⁹/л"
            );

            IMedicalRecord doctorView = new MedicalRecordProxy(patientRecord, "Doctor");
            doctorView.Display();

            IMedicalRecord adminView = new MedicalRecordProxy(patientRecord, "Admin");
            adminView.Display();
        }
    }
}
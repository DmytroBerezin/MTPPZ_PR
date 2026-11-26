using System;
using System.Collections.Generic;

namespace CompanyBehavioralPatternsModule
{

    public class PurchaseRequest
    {
        public string Title { get; }
        public decimal Amount { get; }

        public PurchaseRequest(string title, decimal amount)
        {
            Title = title;
            Amount = amount;
        }
    }

    public abstract class ApprovalHandler
    {
        protected ApprovalHandler? _nextHandler;

        public ApprovalHandler SetNext(ApprovalHandler nextHandler)
        {
            _nextHandler = nextHandler;
            return nextHandler;
        }

        public abstract void Handle(PurchaseRequest request);
    }

    public class TeamLeadHandler : ApprovalHandler
    {
        public override void Handle(PurchaseRequest request)
        {
            if (request.Amount <= 5000m)
            {
                Console.WriteLine($"[TeamLead] Заявку '{request.Title}' ({request.Amount:F2} грн) схвалено на рівні тімліда.");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"[TeamLead] Сума {request.Amount:F2} грн перевищує ліміт -> передача керівнику відділу...");
                _nextHandler.Handle(request);
            }
        }
    }

    public class DepartmentHeadHandler : ApprovalHandler
    {
        public override void Handle(PurchaseRequest request)
        {
            if (request.Amount <= 25000m)
            {
                Console.WriteLine($"[DepartmentHead] Заявку '{request.Title}' ({request.Amount:F2} грн) схвалено керівником відділу.");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"[DepartmentHead] Сума {request.Amount:F2} грн перевищує ліміт -> передача директору...");
                _nextHandler.Handle(request);
            }
        }
    }

    public class DirectorHandler : ApprovalHandler
    {
        public override void Handle(PurchaseRequest request)
        {
            if (request.Amount <= 100000m)
            {
                Console.WriteLine($"[Director] Заявку '{request.Title}' ({request.Amount:F2} грн) затверджено генеральним директором.");
            }
            else
            {
                Console.WriteLine($"[Director] Заявку '{request.Title}' ({request.Amount:F2} грн) відхилено: потрібне рішення ради акціонерів.");
            }
        }
    }


    public interface ICompanyVisitor
    {
        void VisitDeveloper(DeveloperEmployee developer);
        void VisitManager(ManagerEmployee manager);
    }

    public interface ICompanyElement
    {
        void Accept(ICompanyVisitor visitor);
    }

    public class DeveloperEmployee : ICompanyElement
    {
        public string Name { get; }
        public decimal BaseSalary { get; }
        public int OvertimeHours { get; }

        public DeveloperEmployee(string name, decimal baseSalary, int overtimeHours)
        {
            Name = name;
            BaseSalary = baseSalary;
            OvertimeHours = overtimeHours;
        }

        public void Accept(ICompanyVisitor visitor) => visitor.VisitDeveloper(this);
    }

    public class ManagerEmployee : ICompanyElement
    {
        public string Name { get; }
        public decimal BaseSalary { get; }
        public decimal TeamBonus { get; }

        public ManagerEmployee(string name, decimal baseSalary, decimal teamBonus)
        {
            Name = name;
            BaseSalary = baseSalary;
            TeamBonus = teamBonus;
        }

        public void Accept(ICompanyVisitor visitor) => visitor.VisitManager(this);
    }

    public class PayrollAuditVisitor : ICompanyVisitor
    {
        public decimal TotalCompanyPayroll { get; private set; }

        public void VisitDeveloper(DeveloperEmployee dev)
        {
            decimal overtimePay = dev.OvertimeHours * 500m;
            decimal payout = dev.BaseSalary + overtimePay;
            TotalCompanyPayroll += payout;
            Console.WriteLine($"[Бухгалтерія | Розробник] {dev.Name}: оклад {dev.BaseSalary:F2} + понаднормові {overtimePay:F2} = {payout:F2} грн");
        }

        public void VisitManager(ManagerEmployee mgr)
        {
            decimal payout = mgr.BaseSalary + mgr.TeamBonus;
            TotalCompanyPayroll += payout;
            Console.WriteLine($"[Бухгалтерія | Менеджер] {mgr.Name}: оклад {mgr.BaseSalary:F2} + бонус KPI {mgr.TeamBonus:F2} = {payout:F2} грн");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. УЗГОДЖЕННЯ ЗАЯВОК КОМПАНІЇ (CHAIN OF RESPONSIBILITY) ===");
            var teamLead = new TeamLeadHandler();
            var deptHead = new DepartmentHeadHandler();
            var director = new DirectorHandler();
            teamLead.SetNext(deptHead).SetNext(director);

            teamLead.Handle(new PurchaseRequest("Канцтовари та кава в офіс", 3200m));
            Console.WriteLine();
            teamLead.Handle(new PurchaseRequest("Монітор 4K для дизайнера", 18500m));
            Console.WriteLine();
            teamLead.Handle(new PurchaseRequest("Серверне обладнання", 75000m));

            Console.WriteLine("\n=== 2. АУДИТ ЗАРПЛАТНОГО ФОНДУ (VISITOR) ===");
            var staff = new List<ICompanyElement>
            {
                new DeveloperEmployee("Ігор Ткаченко", 45000m, 8),
                new DeveloperEmployee("Анна Мельник", 52000m, 4),
                new ManagerEmployee("Олег Сидоренко", 48000m, 12000m)
            };

            var payrollVisitor = new PayrollAuditVisitor();
            foreach (var employee in staff)
            {
                employee.Accept(payrollVisitor);
            }
            Console.WriteLine($"Загальний фонд виплат компанії: {payrollVisitor.TotalCompanyPayroll:F2} грн");
        }
    }
}
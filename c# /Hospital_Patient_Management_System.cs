using System;


public delegate double BillingStrategy(double amount);


public class HospitalEvents
{
    public event Action<string> OnNotification;

    public void Notify(string message)
    {
        if (OnNotification != null)
            OnNotification(message);
    }
}


public abstract class Patient
{
    public string Name;
    public int Age;

    public Patient(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public abstract double CalculateTreatmentCost();
}


public class OPDPatient : Patient
{
    public OPDPatient(string name, int age) : base(name, age) { }

    public override double CalculateTreatmentCost()
    {
        return 1500;
    }
}

public class IPDPatient : Patient
{
    public IPDPatient(string name, int age) : base(name, age) { }

    public override double CalculateTreatmentCost()
    {
        return 4000;
    }
}

public class EmergencyPatient : Patient
{
    public EmergencyPatient(string name, int age) : base(name, age) { }

    public override double CalculateTreatmentCost()
    {
        return 6000;
    }
}


public class BillingService
{
    public double ApplyBilling(double amount, BillingStrategy strategy)
    {
        return strategy(amount);
    }
}


class Program
{
    static void Main()
    {
        HospitalEvents hospitalEvents = new HospitalEvents();

        hospitalEvents.OnNotification += msg => Console.WriteLine("[Billing Dept] " + msg);
        hospitalEvents.OnNotification += msg => Console.WriteLine("[Doctor Dept] " + msg);
        hospitalEvents.OnNotification += msg => Console.WriteLine("[Admin Dept] " + msg);

        Console.WriteLine("=== Hospital Patient Management System ===\n");

        Console.Write("Enter Patient Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter Patient Age: ");
        int age = int.Parse(Console.ReadLine());

        Console.WriteLine("\nSelect Patient Type:");
        Console.WriteLine("1. OPD Patient");
        Console.WriteLine("2. IPD Patient");
        Console.WriteLine("3. Emergency Patient");
        Console.Write("Choice: ");
        int choice = int.Parse(Console.ReadLine());

        Patient patient;

        if (choice == 1)
            patient = new OPDPatient(name, age);
        else if (choice == 2)
            patient = new IPDPatient(name, age);
        else if (choice == 3)
            patient = new EmergencyPatient(name, age);
        else
        {
            Console.WriteLine("Invalid Choice");
            return;
        }

        double baseAmount = patient.CalculateTreatmentCost();

        BillingStrategy strategy;
        string ruleDescription;

        if (patient is IPDPatient)
        {
            strategy = amt => amt * 0.8;
            ruleDescription = "IPD Patient: 20% discount applied";
        }
        else if (patient is EmergencyPatient)
        {
            strategy = amt => amt + 1500;
            ruleDescription = "Emergency Patient: Rs.1500 surcharge applied";
        }
        else
        {
            strategy = amt => amt;
            ruleDescription = "OPD Patient: No extra charge";
        }

        BillingService billingService = new BillingService();
        double finalBill = billingService.ApplyBilling(baseAmount, strategy);


        Console.WriteLine("Patient Name: " + patient.Name);
        Console.WriteLine("Patient Type: " + patient.GetType().Name);
        Console.WriteLine("Base Treatment Cost: Rs." + baseAmount);
        Console.WriteLine("Billing Rule Applied: " + ruleDescription);
        Console.WriteLine("Final Payable Amount: Rs." + finalBill);

        hospitalEvents.Notify("Patient " + patient.Name + " bill generated: Rs." + finalBill);

        Console.WriteLine("\nProcess Completed Successfully.");
    }
}

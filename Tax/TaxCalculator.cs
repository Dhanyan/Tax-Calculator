using System;
using System.Collections.Generic;
using System.Linq;


namespace TaxCalculatorInterviewTests
{
    /// <summary>
    /// This is the public inteface used by our client and may not be changed
    /// </summary>
    public interface ITaxCalculator
    {
        double GetStandardTaxRate(Commodity commodity);
        void SetCustomTaxRate(Commodity commodity, double rate);
        double GetTaxRateForDateTime(Commodity commodity, DateTime date);
        double GetCurrentTaxRate(Commodity commodity);
    }

    /// <summary>
    /// Implements a tax calculator for our client.
    /// The calculator has a set of standard tax rates that are hard-coded in the class.
    /// It also allows our client to remotely set new, custom tax rates.
    /// Finally, it allows the fetching of tax rate information for a specific commodity and point in time.

    /// </summary>
    public class TaxCalculator : ITaxCalculator
    {

        public class Program
        {


            public static void Main(string[] args)
            {
                TaxCalculator taxCalculator = new TaxCalculator();
                bool choice = true;

                while (choice)
                {
                    Console.WriteLine("1.GetStandardTaxRate 2.SetCustomTaxRate 3.GetTaxRateForDateTime 4.GetCurrentTaxRate 5.Exit");
                    Console.WriteLine("Enter your Choice: ");
                    int option = int.Parse(Console.ReadLine());
                    ValidateOption(Convert.ToInt32(option));
                    if (option == 1)
                    {
                        CommandLine();
                        string comm = Console.ReadLine();
                        Commodity com = (Commodity)Convert.ToInt32(comm);
                        bool result = ValidateCommodity(Convert.ToInt32(comm));
                        if (result == true)
                        {
                            Console.WriteLine("Taxrate of {0} is {1}", com, taxCalculator.GetStandardTaxRate(com));
                        }
                        else
                        {
                            Console.WriteLine("Enter commodity from the List");

                        }
                    }
                    else if (option == 2)
                    {
                        CommandLine();
                        string comm = Console.ReadLine();
                        bool result = ValidateCommodity(Convert.ToInt32(comm));
                        if (result == true)
                        {
                            Console.WriteLine("Enter Custom TaxRate:");

                            string custRate = Console.ReadLine();
                            double custTax = Convert.ToDouble(custRate);
                            Commodity com = (Commodity)Convert.ToInt32(comm);
                            taxCalculator.SetCustomTaxRate(com, custTax);
                            Console.WriteLine("Customised Tax rate of {0} is {1}", com, custTax);
                        }
                        else
                        {
                            Console.WriteLine("Enter commodity from the List");

                        }
                    }
                    else if (option == 3)
                    {
                        CommandLine();
                        string comm = Console.ReadLine();
                        bool result = ValidateCommodity(Convert.ToInt32(comm));
                        if (result == true)
                        {
                            Console.WriteLine("Enter the date(yyyy-MM-dd HH:mm:ss):");
                            DateTime newdate = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd h:mm:ss tt", null);
                            Commodity com = (Commodity)Convert.ToInt32(comm);
                            Console.WriteLine("Tax rate of {0} is {1}", com, taxCalculator.GetTaxRateForDateTime(com, newdate));
                        }
                        else
                        {
                            Console.WriteLine("Enter commodity from the List");

                        }
                    }
                    else if (option == 4)
                    {
                        CommandLine();
                        string comm = Console.ReadLine();
                        bool result = ValidateCommodity(Convert.ToInt32(comm));
                        if (result == true)
                        {
                            Commodity com = (Commodity)Convert.ToInt32(comm);
                            Console.WriteLine(" Current Taxrate of {0} is {1}", com, taxCalculator.GetCurrentTaxRate(com));
                        }
                        else
                        {
                            Console.WriteLine("Enter commodity from the List");

                        }
                    }
                    else if (option == 5)
                    {
                        Environment.Exit(0);
                    }
                }

            }

        }
        //---------------------------------VALIDATION-----------------------------------------------------------------------
        /// <summary>
        /// To Vaidate ifthe option entered is from the List
        /// </summary>
        /// <param name="Option"></param>
        static void ValidateOption(int Option)
        {
            if (Option < 1 || Option > 5)

                Console.WriteLine("Enter Option from the List");

        }
        /// <summary>
        /// To Vaidate the commodity entered is in the List
        /// </summary>
        /// <param name="commodity"></param>
        static bool ValidateCommodity(int commodity)
        {

            if (commodity < 0 || commodity > 6)
            {
                return false;
            }
            return true;

        }
        //--------------------------------------------------COMMAND------------------------------------------------------------------
        /// <summary>
        /// To enter choice
        /// </summary>
        static void CommandLine()
        {
            Console.WriteLine("0.Default 1.Alcohol 2.Food 3.FoodServices 4.Literature 5.Transport 6.CulturalServices");
            Console.WriteLine("Enter your Choice: ");
        }

        //----------------------------------------CALCULATE TAX-----------------------------------------------------------------------
        /// <summary>
        /// Standard rate acoording to each commodity
        /// </summary>
        /// <param name="commodity"></param>
        /// <returns></returns>
        public double GetStandardTaxRate(Commodity commodity)
        {
            if (commodity == Commodity.Default || commodity == Commodity.Alcohol)
                return 0.25;

            if (commodity == Commodity.Food || commodity == Commodity.FoodServices)
                return 0.12;
            else
                return 0.6;

        }

        /// <summary>
        /// this will fixa customised rate for commodity
        /// </summary>
        /// <param name="commodity"></param>
        /// <param name="rate"></param>
        public void SetCustomTaxRate(Commodity commodity, double rate)
        {
            DateTime today = DateTime.Now;
            today = DateTime.ParseExact(today.ToString("yyyy-MM-dd HH:mm:ss"), ("yyyy-MM-dd HH:mm:ss"), null);
            Console.WriteLine(today);
            var timeTuple = Tuple.Create(today, commodity);
            if (_customRates.ContainsKey(timeTuple))
                _customRates[timeTuple] = rate;
            else
                _customRates.Add(timeTuple, rate);

        }
        static Dictionary<Tuple<DateTime, Commodity>, double> _customRates = new Dictionary<Tuple<DateTime, Commodity>, double>();


        /// <summary>
        /// this will return tax rate for commodity in the specific date
        /// </summary>
        /// <param name="commodity"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public double GetTaxRateForDateTime(Commodity commodity, DateTime date)
        {
            var timeTuple = Tuple.Create(date, commodity);
            var com = _customRates.Select(s => s.Key).Where(d => d.Item2 == commodity);
            //check if there is no custom rates for the entered commodity or if date entered is in future to the customised date
            //then calculate standard tax rate
            if (com.Count() == 0 || date < _customRates.Keys.First().Item1)
                return GetStandardTaxRate(commodity);
            //if the commodity entered is in the customrates then return that value
            else if (_customRates.ContainsKey(timeTuple))
                return _customRates[timeTuple];
            //if date entered is in future then return the Latest value
            else if (date > com.Last().Item1)
                return _customRates[_customRates.Keys.Last()];
            else
            {
                var prevRecord = com.First();
                foreach (var record in com)
                {
                    if (record.Item1 > date)
                        return _customRates[prevRecord];
                    prevRecord = record;
                }
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// this will return latest rate of commodity
        /// </summary>
        /// <param name="commodity"></param>
        /// <returns></returns>
        public double GetCurrentTaxRate(Commodity commodity)
        {
            var com = _customRates.Select(s => s.Key).Where(d => d.Item2 == commodity);
            if (com.Count() == 0)
                return GetStandardTaxRate(commodity);
            else
                return _customRates[_customRates.Keys.Last()];

        }

    }

    public enum Commodity
    {
        //PLEASE NOTE: THESE ARE THE ACTUAL TAX RATES THAT SHOULD APPLY, WE JUST GOT THEM FROM THE CLIENT!
        Default,            //25%
        Alcohol,            //25%
        Food,               //12%
        FoodServices,       //12%
        Literature,         //6%
        Transport,          //6%
        CulturalServices    //6%
    }

}

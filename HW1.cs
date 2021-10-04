using System;
//For math
using System.Linq;
using System.Collections.Generic;
using stats = MathNet.Numerics.Statistics.ArrayStatistics;
//For formatting numbers
using System.Text.RegularExpressions;
//For scraping data from website
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace ISYS4363HW1
{
    class HW1
    {
        //For calc of STDev later
        static double standardDeviation(IEnumerable<double> sequence)
        {
            double result = 0;

            if (sequence.Any())
            {
                double average = sequence.Average();
                double sum = sequence.Sum(d => Math.Pow(d - average, 2));
                result = Math.Sqrt((sum) / (sequence.Count() - 1));
            }
            return result;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning ...");
            Console.WriteLine();

            //Using ChromeDriver for webscraping
            IWebDriver driver = new ChromeDriver("C:/temp");
            driver.Url = "https://www.the-numbers.com/market/";

            Console.WriteLine("Title: " + driver.Title);
            Console.WriteLine("URL: " + driver.Url);
            //Console.WriteLine(driver.PageSource);

            //Declaring arrays
            int[] year = new int[30];
            int[] tickets = new int[30];
            long[] boxoffice = new long[30];
            long[] inflated = new long[30];
            double[] avgprice = new double[30];

            //Rows and Columns variables for loop
            int r = 0;
            int c = 0;

            //More pulling of data from website
            string xp = "/ html / body / div / div[5] / div[1] / center[2] / table";
            IWebElement table = driver.FindElement(By.XPath(xp));
            var rows = table.FindElements(By.TagName("tr"));
            string FFrow = "";

            //Loop to create arrays for calculations
            foreach (var row in rows)
            {
                r = r + 1;
                var tds = row.FindElements(By.TagName("td"));
                foreach (var entry in tds)
                {
                    c = c + 1;
                    if (c == 1)
                    {
                        year[r] = Int32.Parse(entry.Text);
                    }
                    if (c == 2)
                    {
                        string comma = Regex.Match(entry.Text, @"^[\d,]+").Value.Replace(",", String.Empty);
                        tickets[r] = Int32.Parse(comma);
                    }
                    if (c == 3)
                    {
                        string dollar = entry.Text.TrimStart(' ', '$');
                        string comma = Regex.Match(dollar, @"^[\d,]+").Value.Replace(",", String.Empty);
                        boxoffice[r] = long.Parse(comma);
                    }
                    if (c == 4)
                    {
                        string dollar = entry.Text.TrimStart(' ', '$');
                        string comma = Regex.Match(dollar, @"^[\d,]+").Value.Replace(",", String.Empty);
                        inflated[r] = long.Parse(comma);
                    }
                    if (c == 5)
                    {
                        string dollar = entry.Text.TrimStart(' ', '$');
                        avgprice[r] = double.Parse(dollar);
                    }
                    FFrow = FFrow + entry.Text + "  ";
                    //int result = Int32.Parse(entry.Text);
                    //Console.Write(entry.Text + "\t");
                }
                c = 0;
                Console.WriteLine(FFrow);
                FFrow = "";
                Console.WriteLine();
            }
            driver.Quit();

            //Tickets calculation
            double tixAverage = tickets.Average();
            double tixsumOfSquaresOfDifferences = tickets.Select(val => (val - tixAverage) * (val - tixAverage)).Sum();
            double sdTix = Math.Sqrt(tixsumOfSquaresOfDifferences / tickets.Length);
            Console.WriteLine("SD of Tickets for All Years: {0}", sdTix);

            //Box office calculation
            double BOaverage = boxoffice.Average();
            double BOsumOfSquaresOfDifferences = boxoffice.Select(val => (val - BOaverage) * (val - BOaverage)).Sum();
            double sdBO = Math.Sqrt(BOsumOfSquaresOfDifferences / boxoffice.Length);
            Console.WriteLine("SD of Box Office for All Years: ${0}", sdBO);

            //Inflated calculation
            double infAverage = inflated.Average();
            double infsumOfSquaresOfDifferences = inflated.Select(val => (val - infAverage) * (val - infAverage)).Sum();
            double sdInf = Math.Sqrt(infsumOfSquaresOfDifferences / inflated.Length);
            Console.WriteLine("SD of Inflated Box Office for All Years: ${0}", sdInf);

            //Average price calculation
            double priceAverage = avgprice.Average();
            double pricesumOfSquaresOfDifferences = avgprice.Select(val => (val - priceAverage) * (val - priceAverage)).Sum();
            double sdPrice = Math.Sqrt(pricesumOfSquaresOfDifferences / avgprice.Length);
            Console.WriteLine("SD of Average Ticket Price for All Years: ${0}", sdPrice);
            Console.WriteLine();

            //2021 Example
            double y2021TixDiff = tickets[2] - sdTix;
            Console.WriteLine("2021 Test Example");
            Console.WriteLine("Difference between 2021 tickets sold and standard deviation: {0} - {1}", tickets[2], sdTix);
            Console.WriteLine();
            Console.WriteLine("Testing for positive or negative number...");
            if (y2021TixDiff > 1)
            {
                Console.WriteLine("Difference is positive! Result = {0}", y2021TixDiff);
            }
            else
            {
                Console.WriteLine("Difference is negative. Result = {0}", y2021TixDiff);
            }
            Console.WriteLine();
            Console.WriteLine("Calculating # of Standard Deviations...");
            double numStdDev = sdTix / tickets[2];
            Console.WriteLine("Number of SD = {0}", numStdDev);
            Console.WriteLine();
            Console.WriteLine("Year is after 2016 - Multiply by .2");
            double Adjustment2021 = numStdDev * .2;
            Console.WriteLine();
            Console.WriteLine("Adding average ticket price of 2021");
            double FinalNumber2021 = Adjustment2021 + avgprice[2];
            Console.WriteLine("Final Index Number = {0}", FinalNumber2021);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            //Shang Chi Example
            int tixSold2021 = 358660716;
            double avgPrice2021 = 9.16;
            int SCBO = 197151684;
            double SCTixSoldEst = SCBO / avgPrice2021;
            
            List<double> intList = new List<double> { SCTixSoldEst, tixSold2021 };
            double SCstandard_deviation = standardDeviation(intList);
            Console.WriteLine("Standard Deviation of Shang Chi = {0}", SCstandard_deviation);
            double SCStdDevDiff = SCstandard_deviation / SCTixSoldEst;
            Console.WriteLine("# of Standard Deviations Below Mean = {0}", SCStdDevDiff);
            double DomesticAdjustment = (-.4);
            double PostAdjustment = SCStdDevDiff * DomesticAdjustment;
            Console.WriteLine("After Domestic Adjustment = {0}", PostAdjustment);
            double FinalNumber = PostAdjustment + avgPrice2021;
            Console.WriteLine("Final Index Value = {0}", FinalNumber);

            Console.Write("Press any key to end ...");
            Console.ReadLine();
        }
    }
}

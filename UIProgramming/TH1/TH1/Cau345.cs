using System;
using System.Collections.Generic;
using System.Text;

namespace TH1
{
    class Date
    {
        private int day = 0;
        private int month = 0;
        private int year = 0;
        public void Input()
        {
            Console.WriteLine("===Cau 3+4+5===");
            Console.WriteLine("Enter day: ");
            this.day = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter month: ");
            this.month = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter year: ");
            this.year = Convert.ToInt32(Console.ReadLine());
        }
        private bool CheckLeapYear()
        {
            if ((year % 100 == 0 && year % 4 == 0) || year % 400 == 0) return true;
            return false;
        }
        private bool CheckDate()
        {
            if (year < 0 || month < 0 || month > 12 || day > 31 || day < 0) return false;
            switch (month)
            {

                case 4:
                case 6:
                case 9:
                case 11:
                    if (day >= 31) return false;
                    else return false;
                case 2:
                    if (CheckLeapYear() == true && day <= 29 || CheckLeapYear() == false && day <= 28) return true;
                    else return false;
                default:
                    return true;
            }
        }
        private void DateOfWeek()
        {
        }
        public override string ToString()
        {
            DateTime date = new DateTime(this.year, this.month, this.day);
            if (CheckDate()) return this.day + "/" + this.month + "/" + this.year + " is standard\nThis month has" + PrintNumberOfDay() + " days\n" + date.DayOfWeek;
            return this.day + "/" + this.month + "/" + this.year + " is non-standard\nThis month has " + PrintNumberOfDay() + "days\n" + date.DayOfWeek;
        }
        private int PrintNumberOfDay()
        {
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) return 31;
            else if (month == 12)
                if (CheckLeapYear()) return 29;
                else return 28;
            else return 30;
        }
    }
}

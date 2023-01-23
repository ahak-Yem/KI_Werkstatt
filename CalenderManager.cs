
namespace BookingPlatform
{
    public enum Months
    {
        Jan = 1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sept, Oct, Nov, Dec
    }
    public class CalenderManager
    {
        public readonly Months ChoosenMonth;
        public readonly int ChoosenYear;
        public readonly DateTime ChoosenDate;
        Dictionary<int, string> DateAndDay = new Dictionary<int, string>();

        public CalenderManager(int year, int month, int day)
        {
            ChoosenYear = year;
            ChoosenMonth = (Months)month;
            ChoosenDate = new DateTime(year, month, 1);
        }
        public DateTime[] GetChoosenDays(IFormCollection formCollection)
        {
            DateTime[] days = new DateTime[2];
            int day1 = 0, day2 = 0;
            int temp = 1;
            for (int possibleKeys = 1; possibleKeys <= 31; possibleKeys++)
            {
                if (!string.IsNullOrEmpty(formCollection[$"{possibleKeys}"]))
                {
                    day1 = int.Parse(formCollection[$"{possibleKeys}"]);
                    temp = possibleKeys + 1;
                    break;
                }
            }
            for (int possibleKeys = temp; possibleKeys <= 31; possibleKeys++)
            {
                if (!string.IsNullOrEmpty(formCollection[$"{possibleKeys}"]))
                {
                    day2 = int.Parse(formCollection[$"{possibleKeys}"]);
                    break;
                }
            }
            if (day2 != 0)
            {
                days[0] = new DateTime(this.ChoosenYear, (int)this.ChoosenMonth, day1);
                days[1] = new DateTime(this.ChoosenYear, (int)this.ChoosenMonth, day2);
            }
            else
            {
                days[0] = new DateTime(0001, 1, 1);                                                                                                                                           
                days[1] = new DateTime(0001, 1, 1);
            }
            return days;
        }
        public Dictionary<int, string> SetDaysOfCurrentMonth()
        {
            if (ChoosenMonth == Months.Jan || ChoosenMonth == Months.Mar || ChoosenMonth == Months.May || ChoosenMonth == Months.Jul || ChoosenMonth == Months.Aug || ChoosenMonth == Months.Oct || ChoosenMonth == Months.Dec)
            {
                for (int daysIdx = 1; daysIdx <= 31; daysIdx++)
                {
                    string dayName = new DateTime(ChoosenYear, (int)ChoosenMonth, daysIdx).DayOfWeek.ToString();
                    DateAndDay.Add(daysIdx, dayName);
                }
            }
            else if (ChoosenMonth == Months.Apr || ChoosenMonth == Months.Jun || ChoosenMonth == Months.Sept || ChoosenMonth == Months.Nov)
            {
                for (int daysIdx = 1; daysIdx <= 30; daysIdx++)
                {
                    string dayName = new DateTime(ChoosenYear, (int)ChoosenMonth, daysIdx).DayOfWeek.ToString();
                    DateAndDay.Add(daysIdx, dayName);
                }
            }
            else if (ChoosenMonth == Months.Feb && DateTime.IsLeapYear(ChoosenYear))
            {
                for (int daysIdx = 1; daysIdx <= 29; daysIdx++)
                {
                    string dayName = new DateTime(ChoosenYear, (int)ChoosenMonth, daysIdx).DayOfWeek.ToString();
                    DateAndDay.Add(daysIdx, dayName);
                }
            }
            else if (ChoosenMonth == Months.Feb && !DateTime.IsLeapYear(ChoosenYear))
            {
                for (int daysIdx = 1; daysIdx <= 28; daysIdx++)
                {
                    string dayName = new DateTime(ChoosenYear, (int)ChoosenMonth, daysIdx).DayOfWeek.ToString();
                    DateAndDay.Add(daysIdx, dayName);
                }
            }
            return DateAndDay;
        }
    }
}
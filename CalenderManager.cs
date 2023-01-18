
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
            ChoosenDate = new DateTime(year, month,1);
        }

        public Dictionary<int, string> SetDaysOfCurrentMonth()
        {
            if(ChoosenMonth==Months.Jan || ChoosenMonth == Months.Mar || ChoosenMonth ==Months.May|| ChoosenMonth == Months.Jul || ChoosenMonth == Months.Aug || ChoosenMonth == Months.Oct || ChoosenMonth == Months.Dec)
            {
                for (int daysIdx = 1; daysIdx <= 31; daysIdx++)
                {
                    string dayName = new DateTime(ChoosenYear,(int)ChoosenMonth,daysIdx).DayOfWeek.ToString();
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
            else if(ChoosenMonth==Months.Feb && DateTime.IsLeapYear(ChoosenYear))
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
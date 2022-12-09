namespace BookingPlatform
{
    //Singleton Pattern to save the logged in Admin
    public sealed class CurrentAdmin
    {
        private string AdminID { get; set; }
        private static CurrentAdmin instance = null;
        private static readonly object padlock = new object();

        CurrentAdmin()
        {
        }
        public void SetAdminID(string adminID)
        {
            if (AdminID == null)
                AdminID = adminID;
        }
        public string GetAdminID()
        {
            return AdminID;
        }
        
        public static CurrentAdmin Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CurrentAdmin();
                    }
                    return instance;
                }
            }
        }
    }
}

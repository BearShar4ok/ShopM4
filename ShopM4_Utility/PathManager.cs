namespace ShopM4_Utility
{
    public static class PathManager
    {
        public const string ImageProductPath = @"\images\product\";//{ get; set; }

        public const string SessionCart = "SessionCart";//{ get; set; }
        public const string SessionQuery = "SessionQuery";//{ get; set; }

        public const string AdminRole = "Admin";//{ get; set; }
        public const string CustomerRole = "Customer";//{ get; set; }

        public const string EmailSender = "vasya.bershak@gmail.com";//{ get; set; }

        public const string NameCategory = "Category";
        public const string NameMyModel = "MyModel";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";//ожидание
        public const string StatusAccepted = "Accepted";
        public const string StatusInProcess = "InProcess";
        public const string StatusOrderDone = "OrderDone";
        public const string StatusDenied = "Denied";

        public static IEnumerable<string> StatusList =
            new List<string>() { StatusPending, StatusAccepted, StatusInProcess, StatusDenied, StatusOrderDone };
    }
}

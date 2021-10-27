using System;
using System.Linq;

namespace Incapsulation.EnterpriseTask
{
    public class Enterprise
    {
        public readonly Guid Guid;
        public Enterprise(Guid guid)
        {
            Guid = guid;
        }

        public string Name { get; set; }

        private string inn;
        public string Inn
        {
            get { return inn; }
            set
            {
                if (inn.Length != 10 || !inn.All(z => char.IsDigit(z)))
                    throw new ArgumentException();
                inn = value;
            }
        }

        public DateTime EstablishDate { get; set; }
        public TimeSpan ActiveTimeSpan => DateTime.Now - EstablishDate;

        public double GetTotalTransactionsAmount()
        {
            DataBase.OpenConnection();
            var amount = 0.0;
            foreach (Transaction t in DataBase.Transactions().Where(z => z.EnterpriseGuid == Guid))
                amount += t.Amount;
            return amount;
        }
    }
}

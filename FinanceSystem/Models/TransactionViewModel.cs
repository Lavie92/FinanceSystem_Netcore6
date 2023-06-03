namespace FinanceSystem.Models
{
    public class TransactionViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}

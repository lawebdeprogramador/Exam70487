using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Xunit;

namespace Exam70487._1._1._1___Choose_data_access_technologies__EF_
{
    public class EF
    {


        [Theory]
        [InlineData(3)]
        public static void GetCustomerById(int customerId)
        {
            TestEntities database = new TestEntities();

            Customer result = database.Customers.SingleOrDefault(cust => cust.CustomerId == customerId);

            //Expected a value. Null here indicates no record with this ID.
            Assert.NotNull(result);
            //Uh oh!
            Assert.Equal(customerId,result.CustomerId);
        }

        [Theory]
        [InlineData(3)]
        public static void GetCustomerByIdOnObjectContext(int customerId)
        {
            TestEntities database = new TestEntities();
            ObjectContext context = ConvertContext(database);

            ObjectSet<Customer> customers = context.CreateObjectSet<Customer>("Customers");
            Customer result = customers.SingleOrDefault(cust => cust.CustomerId == customerId);

            //Expected a value. Null here indicates no record with this ID.
            Assert.NotNull(result);
            //Uh oh!
            Assert.Equal(customerId, result.CustomerId);
        }

        [Theory]
        [InlineData(true, 2)]
        [InlineData(false, 1)]
        public static void GetAccountsByAliasName(bool useCSharpNullBehavior, int recordsToFind)
        {
            TestEntities database = new TestEntities();
            ObjectContext context = ConvertContext(database);
            ObjectSet<Account> accounts = context.CreateObjectSet<Account>("Accounts");

            context.ContextOptions.UseCSharpNullComparisonBehavior = useCSharpNullBehavior;
            int result = accounts.Count(acc => acc.Name != "Home");
            
            //"Uh oh!"
            Assert.Equal(recordsToFind,result);
        }

        public partial class Customer
        {
            public Customer()
            {
                this.Transactions = new HashSet<Transaction>();
            }

            public int CustomerId { get; set; }
            public int AccountId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public virtual Account Account { get; set; }
            public virtual ICollection<Transaction> Transactions { get; set; }
        }

        public partial class Transaction
        {
            public int TransactionId { get; set; }
            public string Name { get; set; }
        }


        public partial class TransactionDetail
        {
            public int TransactionDetailId { get; set; }
            public string Name { get; set; }
        }

        public partial class Account
        {
            public int AccountId { get; set; }
            public string Name { get; set; }
        }


        public partial class TestEntities : DbContext
        {
            public TestEntities() : base("name=TestDB")
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                //throw new UnintentionalCodeFirstException();
            }

            public DbSet<Account> Accounts { get; set; }
            public DbSet<Customer> Customers { get; set; }
            public DbSet<Transaction> Transactions { get; set; }
            public DbSet<TransactionDetail> TransactionDetails { get; set; }
        }

        public static ObjectContext ConvertContext(DbContext db)
        {
            return ((IObjectContextAdapter)db).ObjectContext;
        }

        
    }
}

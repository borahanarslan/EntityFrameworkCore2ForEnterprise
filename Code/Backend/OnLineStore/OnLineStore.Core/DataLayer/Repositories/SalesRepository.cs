using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnLineStore.Core.DataLayer.Contracts;
using OnLineStore.Core.DataLayer.DataContracts;
using OnLineStore.Core.EntityLayer.Dbo;
using OnLineStore.Core.EntityLayer.Sales;

namespace OnLineStore.Core.DataLayer.Repositories
{
    public class SalesRepository : Repository, ISalesRepository
    {
        public SalesRepository(IUserInfo userInfo, OnLineStoreDbContext dbContext)
            : base(userInfo, dbContext)
        {
        }

        public IQueryable<Customer> GetCustomers()
            => DbContext.Customers;

        public async Task<Customer> GetCustomerAsync(Customer entity)
            => await DbContext.Customers.FirstOrDefaultAsync(item => item.CustomerID == entity.CustomerID);

        public IQueryable<OrderInfo> GetOrders(short? currencyID = null, int? customerID = null, int? employeeID = null, short? orderStatusID = null, Guid? paymentMethodID = null, int? shipperID = null)
        {
            var query = from order in DbContext.Orders
                        join currencyJoin in DbContext.Currencies on order.CurrencyID equals currencyJoin.CurrencyID into currencyTemp
                        from currency in currencyTemp.DefaultIfEmpty()
                        join customer in DbContext.Customers on order.CustomerID equals customer.CustomerID
                        join employeeJoin in DbContext.Employees on order.EmployeeID equals employeeJoin.EmployeeID into employeeTemp
                        from employee in employeeTemp.DefaultIfEmpty()
                        join orderStatus in DbContext.OrderStatuses on order.OrderStatusID equals orderStatus.OrderStatusID
                        join paymentMethodJoin in DbContext.PaymentMethods on order.PaymentMethodID equals paymentMethodJoin.PaymentMethodID into paymentMethodTemp
                        from paymentMethod in paymentMethodTemp.DefaultIfEmpty()
                        join shipperJoin in DbContext.Shippers on order.ShipperID equals shipperJoin.ShipperID into shipperTemp
                        from shipper in shipperTemp.DefaultIfEmpty()
                        select new OrderInfo
                        {
                            OrderID = order.OrderID,
                            OrderStatusID = order.OrderStatusID,
                            CustomerID = order.CustomerID,
                            EmployeeID = order.EmployeeID,
                            ShipperID = order.ShipperID,
                            OrderDate = order.OrderDate,
                            Total = order.Total,
                            CurrencyID = order.CurrencyID,
                            PaymentMethodID = order.PaymentMethodID,
                            Comments = order.Comments,
                            DetailsCount = order.DetailsCount,
                            ReferenceOrderID = order.ReferenceOrderID,
                            CreationUser = order.CreationUser,
                            CreationDateTime = order.CreationDateTime,
                            LastUpdateUser = order.LastUpdateUser,
                            LastUpdateDateTime = order.LastUpdateDateTime,
                            Timestamp = order.Timestamp,
                            CurrencyCurrencyName = currency == null ? string.Empty : currency.CurrencyName,
                            CurrencyCurrencySymbol = currency == null ? string.Empty : currency.CurrencySymbol,
                            CustomerCompanyName = customer == null ? string.Empty : customer.CompanyName,
                            CustomerContactName = customer == null ? string.Empty : customer.ContactName,
                            EmployeeFirstName = employee.FirstName,
                            EmployeeMiddleName = employee == null ? string.Empty : employee.MiddleName,
                            EmployeeLastName = employee.LastName,
                            EmployeeBirthDate = employee.BirthDate,
                            OrderStatusDescription = orderStatus.Description,
                            PaymentMethodPaymentMethodName = paymentMethod == null ? string.Empty : paymentMethod.PaymentMethodName,
                            PaymentMethodPaymentMethodDescription = paymentMethod == null ? string.Empty : paymentMethod.PaymentMethodDescription,
                            ShipperCompanyName = shipper == null ? string.Empty : shipper.CompanyName,
                            ShipperContactName = shipper == null ? string.Empty : shipper.ContactName,
                        };

            if (currencyID.HasValue)
                query = query.Where(item => item.CurrencyID == currencyID);

            if (customerID.HasValue)
                query = query.Where(item => item.CustomerID == customerID);

            if (employeeID.HasValue)
                query = query.Where(item => item.EmployeeID == employeeID);

            if (orderStatusID.HasValue)
                query = query.Where(item => item.OrderStatusID == orderStatusID);

            if (paymentMethodID.HasValue)
                query = query.Where(item => item.PaymentMethodID == paymentMethodID);

            if (shipperID.HasValue)
                query = query.Where(item => item.ShipperID == shipperID);

            return query;
        }

        public async Task<Order> GetOrderAsync(Order entity)
            => await DbContext.Orders.Include(p => p.OrderDetails).FirstOrDefaultAsync(item => item.OrderID == entity.OrderID);

        public async Task<OrderDetail> GetOrderDetailAsync(OrderDetail entity)
            => await DbContext.OrderDetails.FirstOrDefaultAsync(item => item.OrderDetailID == entity.OrderDetailID);

        public IQueryable<Shipper> GetShippers()
            => DbContext.Shippers;

        public async Task<Shipper> GetShipperAsync(Shipper entity)
            => await DbContext.Shippers.FirstOrDefaultAsync(item => item.ShipperID == entity.ShipperID);

        public IQueryable<OrderStatus> GetOrderStatus()
            => DbContext.OrderStatuses;

        public async Task<OrderStatus> GetOrderStatusAsync(OrderStatus entity)
            => await DbContext.OrderStatuses.FirstOrDefaultAsync(item => item.OrderStatusID == entity.OrderStatusID);

        public IQueryable<Currency> GetCurrencies()
            => DbContext.Currencies;

        public IQueryable<PaymentMethod> GetPaymentMethods()
            => DbContext.PaymentMethods;
    }
}

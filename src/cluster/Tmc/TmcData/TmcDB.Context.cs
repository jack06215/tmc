﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TmcData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class ICTDEntities : DbContext
    {
        public ICTDEntities()
            : base("name=ICTDEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<OrderList> OrderLists { get; set; }
    
        public virtual int AddNewOrder(Nullable<System.Guid> userID, Nullable<int> black, Nullable<int> blue, Nullable<int> red, Nullable<int> green, Nullable<int> white)
        {
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(System.Guid));
    
            var blackParameter = black.HasValue ?
                new ObjectParameter("Black", black) :
                new ObjectParameter("Black", typeof(int));
    
            var blueParameter = blue.HasValue ?
                new ObjectParameter("Blue", blue) :
                new ObjectParameter("Blue", typeof(int));
    
            var redParameter = red.HasValue ?
                new ObjectParameter("Red", red) :
                new ObjectParameter("Red", typeof(int));
    
            var greenParameter = green.HasValue ?
                new ObjectParameter("Green", green) :
                new ObjectParameter("Green", typeof(int));
    
            var whiteParameter = white.HasValue ?
                new ObjectParameter("White", white) :
                new ObjectParameter("White", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddNewOrder", userIDParameter, blackParameter, blueParameter, redParameter, greenParameter, whiteParameter);
        }
    
        public virtual int CompleteOrder(Nullable<int> orderID)
        {
            var orderIDParameter = orderID.HasValue ?
                new ObjectParameter("OrderID", orderID) :
                new ObjectParameter("OrderID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CompleteOrder", orderIDParameter);
        }
    
        public virtual int UpdateOrderStatus(Nullable<int> orderID, string orderStatus)
        {
            var orderIDParameter = orderID.HasValue ?
                new ObjectParameter("OrderID", orderID) :
                new ObjectParameter("OrderID", typeof(int));
    
            var orderStatusParameter = orderStatus != null ?
                new ObjectParameter("OrderStatus", orderStatus) :
                new ObjectParameter("OrderStatus", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateOrderStatus", orderIDParameter, orderStatusParameter);
        }
    
        public virtual int UpdateOrderStatusByID(Nullable<int> orderID, Nullable<int> orderStatus)
        {
            var orderIDParameter = orderID.HasValue ?
                new ObjectParameter("OrderID", orderID) :
                new ObjectParameter("OrderID", typeof(int));
    
            var orderStatusParameter = orderStatus.HasValue ?
                new ObjectParameter("OrderStatus", orderStatus) :
                new ObjectParameter("OrderStatus", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateOrderStatusByID", orderIDParameter, orderStatusParameter);
        }
    
        public virtual int UpdateProductProduced(Nullable<int> orderID, Nullable<int> productNumber)
        {
            var orderIDParameter = orderID.HasValue ?
                new ObjectParameter("OrderID", orderID) :
                new ObjectParameter("OrderID", typeof(int));
    
            var productNumberParameter = productNumber.HasValue ?
                new ObjectParameter("ProductNumber", productNumber) :
                new ObjectParameter("ProductNumber", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateProductProduced", orderIDParameter, productNumberParameter);
        }
    }
}

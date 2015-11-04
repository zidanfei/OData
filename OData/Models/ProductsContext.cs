using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OData.Models
{
    public class ProductsContext: DbContext
    {
        static ProductsContext current = new ProductsContext();

        public ProductsContext()
             : base("name=ProductsContext")
        {
        }

        public static DbContext DBContext
        {
            get
            {
                return current;
            }
        }


        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
    }

}
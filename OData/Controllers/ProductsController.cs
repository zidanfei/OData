using OData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace OData.Controllers
{
    public class ProductsController : ODataController
    {
        ProductsContext db = new ProductsContext();

        private bool ProductExists(int key)
        {
            return db.Products.Any(p => p.Id == key);
        }
        public async Task<IHttpActionResult> Post(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Created(product);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Product> product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await db.Products.FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            product.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var product = await db.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public SingleResult<Product> Get([FromODataUri] int key)
        {
            IQueryable<Product> query = db.Products.Where(p => p.Id == key);
            return SingleResult.Create(query);
        }

        ///// <summary>
        ///// 创建Product与Supplier的关系
        ///// 如果为Product.Supplier创建关系，使用PUT请求
        ///// 如果为Supplier.Products创建关系，使用POST请求
        ///// </summary>
        ///// <param name="key">Product的主键</param>
        ///// <param name="navigationProperty">Product的导航属性</param>
        ///// <param name="link"></param>
        ///// <returns></returns>
        //[AcceptVerbs("POST", "PUT")]
        //public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link)
        //{
        //    //现保证Product是存在的
        //    var product = db.Products.SingleOrDefault(p => p.Id == key);
        //    if (product == null)
        //        return NotFound();

        //    switch (navigationProperty)
        //    {
        //        case "Supplier":
        //            //获取Supplier的主键
        //            var supplierId = Helpers.GetKeyFromUri<int>(Request, link);
        //            var supplier = db.Suppliers.SingleOrDefault(s => s.Id == supplierId);
        //            if (supplier == null)
        //                return NotFound();
        //            product.Supplier = supplier;
        //            break;
        //        default:
        //            return StatusCode(HttpStatusCode.NotImplemented);
        //    }
        //    await db.SaveChangesAsync();
        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        [HttpGet]
        [EnableQuery]
        public IQueryable<Product> Get()
        {
            return db.Products;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

       
    }
}
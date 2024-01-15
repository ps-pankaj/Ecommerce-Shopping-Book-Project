using Ecommerce_Project1.DataAccess.Data;
using Ecommerce_Project1.DataAccess.Repository.IRepository;
using Ecommerce_Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Project1.DataAccess.Repository
{
    public class CategoryRepository:Repository<Category>,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context):base(context)
        {
                _context = context;
        }
    }
}

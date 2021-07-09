using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Buyer.Mvc.Models;
namespace Buyer.Data
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder modelbuilder)
		{
			modelbuilder.Entity<OrderUser>().HasNoKey();
			modelbuilder.Entity<ItemByCustomer>().HasNoKey();
			modelbuilder.Entity<ItemSize>().HasNoKey();
			modelbuilder.Entity<ItemColor>().HasNoKey();
			modelbuilder.Entity<ItemCategory>().HasNoKey();
			modelbuilder.Entity<ItemSeason>().HasNoKey();
			modelbuilder.Entity<ColorAndSizeByItem>().HasNoKey();
			modelbuilder.Entity<PriceCurrency>().HasNoKey();
			modelbuilder.Entity<CartDetail>().HasNoKey();
			modelbuilder.Entity<ItemCollection>().HasNoKey();
			modelbuilder.Entity<Division>().HasNoKey();

			//Order
			modelbuilder.Entity<OrderHeader>().HasNoKey();
			modelbuilder.Entity<OrderLine>().HasNoKey();
		}
		#region DbSet

		public DbSet<OrderUser> OrderUserDbs { get; set; }
		public DbSet<ItemByCustomer> ItemByCustomerDbs { get; set; }
		public DbSet<ItemColor> ItemColorDbs { get; set; }
		public DbSet<ItemSize> ItemSizeDbs { get; set; }
		public DbSet<ItemCategory> ItemCategoryDbs { get; set; }
		public DbSet<ItemSeason> ItemSeasonDbs { get; set; }
		public DbSet<ColorAndSizeByItem> ColorAndSizeByItemDbs { get; set; }
		public DbSet<PriceCurrency> PriceCurrencyDbs { get; set; }
		public DbSet<CartDetail> CartDetailDbs { get; set; }
		public DbSet<ItemCollection> ItemCollectionDbs { get; set; }
		public DbSet<Division> DivisionDbs { get; set; }

		//Order
		public DbSet<OrderHeader> OrderHeaderDbs { get; set; }
		public DbSet<OrderLine> OrderLineDbs { get; set; }


		#endregion DbSet
	}
}

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
			modelbuilder.Entity<Size>().HasNoKey();
			modelbuilder.Entity<Color>().HasNoKey();
			modelbuilder.Entity<ItemRegistration>().HasNoKey();
			modelbuilder.Entity<ItemSample>().HasNoKey();
			modelbuilder.Entity<ItemCategory>().HasNoKey();
			modelbuilder.Entity<ItemSeason>().HasNoKey();
			modelbuilder.Entity<ItemFit>().HasNoKey();
			modelbuilder.Entity<BuyerUser>().HasNoKey();

			modelbuilder.Entity<OrderHeader>().HasNoKey();

			//Update
			modelbuilder.Entity<OrderUser>().HasNoKey();
		}
		#region DbSet
		public DbSet<Size> SizeDbs { get; set; }
		public DbSet<Color> ColorDbs { get; set; }
		public DbSet<ItemRegistration> ItemRegistrationDbs { get; set; }
		public DbSet<ItemSample> ItemSampleDbs { get; set; }
		public DbSet<ItemCategory> ItemCategoryDbs { get; set; }
		public DbSet<ItemSeason> ItemSeasonDbs { get; set; }
		public DbSet<ItemFit> ItemFitDbs { get; set; }
		public DbSet<BuyerUser> BuyerUserDbs { get; set; }
		public DbSet<OrderHeader> OrderHeaderDbs { get; set; }

		//Update
		public DbSet<OrderUser> OrderUserDbs { get; set; }
		#endregion DbSet
	}
}

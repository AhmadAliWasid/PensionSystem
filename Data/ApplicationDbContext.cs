using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Entities.Models;
//using PensionSystem.Entities.Models;


namespace PensionSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // pensioner id and month must be unqiue
            builder.Entity<PensionerPayment>().HasIndex(p => new { p.PensionerId, p.Month }).IsUnique(true);
            // make sure the account number is unique
            builder.Entity<Pensioner>().HasIndex(p => new { p.AccountNumber, p.BranchId }).IsUnique(true);
            // make sure the list have one pensioner
            builder.Entity<Pensioner>().HasIndex(p => new { p.ClaimantCNIC }).IsUnique(true);
            // make sure the list have one pensioner
            builder.Entity<Company>().HasIndex(p => new { p.Order }).IsUnique(true);
            // short name is unique
            builder.Entity<Company>().HasIndex(p => new { p.ShortName }).IsUnique(true);
            builder.Entity<Company>().HasIndex(p => new { p.Name }).IsUnique(true);
            /// office entities
            builder.Entity<Office>().HasIndex(w => new { w.Title }).IsUnique(true);
            // HBL make sure the pensioner exist only once
            builder.Entity<HBLPayment>().HasIndex(h => new { h.Month, h.PensionerId }).IsUnique(true);
            builder.Entity<HBLPayment>().HasIndex(h => new { h.BranchId, h.AccountNumber, h.Month }).IsUnique(true);
            builder.Entity<HBLArrears>().HasIndex(h => new { h.PensionerId, h.FromMonth, h.ChequeId, h.Description }).IsUnique(true);
            builder.Entity<HBLArrears>().HasIndex(h => new { h.PensionerId, h.ToMonth, h.ChequeId, h.Description }).IsUnique(true);
            builder.Entity<Relation>().HasIndex(h => new { h.Name }).IsUnique(true);
            builder.Entity<MonthlyPensionDemand>().HasIndex(h => new { h.Number, h.Date }).IsUnique(true);
            builder.Entity<Bank>().HasIndex(b => new { b.ShortName });
            builder.Entity<Branch>().HasIndex(b => new { b.BankId, b.Code });
        }

        public DbSet<Company> Company { get; set; }
        public DbSet<Pensioner> Pensioner { get; set; }
        public DbSet<PensionerPayment> PensionerPayments { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<HBLPayment> HBLPayments { get; set; }
        public DbSet<ChequeCategory> ChequeCategory { get; set; }
        public DbSet<Cheque> Cheque { get; set; }
        public DbSet<HBLArrears> HBLArrears { get; set; }
        public DbSet<Commutation> Commutations { get; set; }
        public DbSet<ArrearsPayment> ArrearsPayments { get; set; }
        public DbSet<ArrearsDemand> ArrearsDemands { get; set; }
        public DbSet<Relation> Relations { get; set; }
        //public DbSet<WWFBill> WWFBills { get; set; }
        //public DbSet<WWFDemand> WWFDemands { get; set; }
        //public DbSet<WWFPayment> WWFPayments { get; set; }
        public DbSet<MonthlyPensionDemand> MonthlyPensionDemands { get; set; }
        public DbSet<RestorationDemand> RestorationDemands { get; set; }
        public DbSet<RestorationPayment> RestorationPayments { get; set; }
        public DbSet<HBLRestoration> HBLRestorations { get; set; }
        public DbSet<CashBook> CashBook { get; set; }
        public DbSet<ConversionCase> ConversionCases { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<PDU> PDUs { get; set; }
        public DbSet<UserPDU> UserPDUs { get; set; }
        public DbSet<MessagesHistory> MessagesHistories { get; set; }
        public DbSet<WWFSanction> WWFSanctions { get; set; }
    }
}
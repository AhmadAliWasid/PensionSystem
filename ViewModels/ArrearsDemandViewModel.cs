using PensionSystem.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace PensionSystem.ViewModels
{
	public class ArrearsDemandViewModel
	{
		public IEnumerable<ArrearsDemand>? ArrearsDemands { get; set; }
		public List<ArrearsPayment>? ArrearsPayments { get; set; }
	}
	public class CreateArrearDemandModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Please Specify a demand Description")]
		[StringLength(100)]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Number is Required!")]
		public int Number { get; set; }

		[Required(ErrorMessage = "Date is Required")]
		[DataType(DataType.Date)]
		public DateTime Date { get; set; }

		public bool IsPaid { get; set; }
		public DateTime? PaymentDate { get; set; }
		public int PDUId { get; set; }
	}
	public class SelectOptions
	{
		/// <summary>
		/// string the text of an option
		/// </summary>
		public string Text { get; set; } = string.Empty;

		/// <summary>
		/// value of selectio option
		/// </summary>
		public int Value { get; set; }
	}
}
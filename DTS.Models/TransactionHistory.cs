using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DTS.Models;

public class TransactionHistory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [ValidateNever]
    public Document Document { get; set; }

    [Required]
    public int StatusId { get; set; }

    [ValidateNever]
    public Status Status { get; set; }

    public string Remarks { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }
}
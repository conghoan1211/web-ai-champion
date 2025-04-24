using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public partial class UserDailyUsage
{
    [Key]
    [StringLength(36)]
    public string UsageId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    [Required]
    public DateOnly UsageDate { get; set; }

    public int? QuestionCount { get; set; } = 0;

    public int? MaxQuestionsPerDay { get; set; } = 20;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

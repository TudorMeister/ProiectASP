using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ProiectASP.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        public int ArticleId { get; set; } 
        public string Content { get; set; } = null!;

        public History(int articleId, string content)
        {
            ArticleId = articleId;
            Content = content;
        }
        public History()
        {

        }
    }
}

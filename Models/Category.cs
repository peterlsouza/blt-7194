using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve ter no máximo 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve ter no minimo 3 caracteres" )]
        //[DataType("nvarchar")]
        public string Title { get; set; }
        

        
    }
}

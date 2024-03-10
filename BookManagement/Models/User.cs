using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BookManagement.Models
{
    /// <summary>
    /// User Model
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id of the User (Identity Key)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }
        /// <summary>
        /// Name of the user
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string Name { get; set; }
        /// <summary>
        /// Email of the user
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Email { get; set; }
    }
}

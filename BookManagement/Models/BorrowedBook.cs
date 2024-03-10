using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models
{
    /// <summary>
    /// Model that server as an entry for keeping track of books that have been borrowed
    /// </summary>
    public class BorrowedBook
    {
        /// <summary>
        /// Id of the borrowed book (Key)
        /// </summary>
        [Key]
        [Required(ErrorMessage = "BookID is required.")]
        public int BookID { get; set; }
        /// <summary>
        /// Id of the User that borrwed the book
        /// </summary>
        [Required(ErrorMessage = "UserID is required.")]
        public int UserID { get; set; }
        /// <summary>
        /// Date when the book was borrowed
        /// </summary>
        [Required(ErrorMessage = "BorrowedFrom date is required.")]
        [Display(Name = "Borrowed From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly BorrowedFrom { get; set; }
        /// <summary>
        /// Date when the book shall be returned
        /// </summary>
        [Required(ErrorMessage = "BorrowedUntil date is required.")]
        [Display(Name = "Borrowed Until")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly BorrowedUntil { get; set; }
    }
}

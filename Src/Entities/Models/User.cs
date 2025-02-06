using Microsoft.EntityFrameworkCore;
using SlugEnt.BWA.Entities.EntityEnums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SlugEnt.BWA.Entities.Models
{
    /// <summary>
    ///     Person represents a single individual that is stored in the HRNextGen system.  A Person may in their lifetime go
    ///     thru several different employee relations with the organization
    ///     But there basic information stays with them. This is the basic information
    /// </summary>
    [Index(nameof(UPN), IsUnique = true)]
    [Index(nameof(LastName), IsUnique = false)]
    [Index(nameof(FirstName), IsUnique = false)]
    public class User : Abstract_IIDInt_Audit
    {
        [StringLength(Constants.EMAIL)] public string? EmailExternal { get; set; }

        // Relationships

        [Required]
        [StringLength(Constants.NAME_LENGTH)]
        public string FirstName { get; set; } = "First";

        [Required]
        [StringLength(Constants.NAME_LENGTH)]
        public string LastName { get; set; } = "LastName";


        [StringLength(Constants.NAME_LENGTH)] public string? MiddleName { get; set; }

        [StringLength(Constants.NAME_LENGTH)] public string NickName { get; set; } = string.Empty;


        [StringLength(Constants.PHONE)] public string? CellPhone { get; set; }


        /// <summary>
        ///     The type of user
        /// </summary>
        [Column(TypeName = "tinyint")]
        public EnumUserTypes Type { get; set; }



        /// <summary>
        ///     The Corporate / Organization Universal Principal Name Identifier
        /// </summary>
        public string? UPN { get; set; }



        // Custom Methods
        public static string FullName(User p)
        {
            if (p == null)
            {
                return "";
            }

            StringBuilder sb = new();

            sb.Append(p.LastName + ", " + p.FirstName);
            if (p.MiddleName != null)
            {
                sb.Append(" " + p.MiddleName);
            }

            if (p.NickName != null)
            {
                sb.Append(" [ " + p.NickName + " ]");
            }

            return sb.ToString();
        }
    }
}
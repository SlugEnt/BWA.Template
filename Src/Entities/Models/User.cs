using Microsoft.EntityFrameworkCore;
using SlugEnt.BWA.Entities.EntityEnums;
using System;
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
    [Index(nameof(Email), IsUnique = true)]
    public class User : AbstractEntityInt, IEntityInt
    {
        [Required]
        [StringLength(Constants.NAME_LENGTH)]
        [Column(Order = 202)]
        public string FirstName { get; set; } = "";


        [Required]
        [StringLength(Constants.NAME_LENGTH)]
        [Column(Order = 200)]
        public string LastName { get; set; } = "";


        /// <summary>
        ///     The type of person as in their relationship to the organization.  Ie, Employee, Contractor, etc.
        /// </summary>
        [Column(TypeName = "tinyint", Order = 210)]
        public EnumUserTypes Type { get; set; }


        /// <summary>
        ///     The Corporate / Organization Universal Principal Name Identifier
        /// </summary>
        [Column(Order = 220)]
        public string? UPN { get; set; }


        /// <summary>
        /// The person's company email address.
        /// </summary>
        [StringLength(Constants.EMAIL)]
        [Column(Order = 250)]
        public string? Email { get; set; } = "";



        [Required]
        [Column(Order = 252)]
        [StringLength(Constants.PHONE)]
        public string? PhoneExternal { get; set; }


        [StringLength(Constants.NAME_LENGTH)] public string? MiddleName { get; set; }

        [StringLength(Constants.NAME_LENGTH)] public string? NickName { get; set; } = string.Empty;


        // Custom Methods
        public static string FullName_Create(User u)
        {
            if (u == null)
            {
                return "";
            }

            StringBuilder sb = new();

            sb.Append(u.LastName + ", " + u.FirstName);
            if (u.MiddleName != null)
            {
                sb.Append(" " + u.MiddleName);
            }

            if (u.NickName != null)
            {
                sb.Append(" [ " + u.NickName + " ]");
            }

            return sb.ToString();
        }


        /// <summary>
        /// The users full name
        /// </summary>
        public override string FullName
        {
            get { return FullName_Create(this); }
        }

        /// <summary>
        /// Prints User Key Info.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return FullName; }

    }
}
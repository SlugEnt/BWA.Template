
namespace SlugEnt.BWA.Entities.Models;
    public class AppSetting : AbstractEntityLookups, IEntityInt
    {
        /// <summary>
        /// The version of the Database Code that is currently in use.  Used to determine if specialized initialization code needs to be run when
        /// certain database changes are made - such as inserting values into new properties or tables.
        /// </summary>
        public int CurrentDbCodeVersion { get; set; }

        public override string FullName
        {
            get { return Name; }
        }

        public override string ToString() { return FullName; }
    }


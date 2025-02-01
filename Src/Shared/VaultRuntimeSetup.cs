using Sheakley.ICS.Common.UtilityObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheakley.ICS.Template.Blazor.Shared
{
    /// <summary>
    /// This class is used to setup access to the Sheakley Vault for the application.
    /// </summary>
    public class VaultRuntimeSetup
    {
        public bool GenerateHash { get; set; } = false;
        public bool OutputSPI { get; set; } = false;
        public string VaultUrl { get; set; } = "";
        public bool DevelopmentMode { get; set; } = false;
        public bool SkipVaultErrors { get; set; } = false;
        public bool UseVault { get; set; } = true;
        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// The name of the application as it is stored in the Vault.
        /// </summary>
        public string AssemblyQualifiedName { get; set; } = "";

        public string SecretOverwriteMode { get; set; } = "none";


        /// <summary>
        /// The Applications runtime ServiceProgramInfo 
        /// </summary>
        public ServiceProgramInfo ServiceProgramInfo { get; set; }

        public VaultRuntimeSetup() { }


    }
}

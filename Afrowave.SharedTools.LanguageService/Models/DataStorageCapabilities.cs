using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.I18N.Models
{
    /// <summary>
    /// Represents the capabilities of a data storage provider for language resources.
    /// </summary>
    public class DataStorageCapabilities
    {
        /// <summary>
        /// Indicates whether the storage can read data.
        /// </summary>
        public bool CanRead { get; set; } = true;
        /// <summary>
        /// Indicates whether the storage can write data.
        /// </summary>
        public bool CanWrite { get; set; } = true;
        /// <summary>
        /// Indicates whether the storage can delete data.
        /// </summary>
        public bool CanDelete { get; set; } = true;
        /// <summary>
        /// Indicates whether the storage can list available languages.
        /// </summary>
        public bool CanListLanguages { get; set; } = true;
        /// <summary>
        /// Indicates whether the storage can check if a resource exists.
        /// </summary>
        public bool CanCheckExistence { get; set; } = true;
        /// <summary>
        /// Indicates whether the storage is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; } = false;
    }
}

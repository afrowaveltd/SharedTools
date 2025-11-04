using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.IO.Models
{
   /// <summary>
   /// Specifies the supported file formats for data import and export operations.
   /// </summary>
   /// <remarks>Use this enumeration to indicate the format of a file when reading from or writing to data sources.
   /// The values correspond to commonly used structured data formats. Ensure that the selected format matches the expected
   /// content and structure of the file being processed.</remarks>
   public enum FileType
   {
      /// <summary>
      /// Specifies that the content type is JSON format.
      /// </summary>
      Json = 1,

      /// <summary>
      /// Specifies that the data format is comma-separated values (CSV).
      /// </summary>
      Csv = 2,

      /// <summary>
      /// Specifies that the data format is XML.
      /// </summary>
      Xml = 3,

      /// <summary>
      /// Represents the YAML (YAML Ain't Markup Language) data format option.
      /// </summary>
      /// <remarks>Use this value to indicate that data should be processed or interpreted as YAML. YAML is a
      /// human-readable data serialization standard commonly used for configuration files and data exchange between
      /// languages with different data structures.</remarks>
      Yaml = 4
   }
}
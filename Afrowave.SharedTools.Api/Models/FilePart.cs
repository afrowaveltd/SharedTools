using System;

namespace Afrowave.SharedTools.Api.Models
{
	/// <summary>
	/// Reprezentuje soubor přiložený ve multipart/form-data.
	/// Pro jednoduchost držíme obsah v paměti (byte[]).
	/// Pro velmi velké soubory můžeš později doplnit streamovou variantu.
	/// </summary>
	public sealed class FilePart
	{
		/// <summary>
		/// Gets or sets the name of the form field.
		/// </summary>
		public string Name { get; set; } = string.Empty;          // název form field (např. "file")

		/// <summary>
		/// Gets or sets the name of the file, including its extension.
		/// </summary>
		public string FileName { get; set; } = string.Empty;      // název souboru (např. "doc.pdf")

		/// <summary>
		/// Gets or sets the MIME type of the content associated with this instance.
		/// </summary>
		/// <remarks>The content type typically follows the standard MIME type format, such as "application/pdf" or
		/// "image/png". Setting an appropriate content type is important for correct handling by clients and
		/// servers.</remarks>
		public string ContentType { get; set; } = string.Empty;   // např. "application/pdf"

		/// <summary>
		/// Gets or sets the raw byte data associated with this instance.
		/// </summary>
		public byte[] Bytes { get; set; } = Array.Empty<byte>();

		/// <summary>
		/// Initializes a new instance of the FilePart class.
		/// </summary>
		public FilePart()
		{
		}

		/// <summary>
		/// Initializes a new instance of the FilePart class with the specified part name, file name, content type, and file
		/// data.
		/// </summary>
		/// <param name="name">The name of the file part, typically used as the form field name in multipart requests. Cannot be null.</param>
		/// <param name="fileName">The name of the file as it should appear in the request or on the server. Cannot be null.</param>
		/// <param name="contentType">The MIME type of the file content, such as "image/png" or "application/pdf". Cannot be null.</param>
		/// <param name="bytes">The binary data representing the contents of the file. If null, an empty byte array is used.</param>
		public FilePart(string name, string fileName, string contentType, byte[] bytes)
		{
			Name = name;
			FileName = fileName;
			ContentType = contentType;
			Bytes = bytes ?? Array.Empty<byte>();
		}
	}
}
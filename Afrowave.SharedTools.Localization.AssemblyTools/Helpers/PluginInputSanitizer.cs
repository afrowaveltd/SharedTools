using System;

namespace Afrowave.SharedTools.Localization.AssemblyTools.Helpers
{
	/// <summary>
	/// Provides input normalization to ensure that null values are never passed to plugins.
	/// Dispatcher and other core components can use this to enforce null safety.
	/// </summary>
	public static class PluginInputSanitizer
	{
		/// <summary>
		/// Returns a safe fallback value if the input is null.
		/// </summary>
		/// <param name="input">Input value to validate.</param>
		/// <param name="expectedType">Type expected by the receiver.</param>
		/// <returns>Non-null instance of the expected type.</returns>
		public static object Sanitize(object? input, Type expectedType)
		{
			if(input != null) return input;

			if(expectedType == typeof(string)) return string.Empty;
			if(expectedType == typeof(int)) return 0;
			if(expectedType == typeof(bool)) return false;
			if(expectedType == typeof(Guid)) return Guid.Empty;

			if(expectedType.IsArray)
				return Array.CreateInstance(expectedType.GetElementType() ?? typeof(object), 0);

			if(expectedType.IsValueType)
				return Activator.CreateInstance(expectedType) ?? Activator.CreateInstance(typeof(object))!;

			// fallback for reference types
			return Activator.CreateInstance(expectedType) ?? new object();
		}
	}
}
namespace Afrowave.SharedTools.Docs.Models.Enums;

/// <summary>
/// The selected action that will be performed on the translation
/// </summary>
public enum Action
{
	/// <summary>
	/// Adds the specified item to the collection.
	/// </summary>
	/// <remarks>This method appends the item to the end of the collection. If the collection has a fixed size or is
	/// read-only,  an exception may be thrown depending on the implementation.</remarks>
	Add,

	/// <summary>
	/// Deletes the specified resource or entity.
	/// </summary>
	/// <remarks>This method removes the target resource or entity from the system. Ensure that the resource exists
	/// and is no longer needed before calling this method. Depending on the implementation, this operation may be
	/// irreversible.</remarks>
	Delete,

	/// <summary>
	/// Modifies the current object or state based on the provided implementation.
	/// </summary>
	/// <remarks>The specific behavior of this method depends on the implementation.  Ensure that any preconditions
	/// required by the implementation are met before calling this method.</remarks>
	Modify
}
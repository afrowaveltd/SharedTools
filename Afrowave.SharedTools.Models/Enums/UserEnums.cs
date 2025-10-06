namespace Afrowave.SharedTools.Models.Enums
{
	/// <summary>
	/// Contains user-related enumerations for status, role, gender, and age group access.
	/// </summary>
	public class UserEnums
	{
		/// <summary>
		/// Represents the status of a user account.
		/// </summary>
		public enum UserStatus
		{
			/// <summary>
			/// The user is active and has full access.
			/// </summary>
			Active = 1,

			/// <summary>
			/// The user is inactive and cannot access the system.
			/// </summary>
			Inactive = 2,

			/// <summary>
			/// The user is suspended due to policy violations or other reasons.
			/// </summary>
			Suspended = 3,

			/// <summary>
			/// The user account is pending approval or activation.
			/// </summary>
			Pending = 4
		}

		/// <summary>
		/// Represents the role assigned to a user.
		/// </summary>
		public enum UserRole
		{
			/// <summary>
			/// Guest user with limited access.
			/// </summary>
			Guest = 1,

			/// <summary>
			/// Regular user with standard permissions.
			/// </summary>
			User = 2,

			/// <summary>
			/// User with editing permissions.
			/// </summary>
			Editor = 3,

			/// <summary>
			/// User with moderation capabilities.
			/// </summary>
			Moderator = 4,

			/// <summary>
			/// Administrator with full access.
			/// </summary>
			Admin = 5,

			/// <summary>
			/// Owner of the account or system.
			/// </summary>
			Owner = 6
		}

		/// <summary>
		/// Represents the gender identity of a user.
		/// </summary>
		public enum UserGender
		{
			/// <summary>
			/// No gender specified.
			/// </summary>
			None = 0,

			/// <summary>
			/// Female gender.
			/// </summary>
			Female = 1,

			/// <summary>
			/// Male gender.
			/// </summary>
			Male = 2,

			/// <summary>
			/// Non-binary gender.
			/// </summary>
			NonBinary = 3,

			/// <summary>
			/// Other gender identity.
			/// </summary>
			Other = 4,
		}

		/// <summary>
		/// Represents the age group access level for users.
		/// </summary>
		public enum UserAgeGroupAccess
		{
			/// <summary>
			/// Accessible to everyone.
			/// </summary>
			Everyone = 1,

			/// <summary>
			/// Accessible to kids.
			/// </summary>
			Kids = 2,

			/// <summary>
			/// Accessible to teens.
			/// </summary>
			Teens = 3,

			/// <summary>
			/// Accessible to adults.
			/// </summary>
			Adults = 4
		}
	}
}
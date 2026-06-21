using System;

namespace Library.API.Models;

public class Author
{
	/// <summary>
	/// The id of the author
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// The first name of the author
	/// </summary>
	public string? FirstName { get; set; }

	/// <summary>
	/// The last name of the author
	/// </summary>
	public string? LastName { get; set; }
}

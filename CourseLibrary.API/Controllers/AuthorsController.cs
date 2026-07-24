
using System.Text.Json;
using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController(
	ICourseLibraryRepository courseLibraryRepository,
	IMapper mapper) : ControllerBase
{
	private readonly ICourseLibraryRepository _courseLibraryRepository = courseLibraryRepository ??
			throw new ArgumentNullException(nameof(courseLibraryRepository));
	private readonly IMapper _mapper = mapper ??
			throw new ArgumentNullException(nameof(mapper));

	[HttpGet(Name = "GetAuthors")]
	[HttpHead]
	public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(
		[FromQuery] AuthorsResourceParameters authorsResourceParameters)
	{
		// get authors from repo
		var authorsFromRepo = await _courseLibraryRepository
			.GetAuthorsAsync(authorsResourceParameters);

		var previousPageLink = authorsFromRepo.HasPrevious
			? CreateAuthorsResourceUri(authorsResourceParameters,
				ResourceUriType.PreviousPage)
			: null;

		var nextPageLink = authorsFromRepo.HasNext
			? CreateAuthorsResourceUri(authorsResourceParameters,
				ResourceUriType.NextPage)
			: null;

		var paginationMetadata = new
		{
			totalCount = authorsFromRepo.TotalCount,
			pageSize = authorsFromRepo.PageSize,
			currentPage = authorsFromRepo.CurrentPage,
			totalPages = authorsFromRepo.TotalPages,
			previousPageLink = previousPageLink,
			nextPageLink = nextPageLink
		};

		Response.Headers.Add("X-Pagination",
			JsonSerializer.Serialize(paginationMetadata));

		// return them
		return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
	}

	private string? CreateAuthorsResourceUri(
		AuthorsResourceParameters authorsResourceParameters,
		ResourceUriType type)
	{
		switch (type)
		{
			case ResourceUriType.PreviousPage:
				return Url.Link("GetAuthors",
					new
					{
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery,
						pageNumber = authorsResourceParameters.PageNumber - 1,
						pageSize = authorsResourceParameters.PageSize
					});
			case ResourceUriType.NextPage:
				return Url.Link("GetAuthors",
					new
					{
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery,
						pageNumber = authorsResourceParameters.PageNumber + 1,
						pageSize = authorsResourceParameters.PageSize
					});
			default:
				return Url.Link("GetAuthors",
					new
					{
						mainCategory = authorsResourceParameters.MainCategory,
						searchQuery = authorsResourceParameters.SearchQuery,
						pageNumber = authorsResourceParameters.PageNumber,
						pageSize = authorsResourceParameters.PageSize
					});
		}
	}

	[HttpGet("{authorId}", Name = "GetAuthor")]
	public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
	{
		// get author from repo
		var authorFromRepo = await _courseLibraryRepository.GetAuthorAsync(authorId);

		if (authorFromRepo == null)
		{
			return NotFound();
		}

		// return author
		return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
	}

	[HttpPost]
	public async Task<ActionResult<AuthorDto>> CreateAuthor(
		AuthorForCreationDto author)
	{
		var authorEntity = _mapper.Map<Entities.Author>(author);

		_courseLibraryRepository.AddAuthor(authorEntity);
		await _courseLibraryRepository.SaveAsync();

		var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

		return CreatedAtRoute("GetAuthor",
			new { authorId = authorToReturn.Id },
			authorToReturn);
	}

	[HttpOptions()]
	public IActionResult GetAuthorOptions()
	{
		Response.Headers.Add("Allow", "GET,HEAD,POST,OPTIONS");
		return Ok();
	}
}

using AutoMapper;

namespace Books.API.Profiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Entities.Book, Models.Book>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src =>
                $"{src.Author.FirstName} {src.Author.FirstName}"));

            CreateMap<Models.BookForCreation, Entities.Book>();

            CreateMap<Entities.Book, Models.BookWithCovers>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src =>
                    $"{src.Author.FirstName} {src.Author.LastName}"));

            CreateMap<ExternalModels.BookCover, Models.BookCover>();
        }
    }
}

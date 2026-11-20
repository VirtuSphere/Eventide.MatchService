using AutoMapper;
using MatchService.Application.Models.Administrator;
using MatchService.Application.Models.Bracket;
using MatchService.Application.Models.Match;
using MatchService.Application.Models.User;
using MatchService.Domain;

namespace MatchService.Application.Services.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<Bracket, BracketModel>();

        CreateMap<Administrator, AdministratorModel>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value));

        CreateMap<User, UserModel>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value));

        CreateMap<Match, MatchModel>()
            .ForMember(dest => dest.TournamentId, opt => opt.MapFrom(src => src.Tournament.Id))
            .ForMember(dest => dest.BracketId, opt => opt.MapFrom(src => src.Bracket.Id))
            .ForMember(dest => dest.AdministratorId, opt => opt.MapFrom(src => src.Administrator.Id))
            .ForMember(dest => dest.Player1Id, opt => opt.MapFrom(src => src.Player1.Id))
            .ForMember(dest => dest.Player2Id, opt => opt.MapFrom(src => src.Player2.Id))
            .ForMember(dest => dest.WinnerId, opt => opt.MapFrom(src => src.Winner == null ? (Guid?)null : src.Winner.Id))
            .ForMember(dest => dest.MapName, opt => opt.MapFrom(src => src.MapName == null ? (string?)null : src.MapName.Value))
            .ForMember(dest => dest.ServerInfo, opt => opt.MapFrom(src => src.ServerInfo == null ? (string?)null : src.ServerInfo.Value))
            .ForMember(dest => dest.Player1Score, opt => opt.MapFrom(src => src.Player1Score == null ? (int?)null : src.Player1Score.Value))
            .ForMember(dest => dest.Player2Score, opt => opt.MapFrom(src => src.Player2Score == null ? (int?)null : src.Player2Score.Value));
    }
}
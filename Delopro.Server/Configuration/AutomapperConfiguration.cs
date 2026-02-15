using AutoMapper;

using Delopro.Server.Models;
using Delopro.Data.Entities;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Delopro.Server.Enums;
using System.ComponentModel;
using Delopro.Data.Enums;
using Delopro.Bll.Services;
using Delopro.Bll;

namespace Delopro.Server.Configurations
{
    public static class AutomapperConfiguration
    {
        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            _ = services.AddSingleton(provider =>
            {
                var cryptoService = provider.GetRequiredService<CryptoService>();

                var config = new MapperConfiguration(autoMapperConfig =>
                {
                    autoMapperConfig.CreateMap<RegisterRequest, User>()
                        .ForMember(dest => dest.Nickname, opts => opts.MapFrom(src => src.Nickname))
                        .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => src.FirstName))
                        .ForMember(dest => dest.Password, opts => opts.MapFrom(src => src.Password))
                        .ForMember(dest => dest.IsConfirmed, opts => opts.Ignore());

                    autoMapperConfig.CreateMap<Chapter, ChapterResponse>()
                        .ForMember(dest => dest.Themes, opts => opts.MapFrom(src =>
                            src.Themes == null
                            ? Array.Empty<ThemeResponse>()
                            : src.Themes.Select(x => new ThemeResponse
                            {
                                ThemeId = x.ThemeId,
                                UserId = x.UserId,
                                ChapterId = x.ChapterId,
                                ThemeTitle = x.ThemeTitle,
                                Content = x.Content,
                                DateCreated = x.DateCreated,
                                DateDeleted = x.DateDeleted
                            }
                        ).ToArray()));

                    autoMapperConfig.CreateMap<Chapter, ChapterNode>()
                        .ForMember(dest => dest.Key, opts => opts.MapFrom(src => $"{src.ChapterId}"))
                        .ForMember(dest => dest.Label, opts => opts.MapFrom(src => src.ChapterTitle))
                        .ForMember(dest => dest.Children, opts => opts.MapFrom(src =>
                            src.Themes == null
                            ? Array.Empty<ThemeNode>()
                            : src.Themes.Select(x => new ThemeNode
                            {
                                Key = $"{x.ChapterId}-{x.ThemeId}",
                                Label = x.ThemeTitle,
                                Data = $"/chapters/{x.ChapterId}/{x.ThemeId}"
                            }).ToArray()));

                    autoMapperConfig.CreateMap<ChapterUpdateRequest, Chapter>().ForMember(dest => dest.Themes, opts => opts.Ignore());

                    autoMapperConfig.CreateMap<ThemeUpdateRequest, Theme>()
                        .ForMember(dest => dest.Content, opts => opts.MapFrom(src => src.Content != null ? src.Content.Replace("&nbsp;", " ") : null));
                    autoMapperConfig.CreateMap<ThemeCreateRequest, Theme>()
                        .ForMember(dest => dest.Content, opts => opts.MapFrom(src => src.Content != null ? src.Content.Replace("&nbsp;", " ") : null));

                    autoMapperConfig.CreateMap<MessageForm, Message>();
                    autoMapperConfig.CreateMap<Message, MessageResponse>()
                        .ForMember(dest => dest.Contacts, opts => opts.MapFrom(src => GetContacts(src.Email, src.Phone)));

                    autoMapperConfig.CreateMap<User, UserShortResponse>()
                        .ForMember(dest => dest.Email, opts => opts.MapFrom(src => cryptoService.Decrypt(src.Email)))
                        .ForMember(dest => dest.Roles, opts => opts.MapFrom(src =>
                            src.UserRoles != null
                            ? string.Join(", ", src.UserRoles.Select(x => GetEnumDescription((UserRoleType)x.RoleId)))
                            : "")
                        )
                        .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.IsDeleted ? UserStatus.Deleted :
                            (src.IsConfirmed ? UserStatus.Confirmed : UserStatus.NotConfirmed))
                        )
                        .ForMember(dest => dest.AvatarPath, opts => opts.MapFrom(src => GetFullAvatarPath(src.AvatarPath)))
                        .ForMember(dest => dest.FullName, opts => opts.MapFrom(src => GetFullName(src, cryptoService)));

                    autoMapperConfig.CreateMap<User, UserLongResponse>()
                        .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => cryptoService.Decrypt(src.FirstName)))
                        .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => cryptoService.Decrypt(src.LastName)))
                        .ForMember(dest => dest.Email, opts => opts.MapFrom(src => cryptoService.Decrypt(src.Email)))
                        .ForMember(dest => dest.Phone, opts => opts.MapFrom(src => cryptoService.Decrypt(src.Phone)))
                        .ForMember(dest => dest.AvatarPath, opts => opts.MapFrom(src => GetFullAvatarPath(src.AvatarPath)))
                        .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.IsDeleted ? UserStatus.Deleted :
                            (src.IsConfirmed ? UserStatus.Confirmed : UserStatus.NotConfirmed))
                        );

                    autoMapperConfig.CreateMap<User, AccountResponse>()
                        .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => cryptoService.Decrypt(src.FirstName)))
                        .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => cryptoService.Decrypt(src.LastName)))
                        .ForMember(dest => dest.Email, opts => opts.MapFrom(src => cryptoService.Decrypt(src.Email)))
                        .ForMember(dest => dest.Phone, opts => opts.MapFrom(src => cryptoService.Decrypt(src.Phone)))
                        .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => src.BirthDate != null ? ((DateTime)src.BirthDate).ToShortDateString() : null))
                        .ForMember(dest => dest.RegisterDate, opts => opts.MapFrom(src => src.RegisterDate != null ? ((DateTime)src.RegisterDate).ToShortDateString() : null))
                        .ForMember(dest => dest.AvatarPath, opts => opts.MapFrom(src => GetFullAvatarPath(src.AvatarPath)))                        
                        .ForMember(dest => dest.Roles, opts => opts.MapFrom(src =>
                            src.UserRoles != null
                                ? src.UserRoles.Select(x => GetEnumDescription((UserRoleType)x.RoleId))
                                : Enumerable.Empty<string?>())
                        );

                    autoMapperConfig.CreateMap<AccountUpdateRequest, User>()
                        .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => cryptoService.Encrypt(src.FirstName)))
                        .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => cryptoService.Encrypt(src.LastName)))
                        .ForMember(dest => dest.Email, opts => opts.MapFrom(src => cryptoService.Encrypt(src.Email)))
                        .ForMember(dest => dest.Phone, opts => opts.MapFrom(src => cryptoService.Encrypt(src.Phone)));
                });

                return config.CreateMapper();
            });
        }

        private static string? GetFullName(User? user, CryptoService cryptoService)
        {
            return (user?.FirstName, user?.LastName) switch
            {
                (var x, var y) when !x.IsNullOrEmpty() && !y.IsNullOrEmpty() => $"{cryptoService.Decrypt(x)} {cryptoService.Decrypt(y)}",
                (var x, var y) when !x.IsNullOrEmpty() && y.IsNullOrEmpty() => cryptoService.Decrypt(x),
                (var x, var y) when x.IsNullOrEmpty() && !y.IsNullOrEmpty() => cryptoService.Decrypt(y),
                _ => null
            };
        }

        private static string? GetFullAvatarPath(string? avatarPath) =>
            avatarPath is null ? null : (ConfigurationHelper.IsDevelopment ? $"/src/assets/avatars/{avatarPath}" : $"/avatars/{avatarPath}");

        private static string? EncodeUTF8(byte[]? bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                return null;
            }

            return Encoding.UTF8.GetString(bytes);
        }

        private static string? GetContacts(string? email, string? phone)
        {
            return (email, phone) switch
            {
                (var e, var p) when !e.IsNullOrEmpty() && !p.IsNullOrEmpty() => $"Email: {e}\n\rТел.: {p}",
                (var e, var p) when e.IsNullOrEmpty() && !p.IsNullOrEmpty() => $"Тел.: {p}",
                (var e, var p) when !e.IsNullOrEmpty() && p.IsNullOrEmpty() => $"Email: {e}",
                _ => null
            };
        }

        private static string? GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field != null ? (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) : null;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
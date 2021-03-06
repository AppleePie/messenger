using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Server.DatabaseWorkers;
using Server.Models;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddCors(options => options.AddPolicy("ClientPermission", policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:3000")
                    .AllowCredentials();
            }));
            services.AddControllers().AddNewtonsoftJson().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Server", Version = "v1"}); });
            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<Repository>();
            services.AddAutoMapper(cfg =>
                {
                    cfg.CreateMap<UserToCreateDto, User>();
                    cfg.CreateMap<User, UserToSendDto>().ForMember(u => u.Chats,
                        options =>
                            options.MapFrom(user => user.UserToChats.Select(c => new ChatForUser
                                {
                                    Id = c.ChatId,
                                    Interlocutor = c.Chat.UserToChats.First(u => u.UserId != user.Id).UserId
                                }).ToList()
                            )
                    );
                    cfg.CreateMap<Chat, ChatToSend>()
                        .ForMember(c => c.Participants,
                            options => options.MapFrom(chat =>
                                chat.UserToChats.Select(u => u.User.Id).ToList()
                            )
                        )
                        .ForMember(c => c.Messages,
                            options => options.MapFrom(chat =>
                                chat.ChatToMessages.Select(x => new ChatMessage
                                {
                                    Id = x.MessageId,
                                    Initiator = x.Message.UserToMessage.UserId,
                                    Interlocutor = x.Chat.UserToChats.First(u => u.UserId != x.Message.UserToMessage.UserId).UserId,
                                    Message = x.Message.Content
                                }).ToList()
                            )
                        ); 
                },
                Array.Empty<Assembly>()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1"));
            }

            app.UseHttpsRedirection();
            // app.UseCors(builder => builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod());
            app.UseCors("ClientPermission");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hubs/chat");
            });
        }
    }
}
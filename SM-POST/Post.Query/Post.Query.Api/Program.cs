using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Action<DbContextOptionsBuilder> configureDbContext = options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
};

builder.Services.AddDbContext<DataContext>(configureDbContext);

builder.Services.AddSingleton(new DataContextFactory(configureDbContext));

//create db and tables from code

var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DataContext>();
dataContext.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<IQueryHandler, QueryHandler>();

var handler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();

var dispatcher = new QueryDispatcher<PostEntity>();

dispatcher.RegisterHandler<FindAllPostsQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindAllPostsWithCommentsQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCertainLikesQuery>(handler.HandleAsync);

builder.Services.AddSingleton<IQueryDispatcher<PostEntity>, QueryDispatcher<PostEntity>>(_ => dispatcher);


builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));

builder.Services.AddScoped<IEventConsumer, EventConsumer>();

builder.Services.AddControllers();

//ensure that starasync method in consumer hosted service will start once app is run
builder.Services.AddHostedService<ConsumerHostedService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

﻿using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Extensions;
using SurveyBasket.API.Health;
using SurveyBasket.API.Settings;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.API
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllers();

			//services.AddCors(options =>
			//	options.AddPolicy("AllowAll", builder =>
			//		builder
			//			.AllowAnyOrigin()
			//			.AllowAnyMethod()
			//			.AllowAnyHeader()
			//			//.WithOrigins("http://localhost:3000")
			//	)
			//);
			services.AddCors(options =>
				options.AddDefaultPolicy(builder =>
				builder
					.AllowAnyMethod()
					.AllowAnyHeader()
					.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
				)

			);


			services.AddAuthConfig(configuration);
			services.AddBackgroundJobsConfig(configuration);

			var connectionStrings = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String DefaultConnection not found.");
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionStrings));

			services
				.AddSwaggerServices()
				.AddFluentValidationConfig();


			services.AddScoped<IPollService, PollService>();
			services.AddScoped<IQuestionService, QuestionService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IVoteService, VoteService>();
			services.AddScoped<IResultService, ResultService>();
			services.AddScoped<IEmailSender, EmailService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IRoleService, RoleService>();

			services.AddExceptionHandler<GlobalExceptionHandler>();
			services.AddProblemDetails();

			services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
			services.AddHttpContextAccessor();

			services.AddHealthChecks()
						.AddDbContextCheck<ApplicationDbContext>(name: "database")
						.AddHangfire(options => { options.MinimumAvailableServers = 1;})
						.AddCheck<MailProviderHealthCheck>(name: "mail service");

			services.AddRateLimitingConfig();
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true;

				options.ApiVersionReader = new UrlSegmentApiVersionReader();
			})
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'V";
				options.SubstituteApiVersionInUrl = true;
			});

			return services;
		}
		private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
		{
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();

			return services;
		}
		private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
		{
			services
				.AddFluentValidationAutoValidation()
				.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			return services;
		}

		private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddIdentity<ApplicationUser, /*IdentityRole*/ApplicationRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders(); // 20

			services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
			services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

			services.AddSingleton<IJwtProvider, JwtProvider>();
			//services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
			services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

			var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(o =>
				{
					o.SaveToken = true;
					o.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
						ValidIssuer = jwtSettings?.Issuer,
						ValidAudience = jwtSettings?.Audience
					};
				});

			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequiredLength = 8;
				options.SignIn.RequireConfirmedEmail = true;
				options.User.RequireUniqueEmail = true;

				//options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //25
				//options.Lockout.MaxFailedAccessAttempts = 5;  //25
				//options.Lockout.AllowedForNewUsers = true; //25
			});

			return services;
		}

		private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHangfire(config => config
					.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
					.UseSimpleAssemblyNameTypeSerializer()
					.UseRecommendedSerializerSettings()
					.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

			services.AddHangfireServer();

			return services;
		}

		private static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
		{
			services.AddRateLimiter(rateLimiterOptions => {
				rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

				rateLimiterOptions.AddPolicy(RateLimiters.IpLimiter, httpContent =>
					RateLimitPartition.GetFixedWindowLimiter(
						partitionKey: httpContent.Connection.RemoteIpAddress?.ToString(),
						factory: _ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 2,
							Window = TimeSpan.FromSeconds(20)
						}
					)
				);

				rateLimiterOptions.AddPolicy(RateLimiters.UserLimiter, httpContent =>
					RateLimitPartition.GetFixedWindowLimiter(
						partitionKey: httpContent.User.GetUserId(),
						factory: _ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 2,
							Window = TimeSpan.FromSeconds(20)
						}
					)
				);

				rateLimiterOptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
				{
					options.PermitLimit = 1000;
					options.QueueLimit = 100;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				});

				rateLimiterOptions.AddTokenBucketLimiter(RateLimiters.Token, options =>
				{
					options.TokenLimit = 2;
					options.QueueLimit = 1;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
					options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
					options.TokensPerPeriod = 2;
					options.AutoReplenishment = true;
				});

				rateLimiterOptions.AddFixedWindowLimiter(RateLimiters.Fixed, options =>
				{
					options.PermitLimit = 2;
					options.Window = TimeSpan.FromSeconds(20);
					options.QueueLimit = 1;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				});

				rateLimiterOptions.AddSlidingWindowLimiter(RateLimiters.Sliding, options =>
				{
					options.PermitLimit = 2;
					options.Window = TimeSpan.FromSeconds(20);
					options.SegmentsPerWindow = 2;
					options.QueueLimit = 1;
					options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				});
			});

			return services;
		}

	}
}

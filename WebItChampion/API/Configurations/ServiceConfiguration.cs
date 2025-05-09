﻿using Amazon.S3;
using API.Models;
using API.Services;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext with connection string from the configuration
            services.AddDbContext<Sep490Context>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MyDB")));

            services.AddSingleton<JwtAuthentication>(); 
            services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(
                  ConfigManager.gI().AWSAccessKey,
                  ConfigManager.gI().AWSSecretKey,
                  Amazon.RegionEndpoint.GetBySystemName(ConfigManager.gI().AWSRegion)
            ));
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IAmazonS3Service, AmazonS3Service>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<ISkillCoreService, SkillCoreService>();
            services.AddScoped<IAIQuizGeneratorService, AIQuizGeneratorService>();
            services.AddScoped<ISkillGeneratorService, SkillGeneratorService>();
            services.AddScoped<IQuizQuestionService, QuizQuestionService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IQuizSubmissionService, QuizSubmissionService>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();

        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Infrastructure.Config;
using RecordHub.CatalogService.Infrastructure.Elasticsearch;

namespace RecordHub.CatalogService.Infrastructure.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var cfg = configuration.Get<AppConfig>();
            services.Configure<ElasticsearchConfig>(
                     configuration.GetSection(
                         key: nameof(ElasticsearchConfig)));

            services.AddScoped<IndexInitializer>();

            var settings = new ConnectionSettings(new Uri(cfg.ElasticsearchConfig.Url))
                .DefaultIndex(cfg.ElasticsearchConfig.Index);

            AddDefaultMappings(settings);
            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }

        public static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<RecordDTO>(x => x
                .AutoMap()
                .Properties(p => p
                .Nested<LabelDTO>(n => n
                    .IncludeInRoot()
                    .Name(np => np.Label)
                    .AutoMap())
                .Nested<StyleDTO>(n => n
                    .IncludeInRoot()
                    .Name(np => np.Styles)
                    .AutoMap())
                .Nested<ArtistDTO>(n => n
                    .IncludeInRoot()
                    .Name(np => np.Artist)
                    .AutoMap()
                    )
                .Nested<CountryDTO>(n => n
                    .IncludeInRoot()
                    .Name(np => np.Country)
                    .AutoMap()))));
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<RecordDTO>(m => m
                    .Ignore(p => p.Slug));
        }
    }
}

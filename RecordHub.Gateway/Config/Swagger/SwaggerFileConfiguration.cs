using Ocelot.Configuration.File;
using RecordHub.Gateway.Swagger;
using Swashbuckle.AspNetCore.Swagger;

namespace RecordHub.Gateway.Config.Swagger
{
    public class SwaggerFileConfiguration
    {
        public List<SwaggerFileReRoute> ReRoutes { get; set; } = new List<SwaggerFileReRoute>();

        public List<FileDynamicRoute> DynamicReRoutes { get; set; } = new List<FileDynamicRoute>();

        // Seperate field for aggregates because this let's you re-use ReRoutes in multiple Aggregates
        public List<FileAggregateRoute> Aggregates { get; set; } = new List<FileAggregateRoute>();

        public FileGlobalConfiguration GlobalConfiguration { get; set; } = new FileGlobalConfiguration();

        public List<SwaggerEndpointOptions> SwaggerEndPoints { get; set; } = new List<SwaggerEndpointOptions>();
    }
}

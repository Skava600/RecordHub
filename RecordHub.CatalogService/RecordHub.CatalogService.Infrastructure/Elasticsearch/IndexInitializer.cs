using AutoMapper;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Infrastructure.Extensions;

namespace RecordHub.CatalogService.Infrastructure.Elasticsearch
{
    public class IndexInitializer
    {
        private IElasticClient elasticClient;
        private IUnitOfWork repo;
        private IMapper mapper;
        public IndexInitializer(IElasticClient elasticClient, IUnitOfWork repo, IMapper mapper)
        {
            this.elasticClient = elasticClient;
            this.mapper = mapper;
            this.repo = repo;
        }

        public async Task Initialize()
        {
            var result = await elasticClient.Indices.ExistsAsync("records");

            if (!result.Exists)
            {
                ElasticsearchExtensions.CreateIndex(elasticClient, "records");
                var records = await repo.Records.GetByPageAsync(1, 100);
                var recordsDTO = mapper.Map<IEnumerable<RecordDTO>>(records);
                await elasticClient.BulkAsync(b => b.CreateMany(recordsDTO, (ud, d) => ud.Id(d.Id)));
            }
        }
    }
}

using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class ConfigService : IConfigService
    {
        private readonly IConfigRepository _repo;

        public ConfigService(IConfigRepository repo)
        {
            _repo = repo;
        }

        public AutoConfig Get() => _repo.GetConfig();

        public AutoConfig Update(AutoConfig config)
        {
            _repo.UpdateConfig(config);
            return config;
        }
    }
}

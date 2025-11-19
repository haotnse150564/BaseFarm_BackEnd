using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class ConfigRepository : IConfigRepository
    {
        private AutoConfig _config = new AutoConfig();  // giữ giá trị trong RAM

        public AutoConfig GetConfig() => _config;

        public void UpdateConfig(AutoConfig config)
        {
            _config = config;
        }
    }
}

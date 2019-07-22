using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Cleaner.Core.DB.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cleaner.Services
{
    public class DbInfoService : IDbInfoService
    {
        private readonly ILogger<DbInfoService> _logger;
        private readonly AppSettings _appSettings;
        private readonly DataDbContext _dataDbContext;

        public DbInfoService(ILogger<DbInfoService> logger, IOptions<AppSettings> appSettings, DataDbContext dataDbContext)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _dataDbContext = dataDbContext;

            _logger.LogDebug("DbInfoService init...");
        }


        public void Stop()
        {
            _logger.LogDebug("DbInfoService stop...");
        }


        public async Task SearchWordsWithotConnection()
        {
            var entities = await _dataDbContext.Set<Words>().Where(x => string.IsNullOrEmpty(x.Word.Trim()))
                //.Take(100)
                .ToListAsync();
            //var entities = await _dataDbContext.Set<Words>().Where(x => x.BaseWordLinks.Any()).Take(100).ToListAsync();

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(entities[i].Id + "\t" + entities[i].Word);
            //}
            var count = entities.Count();

        }

        public async Task DbInfo()
        {
            var dbCon = _dataDbContext.Database.GetDbConnection();

            _logger.LogInformation(string.Format("{0,-20} = {1}", "ConnectionString", dbCon.ConnectionString));
            _logger.LogInformation(string.Format("{0,-20} = {1}", "Server", dbCon.DataSource));
            _logger.LogInformation(string.Format("{0,-20} = {1}", "Database", dbCon.Database));

            await Task.CompletedTask;
        }
    }
}

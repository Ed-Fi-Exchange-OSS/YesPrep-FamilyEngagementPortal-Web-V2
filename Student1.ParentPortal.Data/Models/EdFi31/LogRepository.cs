﻿using System.Data.Entity;
using System.Threading.Tasks;

namespace Student1.ParentPortal.Data.Models.EdFi31
{
    public class LogRepository : ILogRepository
    {
        private readonly EdFi31Context _edFiDb;

        public LogRepository(EdFi31Context edFiDb)
        {
            _edFiDb = edFiDb;
        }

        public void AddLogEntry(string message, string logType)
        {
                _edFiDb.Logs.Add(new Log() { LogMessage = message, LogType = logType });
                _edFiDb.SaveChanges();
        }


        public async Task AddLogEntryAsync(string message, string logType)
        {
                _edFiDb.Logs.Add(new Log() { LogMessage = message, LogType = logType });
                await _edFiDb.SaveChangesAsync();
        }
    }
}

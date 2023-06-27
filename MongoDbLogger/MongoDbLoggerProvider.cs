﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace MongoDbLogging {
    internal sealed class MongoDbLoggerProvider : ILoggerProvider {
        private  MongoDbLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, MongoDbLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        private readonly IDisposable? _onConfigChange;
        private readonly string _environment;
        private readonly string _serviceName;

        public MongoDbLoggerProvider(IOptionsMonitor<MongoDbLoggerConfiguration> config, ServiceInfo serviceInfo) {
            this._config = config.CurrentValue;
            this._onConfigChange = config.OnChange(updatedConfig => this._config = updatedConfig);
            this._environment = serviceInfo.Environment;
            this._serviceName = serviceInfo.ServiceName;
        }

        public ILogger CreateLogger(string categoryName) {
            return _loggers.GetOrAdd(categoryName, (categoryName) => new MongoDbLogger(_environment,_serviceName, categoryName, GetCurrentConfig));
        }

        private MongoDbLoggerConfiguration GetCurrentConfig() => _config;

        public void Dispose() {
            _loggers.Clear();
            _onConfigChange?.Dispose();
        }        
    }
}

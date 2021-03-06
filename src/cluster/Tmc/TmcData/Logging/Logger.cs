﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TmcData
{
    public enum LogStrategy
    {
        File = 0,
        Database,
        All,
        Console,
        FileAndConsole,
        None
    }

    public sealed class Logger
    {
        private static Logger _instance;
        private List<ILogProvider> _logProviders;

        public LogStrategy Strategy { get; set; }
        public readonly LogStrategy DefaultStrategy;

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        private Logger()
        {
            var fileName = ConfigurationManager.AppSettings["LogFile"].ToString();
            _logProviders = new List<ILogProvider>();
            _logProviders.Add(new FileLogProvider(fileName));
            _logProviders.Add(new DatabaseLogProvider());
            _logProviders.Add(new ConsoleLogProvider());
            DefaultStrategy = LogStrategy.All;
            Strategy = DefaultStrategy;
        }

        public void Write(string message)
        {
            foreach (var p in _logProviders)
            {
                if (MatchesStrategy(p))
                {
                    p.Write(message);
                }
            }
        }

        public void Write(string message, LogType level)
        {
            foreach (var p in _logProviders)
            {
                if (MatchesStrategy(p))
                {
                    p.Write(message, level);
                }
            }
        }

        public void Write(LogEntry message)
        {
            foreach (var p in _logProviders)
            {
                if (MatchesStrategy(p))
                {
                    p.Write(message);
                }
            }
        }

        private bool MatchesStrategy(ILogProvider p)
        {
            var matches = false;

            if (Strategy == LogStrategy.None)
            {
                return false;
            }

            matches |= Strategy == LogStrategy.All;
            matches |= Strategy == p.ProvidedStrategy;
            matches |= (Strategy == LogStrategy.FileAndConsole 
                && (p.ProvidedStrategy == LogStrategy.File ||
                    p.ProvidedStrategy == LogStrategy.Console));

            return matches;
        }
    }
}

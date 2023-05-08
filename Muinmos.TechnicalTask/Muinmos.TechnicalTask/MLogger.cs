﻿namespace Muinmos.TechnicalTask
{
    /// <summary>
    /// Try to log into a text file as fast as possible with Multi-Threading support.
    /// </summary>
    public class MLogger
    {
        #region Settings
        private const string LOG_FOLDER = "logs";
        private const string LOG_FILE_EXT = ".log";
        private const string LOG_FILE_NAME = "log-entries";
        private const int MAX_LOG_NUMBER = 10000;
        #endregion

        #region Properties
        //An array is faster than Queue.
        //In this case, because we know the maximum log number,
        //therefore we can use Array instead of Queue.
        private readonly string[] _buffer;
        private readonly object _lock;

        private int _currentIndex;
        private string _filePath = null!;
        #endregion

        #region Ctor
        public MLogger()
        {
            _lock = new object();
            _buffer = new string[MAX_LOG_NUMBER];
            ResetIndex();
            GenerateNewLogFile();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// A new log file will be generated by this method, 
        /// After reaching the maximum number of logs <see cref="MAX_LOG_NUMBER"/>
        /// </summary>
        private void GenerateNewLogFile()
        {
            //I don't use lock here
            //because the method is called inside ctor or log method which has a lock statement

            if (!Directory.Exists(LOG_FOLDER)) Directory.CreateDirectory(LOG_FOLDER);

            //add Guid at the end of the file name to ensure the file name will be unique
            _filePath = $"{LOG_FOLDER}/" +
                $"{LOG_FILE_NAME}-" +
                $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff")}-" +
                $"{Guid.NewGuid()}{LOG_FILE_EXT}";

            //create the file and close it, therefore other threads can use it.
            File.WriteAllText(_filePath, "");
        }

        /// <summary>
        /// <see cref="_currentIndex"/> is used to navigate through the buffer array <see cref="_buffer"/>
        /// after reaching to the end of the _buffer range it must be reset to zero 
        /// and a new file must be created.
        /// </summary>
        private void ResetIndex()
        {
            //I don't use lock here
            //because the method is called inside ctor or log method which has a lock statement

            _currentIndex = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a log string and add it to the <see cref="_buffer"/>
        /// </summary>
        /// <param name="message">custom message to create log string</param>
        public void Log(string message)
        {
            lock (_lock)
            {
                if (_currentIndex == MAX_LOG_NUMBER)
                {
                    Flush();
                    ResetIndex();
                    GenerateNewLogFile();
                }
                _buffer[_currentIndex++] = $"{_currentIndex} | {DateTime.UtcNow} | {message}";
            }
        }

        /// <summary>
        /// Writes all logs inside <see cref="_buffer"/> into the file, then close and flushes the file.
        /// </summary>
        public void Flush()
        {
            lock (_lock)
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    int c = 0;
                    while (c < _currentIndex)
                        writer.WriteLine(_buffer[c++]);
                }
            }
        }
        #endregion
    }
}
